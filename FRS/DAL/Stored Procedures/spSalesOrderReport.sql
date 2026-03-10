/*************************************************************************************************************************************
Purpose:
* This procedure will gather token orders for reporting
**************************************************************************************************************************************
* Revision History 
**************************************************************************************************************************************
* Name								Date							Description
* ------------------------------------------------------------------------------------------------------------------------------------
* SMT		 						2023-04-04						Created.
**************************************************************************************************************************************
Example: 
	exec [dbo].[spSalesOrderReport] '2022-01-01', '2023-04-01', 'cancelled', NULL, -1, -1, '', 'DeliveryDate', 1
	exec [dbo].[spSalesOrderReport] '2022-12-01', '2023-04-01', 'paid', NULL, 1, 10, '1', 'DeliveryDate', 0
**************************************************************************************************************************************/

DROP PROCEDURE IF EXISTS dbo.spSalesOrderReport;
GO
CREATE PROCEDURE [dbo].[spSalesOrderReport]
					@ReportDateFrom		DATE = NULL,
					@ReportDateTo		DATE = NULL,
					@Status	VARCHAR(100) = NULL,
					@IsFAS	BIT = NULL,
					@IsActive	BIT = NULL,
					@Page INT = 1,
					@PageSize INT = 1000,
					@Keywords VARCHAR(MAX) = NULL,
					@SortBy	VARCHAR(MAX) = NULL,
					@SortDirection	BIT = NULL,
					@ReportType	VARCHAR(MAX) = 1,
					@StudentGroupIds	VARCHAR(MAX) = NULL,
					@OrderType	VARCHAR(100) = NULL,
					@CollectionStatuses	VARCHAR(MAX) = NULL,
					@OutletId VARCHAR(MAX) = NULL,
					@ClassLevelIds VARCHAR(MAX) = NULL

				AS 
				BEGIN
					DECLARE @Total AS INT = 0;
					DECLARE @IsCollected AS VARCHAR(100) = NULL;
					DECLARE @IsNotCollected AS VARCHAR(100) = NULL

					SET @Page = IIF(@Page < 0, 1, @Page)
					SET @PageSize = IIF(@PageSize < 0, 1000, @PageSize)
					SET @ReportType = IIF(ISNULL(@ReportType, 0) = 0, 1, @ReportType)
					SELECT @IsCollected = SplitValue FROM SplitAndReturnTable(@CollectionStatuses) WHERE SplitValue = 'COLLECTED'
					SELECT @IsNotCollected = SplitValue FROM SplitAndReturnTable(@CollectionStatuses) WHERE SplitValue = 'NOT COLLECTED'

					SELECT 
						DISTINCT
						o.Id,
						o.PaymentId,
						p.InvoiceNumber,
						p.PaymentNumber,
						pt.Name as PaymentMethod,
						s.Id as StudentId,
						s.Name as StudentName,
						ot.Name as Outlet,
						c.Name as ClassName,
						o.TransactionTime as OrderDate,
						o.Status as Status,
						ISNULL(v.Name, '') as DiscountCode ,
						ISNULL(cor.Status, 'na') as CancelRequestStatus ,
						CAST(ISNULL(p.subtotal, 0) as real) as Subtotal,
						CAST(ISNULL(p.subdisctotal, 0) as real) as SubDiscTotal,
						CAST(ISNULL(p.discount, 0) as real) as Discount,
						CAST(ISNULL(p.gst, 0) as real) as PaymentGst,
						CAST(ISNULL(p.transactionFee, 0) as real) as PaymentTransactionFee,
						CAST(ISNULL(p.fixedTransactionFee, 0) as real) as PaymentFixedTransactionFee,
						CAST(ISNULL(p.total, 0) as real) as PaymentTotalAmount,
						ISNULL(p.fomoid, '') as FomoId,
						ISNULL(p.Status,'') as PaymentStatus,
						o.DeliveryDate,
						ISNULL(md.MealSessionName,'') as MealSessionDetailName,
						ISNULL(dt.Name,'') as MealTypeName,
						ISNULL(dish.Label,'') as DishLabel,
						ISNULL(d.Qty, 0) as Quantity,
						o.TotalAmount as DishPrice,
						ISNULL(FORMAT (o.CollectionTime , 'dd-MMM-yyyy hh:mm tt'),'') AS CollectionTime,
						ISNULL(o.BentoCode, '') as BentoCode,
						ISNULL(FORMAT (o.ReturnTime , 'dd-MMM-yyyy hh:mm tt'),'') AS ReturnTime,
						--s.IsFAS,
						(o.IsFAS | s.IsFAS) AS IsFAS,
						ISNULL(o.CancellationReason, '') as CancellationReason,
						ISNULL(FORMAT (o.CancelledOn , 'dd-MMM-yyyy hh:mm tt'),'') AS CancelledOn,
						CASE 
						  WHEN @ReportType = 1 THEN 1 
						  ELSE ((SELECT COUNT(1) FROM Payments pay WITH (NOLOCK) WHERE p.InvoiceNumber = pay.InvoiceNumber)) 
					   END AS OrderCount,
						--u.UserName as ProcessedBy
						--sg.Name as StudentGroupName
						(SELECT TOP 1 sgs.name FROM StudentGroups sgs WHERE sgs.Id = o.StudentGroupId) as StudentGroupName
						--o.IsStudentGroupOrder,
						--o.IsMealPlan
					INTO #TempResults
					FROM TokenOrders o WITH (NOLOCK)
					LEFT JOIN TokenOrdereds t WITH (NOLOCK) ON t.OrderId = o.Id
					LEFT JOIN TokenOrderDishes d WITH (NOLOCK) ON d.TokenOrderedId = t.Id
					LEFT JOIN Students s WITH (NOLOCK) ON s.Id = o.ProfileId
					LEFT JOIN Outlets ot WITH (NOLOCK) ON ot.Id = s.OutletId
					LEFT JOIN Classes c WITH (NOLOCK) ON c.Id = s.ClassId
					LEFT JOIN Payments p WITH (NOLOCK) ON p.Id = o.PaymentId
					LEFT JOIN MealSessionDetails md WITH (NOLOCK) ON md.Id = o.MealSessionDetailId
					LEFT JOIN MealTypes mt WITH (NOLOCK) ON mt.Id = t.TokenId
					LEFT JOIN Dishes dish WITH (NOLOCK) on dish.Id = d.DishId
					LEFT JOIN DishTypes dt WITH (NOLOCK) on dt.Id = dish.DishTypeId
					LEFT JOIN Vouchers v WITH (NOLOCK) ON v.Id = p.VoucherId
					LEFT JOIN PaymentTypes pt WITH (NOLOCK) ON pt.Id = p.PaymentTypeId
					LEFT JOIN StudentGroupDetails sgd WITH (NOLOCK) ON sgd.StudentId = s.Id
					LEFT JOIN StudentGroups sg WITH (NOLOCK) ON sg.Id = sgd.StudentGroupId
					LEFT JOIN CancelOrderRequests cor WITH (NOLOCK) ON cor.OrderId = o.Id
					--LEFT JOIN [User] u WITH (NOLOCK) ON u.Id = o.CreatedBy
					WHERE 
						(((@ReportType = 1 OR @ReportType = 0 OR @ReportType = 3) AND (CONVERT(DATE, o.DeliveryDate) BETWEEN @ReportDateFrom AND @ReportDateTo)) OR
						(@ReportType = 2 AND (CONVERT(DATE, o.CancelledOn) BETWEEN @ReportDateFrom AND @ReportDateTo)) --OR
						--(@ReportType = 3 AND (CONVERT(DATE, o.CollectionTime) BETWEEN @ReportDateFrom AND @ReportDateTo))
						)
						AND (ISNULL(@Status, '') ='' OR o.Status IN (@Status))
						AND (ISNULL(@OrderType, '') ='' OR ((@OrderType = 'Adhoc' AND @OrderType = pt.Name AND o.IsStudentGroupOrder = 1) OR (@OrderType = 'FAS' AND o.IsFAS = 1) OR (@OrderType = 'Meal Plan' AND @OrderType = pt.Name AND o.IsMealPlan = 1)))
						AND (@IsFAS IS NULL OR s.IsFAS = @IsFAS)
						AND (@IsActive IS NULL OR o.IsActive = @IsActive)
						--AND (@ReportType <> 3 OR (@ReportType = 3 AND sg.Id = @StudentGroupId))
						AND (LEN(ISNULL(@StudentGroupIds, '')) = 0 OR (o.StudentGroupId IN (SELECT SplitValue FROM SplitAndReturnTable(@StudentGroupIds))))
						AND (LEN(ISNULL(@CollectionStatuses, ''))= 0 OR (LEN(ISNULL(@IsCollected, '')) <> 0 AND o.CollectionTime IS NOT NULL) OR (LEN(ISNULL(@IsNotCollected, '')) <> 0 AND o.CollectionTime IS NULL))
						AND (ISNULL(@Keywords, '') = '' OR 
							s.Name LIKE '%' + @Keywords + '%' OR
							c.Name LIKE '%' + @Keywords + '%' OR 
							p.InvoiceNumber LIKE '%' + @Keywords + '%' OR
							v.Code LIKE '%' + @Keywords + '%' OR
							dish.Label LIKE '%' + @Keywords + '%')
						AND (LEN(ISNULL(@OutletId, '')) = 0 OR (ot.Id IN (SELECT SplitValue FROM SplitAndReturnTable(@OutletId))))
						AND (LEN(ISNULL(@ClassLevelIds, '')) = 0 OR (s.ClassLevelId IN (SELECT SplitValue FROM SplitAndReturnTable(@ClassLevelIds))))
	
					SELECT @Total = COUNT(Id) FROM #TempResults 

					SELECT @Total AS Total, * FROM #TempResults 
					ORDER BY 
					CASE WHEN @SortDirection = 0 THEN
						CASE
						   WHEN @SortBy = 'deliveryDate' THEN DeliveryDate
						   WHEN @SortBy = 'profileName' THEN StudentName 
						END
					END ASC
					, CASE WHEN @SortDirection = 1 THEN
						CASE
						   WHEN @SortBy = 'deliveryDate' THEN DeliveryDate
						   WHEN @SortBy = 'profileName' THEN StudentName  
						END
					END DESC
					OFFSET (@Page - 1) * @PageSize ROWS 
					FETCH NEXT @PageSize ROWS ONLY
				END