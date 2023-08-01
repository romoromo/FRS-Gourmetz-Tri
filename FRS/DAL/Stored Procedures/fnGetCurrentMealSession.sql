DROP FUNCTION [dbo].[fnGetCurrentMealSession]
GO
CREATE FUNCTION fnGetCurrentMealSession
(
	@OutletId INT,
	@MealSessionId INT,
	@OrderDate DATE,
	@ClassId INT
)
RETURNS @ret TABLE
(
    Id int primary key NOT NULL,
    Name VARCHAR(MAX) NOT NULL,
    StartDate DATE NOT NULL,
	EndDate DATE NULL,
	RouteTime DATE NULL,
	OverheadTime DATE NULL,
	CalSourceTime DATE NULL,
	RouteInterval REAL NULL,
	OverheadInterval REAL NULL,
	RouteId INT NULL
)
AS
BEGIN
	DECLARE @numOfDays INT, @span INT, @day INT, @d INT, @crStartDate DATE, @scheduleId INT
	DECLARE @T TABLE
	(
		[Id] [int] NOT NULL,
		[IsActive] [bit] NOT NULL,
		[CreatedBy] [int] NULL,
		[UpdatedBy] [int] NULL,
		[UpdatedDate] [datetime2](7) NOT NULL,
		[CreatedDate] [datetime2](7) NOT NULL,
		[OutletClassRosterId] [int] NOT NULL,
		[Day] [int] NOT NULL,
		[StartDate] DATE NOT NULL
	)

	INSERT INTO @T
    SELECT ocrs.* , cr.StartDate
	FROM OutletClassRosterSchedules ocrs WITH (NOLOCK) 
	--INNER JOIN OutletProfiles p  WITH (NOLOCK) ON p.Id = cr.OutletProfileId
	--INNER JOIN Caterers c with (nolock) on c.Id = p.CatererId
	INNER JOIN OutletClassRosters cr on ocrs.OutletClassRosterId = cr.Id
	INNER JOIN CatererOutlets co with (nolock) on co.OutletProfileId = cr.OutletProfileId
	WHERE co.OutletId = @OutletId AND co.IsActive = 1 
	AND cr.IsActive = 1 AND cr.MealSessionId = @MealSessionId AND @OrderDate BETWEEN cr.StartDate AND cr.EndDate
	AND ocrs.IsActive = 1;

	SELECT @numOfDays = COUNT(DISTINCT ocrs.Id)
	FROM @T ocrs;

	SET @crStartDate = (SELECT TOP 1 StartDate FROM @T);

	SET @span = DATEDIFF(DAY, @OrderDate, @crStartDate) + 1;
	SET @day = IIF(@span = 0, 1, @span % @numOfDays)
	SET @day = IIF(@day = 0 , @numOfDays, @day)
	SET @scheduleId = (SELECT TOP 1 Id FROM @T WHERE [Day] = @day);

	INSERT INTO @ret
	SELECT TOP 1
		md.Id,
		md.Name,
		md.StartDate,
		md.EndDate,
		md.RouteTime,
		md.OverheadTime,
		md.CalSourceTime,
		md.RouteInterval,
		md.OverheadInterval,
		md.RouteId
	FROM MealSessionDetails md with (nolock)
	--INNER JOIN MealSessions ms with (nolock) ON ms.Id = md.MealSessionId
	INNER JOIN OutletClassRosterSchedulePeriods sp with (nolock) ON sp.MealSessionDetailId = md.Id
	INNER JOIN OutletClassRosterSchedulePeriodClasses spc with (nolock) ON spc.OutletClassRosterSchedulePeriodId = sp.Id
	WHERE md.MealSessionId=@MealSessionId AND spc.ClassId IN (@ClassId)
	AND sp.OutletClassRosterScheduleId = @scheduleId

	RETURN
END;