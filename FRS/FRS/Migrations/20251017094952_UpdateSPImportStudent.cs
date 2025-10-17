using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSPImportStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

---- Create the stored procedure using the TVP
---- Test script for ImportStudentData stored procedure
--DECLARE @StudentData dbo.tvpStudents;

---- Populate the TVP with sample data
--INSERT INTO @StudentData (ClassName, BatchName, Name, AssociatedEmail, Email, OutletId, CardId, CardNumber, AltEmail, CreatedBy, PasswordHashed)
--VALUES
--    ('2CR', 2023, 'John Doe', 'john@example.com,jane@example.com', 'john@example.com', 1008, 'Card001', '12345', 'jane123@example.com', 1, '$2a$12$AaJX.Qrvmx4eIYgLEYSBqeP6pYv5xC3TY7pV9bN5n5DjSD9ytlT7S'),
--    ('2CR', 2023, 'Jane Doe', 'jane@example.com', 'jane@example.com', 1008, 'Card002', '67890', 'john123@example.com', 1, '$2a$12$AaJX.Qrvmx4eIYgLEYSBqeP6pYv5xC3TY7pV9bN5n5DjSD9ytlT7S');

---- Execute the stored procedure
--EXEC dbo.ImportStudentData @StudentData;

ALTER   PROCEDURE [dbo].[ImportStudentData]
    @StudentData dbo.tvpStudents READONLY
AS
BEGIN
    DECLARE @ClassName NVARCHAR(255), @BatchName INT, @Name NVARCHAR(255),
            @AssociatedEmail NVARCHAR(1000), @Email NVARCHAR(255),
            @OutletId INT, @CardId NVARCHAR(255), @CardNumber NVARCHAR(255), @AltEmail NVARCHAR(255), @CreatedBy INT, @PasswordHashed NVARCHAR(500), @InstitutionId INT, @SecurityStamp NVARCHAR(500), @ConcurrencyStamp NVARCHAR(500), @IssueDate DATE;

	DECLARE @AssociatedEmailList NVARCHAR(MAX);
	SELECT @AssociatedEmailList = STUFF((SELECT ',' + AssociatedEmail
										 FROM @StudentData
										 FOR XML PATH('')), 1, 1, '');

	-- Create a temporary table to store parent account IDs
    CREATE TABLE #TempParentAccounts (ParentAccountId INT, Email NVARCHAR(255));

	-- Insert all ParentAccounts associated with provided emails
    INSERT INTO #TempParentAccounts (ParentAccountId, Email)
    SELECT DISTINCT pa.Id, pa.email
    FROM [User] pa
	WHERE pa.Email IN (SELECT TRIM(value) FROM STRING_SPLIT(@AssociatedEmailList, ','));
    --WHERE pa.Email IN (SELECT TRIM(value) FROM STRING_SPLIT((SELECT STRING_AGG(AssociatedEmail, ',') FROM @StudentData), ','));


    DECLARE rowCursor CURSOR FOR
    SELECT ClassName, BatchName, Name, AssociatedEmail, Email, OutletId, CardId, CardNumber, AltEmail, CreatedBy, PasswordHashed, InstitutionId, SecurityStamp, ConcurrencyStamp, IssueDate
    FROM @StudentData;

    OPEN rowCursor;

    FETCH NEXT FROM rowCursor INTO @ClassName, @BatchName, @Name, @AssociatedEmail, @Email, @OutletId, @CardId, @CardNumber, @AltEmail, @CreatedBy, @PasswordHashed, @InstitutionId, @SecurityStamp, @ConcurrencyStamp, @IssueDate;

    WHILE @@FETCH_STATUS = 0
