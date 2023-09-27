/*************************************************************************************************************************************
Purpose:
* This procedure will gather token orders for reporting
**************************************************************************************************************************************
* Revision History 
**************************************************************************************************************************************
* Name								Date							Description
* ------------------------------------------------------------------------------------------------------------------------------------
* SMT		 						2023-09-10						Created.
**************************************************************************************************************************************
Example: 
	spGetUserActivityLog @ReportDateFrom='2023-01-01', @ReportDateTo='2023-12-31', @userid=5736, @ActionName ='ROSTER',@Page=1,@PageSize=5
**************************************************************************************************************************************/

DROP PROCEDURE IF EXISTS dbo.spGetUserActivityLog;
GO
CREATE PROCEDURE spGetUserActivityLog
	@ReportDateFrom		DATE = NULL,
	@ReportDateTo		DATE = NULL,
    @UserName NVARCHAR(MAX) = NULL,
    @UserId INT = NULL,
    @ActionName NVARCHAR(MAX) = NULL,
	@Page INT = 1,
	@PageSize INT = 1000
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @Total AS INT = 0

	SET @Page = IIF(@Page < 0, 1, @Page)
	SET @PageSize = IIF(@PageSize < 0, 1000, @PageSize)

    SELECT 
        al.[GroupId],
		--COALESCE(al.UserName, al.UserId) UserName,
		al.UserName,
        al.[ActionName],
        aD.[AuditLogId],
        al.[EventDateTime],
		al.[RecordId],
		CASE al.LogType
            WHEN 0 THEN 'Added'
            WHEN 1 THEN 'Deleted'
            WHEN 2 THEN 'Modified'
            WHEN 3 THEN 'SoftDeleted'
            WHEN 4 THEN 'UnDeleted'
            ELSE 'Unknown'
        END AS LogType,
		al.[Remarks],
		ad.[PropertyName],
		dbo.fnGetAuditLogDescription(ad.[PropertyName], ad.[OriginalValue]) as OldVal,
        dbo.fnGetAuditLogDescription(ad.[PropertyName], ad.[NewValue]) as NewVal
        --ad.[OriginalValue],
        --ad.[NewValue]
	INTO #TempResults
    FROM
        [dbo].[AuditLogs] AS al
    JOIN
        [dbo].[AuditLogDetails] AS ad
    ON
        al.[Id] = ad.[AuditLogId]
    WHERE
		al.GroupId IS NOT NULL 
        AND ((@UserName IS NULL OR al.[UserName] = @UserName)
        AND (@UserId IS NULL OR al.[UserId] = @UserId)
		AND
        (
            (
                al.LogType = 0
                AND ad.[PropertyName] NOT IN ('Id', 'CreatedBy', 'CreatedDate', 'IsActive', 'UpdatedBy', 'UpdatedDate')
            )
            OR
            (
                al.LogType <> 0
                AND ad.[PropertyName] NOT IN ('CreatedBy', 'CreatedDate', 'UpdatedBy', 'UpdatedDate')
            )
        )
		AND
        (
            (@ActionName IS NULL)
            OR
            (
                @ActionName = 'Roster'
                AND al.[ActionName] LIKE 'ROSTER_%'
            )
			OR
            (
                @ActionName = 'Order'
                AND al.[ActionName] LIKE 'ORDER_%'
            )
            OR
            (
                @ActionName IS NOT NULL
                AND al.[ActionName] = @ActionName
            )
        ))
		AND (CONVERT(DATE, al.EventDateTime) BETWEEN @ReportDateFrom AND @ReportDateTo)
    ORDER BY
        aD.[AuditLogId] ASC;

	SELECT GroupId, UserName, Remarks
	INTO #TempGrp
	FROM #TempResults 
	GROUP BY GroupId, UserName, Remarks

	SELECT @Total = COUNT(GroupId) FROM #TempGrp 

	SELECT @Total AS Total, * 
	INTO #Results
	FROM #TempGrp 
	ORDER BY GroupId ASC
	OFFSET (@Page - 1) * @PageSize ROWS 
	FETCH NEXT @PageSize ROWS ONLY

	-- select header
	SELECT * FROM #Results

	-- select details
	SELECT r.* FROM #TempResults r
	JOIN #Results g ON g.GroupId = r.GroupId

END