BEGIN
	--select * from @StudentData
	SET @CreatedBy = COALESCE(@CreatedBy, 1);
	SET @CreatedBy = IIF(@CreatedBy = 0, 1, @CreatedBy);

	SET @InstitutionId = COALESCE(@InstitutionId, 1);
	SET @InstitutionId = IIF(@InstitutionId = 0, 1, @InstitutionId);

	PRINT '@CreatedBy' + cast(@CreatedBy as varchar)
    -- Check if the class exists
    IF NOT EXISTS (SELECT 1 FROM Classes WHERE Name = @ClassName AND ClassLevelId IN (SELECT Id from ClassLevels WHERE IsActive = 1 AND OutletId = @OutletId))
    BEGIN
        -- Class not found, throw an exception or handle the error as needed
        THROW 50000, 'Class not found. Please check the imported file.', 1;
    END

    -- Get ClassLevelId for the class
    DECLARE @ClassLevelId INT, @ClassId INT;
    SELECT @ClassLevelId = c.ClassLevelId, @ClassId = c.Id
    FROM Classes c
    WHERE c.Name = @ClassName AND c.IsActive = 1  AND ClassLevelId IN (SELECT Id from ClassLevels WHERE IsActive = 1 AND OutletId = @OutletId);

	-- Get ClassLevelId
    --SELECT @ClassLevelId = Id
    --FROM ClassLevels
    --WHERE @ClassLevelId = Id AND OutletId = @OutletId;

    -- Class level not provided, create one from class name
  --  IF @ClassLevelId IS NULL
  --  BEGIN
  --      INSERT INTO ClassLevels (Name, Year, OutletId, IsActive, CreatedBy, UpdatedBy, UpdatedDate, CreatedDate)
  --      VALUES (@ClassName, YEAR(GETDATE()), @OutletId, 1, @CreatedBy, @CreatedBy, GETDATE(), GETDATE());

		--SET @ClassLevelId = SCOPE_IDENTITY();
  --  END

    -- Get BatchId
    DECLARE @BatchId INT;
    SELECT @BatchId = Id
    FROM ClassBatches
    WHERE Year = @BatchName AND IsActive = 1 AND OutletId = @OutletId;

    -- Batch not provided, create one from class name
    IF @BatchId IS NULL
    BEGIN
        INSERT INTO ClassBatches (Name, Year, IsActive, CreatedBy, UpdatedBy, UpdatedDate, CreatedDate, OutletId)
        VALUES (CONVERT(NVARCHAR(255), @BatchName), @BatchName, 1, @CreatedBy, @CreatedBy, GETDATE(), GETDATE(), @OutletId);

		SET @BatchId = SCOPE_IDENTITY();
    END

	select * from #TempParentAccounts;

	DECLARE @StudentId INT = NULL;
    -- Check if student exists and has duplicates, check for the parent's email
    SELECT TOP 1 @StudentId = s.Id
    FROM Students s
	INNER JOIN StudentManageAccounts sm ON s.Id = sm.StudentId
	INNER JOIN #TempParentAccounts pa ON pa.ParentAccountId = sm.UserId
    WHERE s.IsActive =1 AND s.Name = @Name AND s.OutletId = @OutletId 
			AND pa.Email IN (SELECT TRIM(SplitValue) FROM SplitAndReturnTable(@AssociatedEmail));

	SELECT @ClassName, @BatchName, @Name, @AssociatedEmail, @Email, @OutletId, @CardId, @CardNumber, @AltEmail, @CreatedBy, @PasswordHashed, @InstitutionId, @SecurityStamp, @ConcurrencyStamp, @IssueDate
	--PRINT '1' + CAST(@StudentId AS VARCHAR)
	-- If student not found, search email
	IF @StudentId IS NULL
		BEGIN
			SELECT TOP 1 @StudentId = s.Id
			FROM Students s
			WHERE s.IsActive =1 AND s.Email = @Email AND s.OutletId = @OutletId;
		END

	--PRINT '2' + CAST(@StudentId AS VARCHAR)
	-- If student not found, search email in the associated email. It means parent's email is being used
	IF @StudentId IS NULL
		BEGIN
			SELECT TOP 1 @StudentId = s.Id
			FROM Students s
			INNER JOIN StudentManageAccounts sm ON s.Id = sm.StudentId
			INNER JOIN #TempParentAccounts pa ON pa.ParentAccountId = sm.UserId
			WHERE s.IsActive =1 AND s.Name = @Name AND s.OutletId = @OutletId 
					AND s.Email IN (SELECT TRIM(SplitValue) FROM SplitAndReturnTable(@AssociatedEmail));
		END

		--PRINT '3' + CAST(@StudentId AS VARCHAR)

	-- If student not found, verify if alt email exists already
	IF @StudentId IS NULL
		BEGIN
			SELECT TOP 1 @StudentId = s.Id
			FROM Students s
			WHERE s.IsActive =1 AND s.Email = @AltEmail AND s.OutletId = @OutletId;
		END

		--PRINT '4' + CAST(@StudentId AS VARCHAR)
    IF @StudentId IS NOT NULL
    BEGIN
		--PRINT '5' + CAST(@StudentId AS VARCHAR)
        -- Student exists, update
        UPDATE Students
        SET Name = @Name,
            ClassBatchId = @BatchId,
            ClassLevelId = @ClassLevelId,
			ClassId = @ClassId,
            Email = COALESCE(Students.Email, @Email, @AltEmail)
		WHERE Id = @StudentId;

        -- Disable existing cards
        UPDATE StudentCards
        SET IsActive = 0
        WHERE StudentId = @StudentId AND CardId <> @CardId;

        -- Insert or update StudentCard
        MERGE INTO StudentCards AS Target
        USING (VALUES (@StudentId, @CardId, @CardNumber)) AS Source (StudentId, CardId, CardNumber)
        ON Target.StudentId = Source.StudentId AND Target.CardId = Source.CardId
        WHEN MATCHED THEN
            UPDATE SET
                Target.Status = 'ACTIVE',
                Target.Remarks = Source.CardNumber,
				IsActive = 1,
				CreatedDate = COALESCE(@IssueDate, CreatedDate, GETDATE())
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (StudentId, CardId, Remarks, Status, IsActive, CreatedBy, UpdatedBy, UpdatedDate, CreatedDate)
            VALUES (Source.StudentId, Source.CardId, Source.CardNumber, 'ACTIVE', 1, @CreatedBy, @CreatedBy, GETDATE(), COALESCE(@IssueDate, GETDATE()));
    END
    ELSE
    BEGIN
		DECLARE @GeneratedEmail VARCHAR(500);
		SET @GeneratedEmail = dbo.GenerateStudentEmail(@ClassName, @Name, @OutletId);
		--PRINT '6' + CAST(@StudentId AS VARCHAR)
		--IF LEN(COALESCE(NULLIF(COALESCE(@Email, @AltEmail), ''), @GeneratedEmail)) > 0
		--BEGIN
			-- Student doesn't exist, insert
			INSERT INTO Students (ClassBatchId, ClassId, ClassLevelId, Name, Email, OutletId, IsFAS, newPassword, IsActive, CreatedBy, UpdatedBy, UpdatedDate, CreatedDate)
			VALUES (@BatchId, @ClassId, @ClassLevelId, @Name, COALESCE(NULLIF(COALESCE(@Email, @AltEmail), ''), @GeneratedEmail), @OutletId, 0, 'We1come@YISS', 1, @CreatedBy, @CreatedBy, GETDATE(), GETDATE());

			SET @StudentId = SCOPE_IDENTITY();
		--END

        -- Insert StudentCard
		IF LEN(@CardId) > 0
		BEGIN
			INSERT INTO StudentCards (StudentId, CardId, Remarks, Status, IsActive, CreatedBy, UpdatedBy, UpdatedDate, CreatedDate)
			VALUES (@StudentId, @CardId, @CardNumber, 'ACTIVE', 1, @CreatedBy, @CreatedBy, GETDATE(), COALESCE(@IssueDate, GETDATE()));
		END
    END
	
	IF LEN(TRIM(@AssociatedEmail)) != 0
    BEGIN
		-- Add parent's account if exist
		MERGE INTO [User] AS Target
		USING (
			SELECT TRIM(value) AS AssociatedEmail
			FROM STRING_SPLIT(@AssociatedEmail, ',')
		) AS Source
		ON Target.Email = Source.AssociatedEmail
		WHEN MATCHED AND Target.IsActive = 0 THEN
			UPDATE SET IsActive = 1,IsPasswordMustChange = 1,PasswordHash = @PasswordHashed,SecurityStamp = @SecurityStamp,ConcurrencyStamp = @ConcurrencyStamp
		WHEN NOT MATCHED BY TARGET THEN
			INSERT (Email, UserName, NormalizedUserName, NormalizedEmail, IsEnabled, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, PasswordHash, AccessFailedCount, InstitutionId, SecurityStamp, ConcurrencyStamp, IsActive, CreatedBy, UpdatedBy, UpdatedDate, CreatedDate,IsPasswordMustChange)
			VALUES (Source.AssociatedEmail, 
			SUBSTRING(Source.AssociatedEmail, 1, 
				CASE 
					WHEN CHARINDEX('@', Source.AssociatedEmail) > 0 
					THEN CHARINDEX('@', Source.AssociatedEmail) - 1
					ELSE LEN(Source.AssociatedEmail) -- Use the entire length if '@' is not found
				END
			), UPPER(Source.AssociatedEmail), UPPER(
			SUBSTRING(Source.AssociatedEmail, 1, 
				CASE 
					WHEN CHARINDEX('@', Source.AssociatedEmail) > 0 
					THEN CHARINDEX('@', Source.AssociatedEmail) - 1
					ELSE LEN(Source.AssociatedEmail) -- Use the entire length if '@' is not found
				END
			)), 1, 1, 1, 0, 1, @PasswordHashed, 0, @InstitutionId, @SecurityStamp, @ConcurrencyStamp, 1, @CreatedBy, @CreatedBy, GETDATE(), GETDATE(),1);

		--PRINT '8888' + CAST(@StudentId AS VARCHAR)
		--PRINT '44' + @SecurityStamp
		--PRINT '55' + @ConcurrencyStamp
		MERGE INTO StudentManageAccounts AS Target
		USING (
			SELECT
				SSA.StudentId,
				AU.Id AS UserId,
				TRIM(AES.value) AS AssociatedEmail
			FROM STRING_SPLIT(@AssociatedEmail, ',') AES
			JOIN [User] AU ON AU.Email = TRIM(AES.value)
			LEFT JOIN StudentManageAccounts SSA ON SSA.UserId = AU.Id
				AND SSA.StudentId = @StudentId
			WHERE AU.Id IS NOT NULL
		) AS Source
		ON Target.UserId = Source.UserId AND Target.StudentId = Source.StudentId
		WHEN NOT MATCHED BY TARGET THEN
			INSERT (UserId, StudentId, IsActive, CreatedBy, UpdatedBy, UpdatedDate, CreatedDate)
			VALUES (Source.UserId, @StudentId, 1, @CreatedBy, @CreatedBy, GETDATE(), GETDATE());
	END


    FETCH NEXT FROM rowCursor INTO @ClassName, @BatchName, @Name, @AssociatedEmail, @Email, @OutletId, @CardId, @CardNumber, @AltEmail, @CreatedBy, @PasswordHashed, @InstitutionId, @SecurityStamp, @ConcurrencyStamp, @IssueDate;
END

CLOSE rowCursor;
DEALLOCATE rowCursor;
END;
GO

            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
