/*************************************************************************************************************************************
Purpose:
* This procedure will get current meal session
**************************************************************************************************************************************
* Revision History 
**************************************************************************************************************************************
* Name								Date							Description
* ------------------------------------------------------------------------------------------------------------------------------------
* SMT		 						2023-04-04						Created.
**************************************************************************************************************************************
Example: 
	exec [dbo].[spRetrieveSalesOrder] '2022-12-01', '2023-04-01', '', NULL, -1, -1, '', 'profileName', 1
	exec [dbo].[spRetrieveSalesOrder] '2022-12-01', '2023-04-01', 'paid', NULL, 1, 10, '', 'profileName', 1
	exec [dbo].[spRetrieveSalesOrder] '2022-12-01', '2023-04-01', 'paid', NULL, 1, 100000, '', 'profileName', 0
**************************************************************************************************************************************/

DROP PROCEDURE IF EXISTS dbo.spRetrieveSalesOrder;
GO
CREATE PROCEDURE spRetrieveSalesOrder
	@ReportDateFrom		DATE = NULL,
	@ReportDateTo		DATE = NULL,
	@Status	VARCHAR(100) = NULL,
	@IsFAS	BIT = NULL,
	@Page INT = 1,
	@PageSize INT = 1000,
	@Keywords VARCHAR(MAX) = NULL,
	@SortBy	VARCHAR(MAX) = NULL,
	@SortDirection	BIT = 0
AS 
BEGIN
	DECLARE @Total AS INT = 0

	SET @Page = IIF(@Page < 0, 1, @Page)
	SET @PageSize = IIF(@PageSize < 0, 1000, @PageSize)

	IF OBJECT_ID(N'tempdb..#TempResults') IS NOT NULL
	BEGIN
		DROP TABLE #TempResults
	END
	

	SELECT 
		DISTINCT
		o.Id,
		COALESCE(o.PaymentId, 0) AS PaymentId,
		COALESCE(p.InvoiceNumber, '') as InvoiceNumber,
		COALESCE(s.Name, '') as StudentName,
		COALESCE(ot.Name, '') as Outlet,
		COALESCE(c.Name, '') as ClassName,
		o.TransactionTime as OrderDate,
		COALESCE(o.Status, '') as Status,
		COALESCE(v.Name, '') as DiscountCode ,
		CAST(COALESCE(p.discount, 0) as real) as Discount,
		CAST(COALESCE(p.gst, 0) as real) as PaymentGst,
		CAST(COALESCE(p.transactionFee, 0) as real) as PaymentTransactionFee,
		CAST(COALESCE(p.fixedTransactionFee, 0) as real) as PaymentFixedTransactionFee,
		CAST(COALESCE(p.total, 0) as real) as PaymentTotalAmount,
		COALESCE(p.fomoid, '') as FomoId,
		COALESCE(p.Status,'') as PaymentStatus,
		o.DeliveryDate,
		COALESCE(md.Name,'') as MealSessionDetailName,
		COALESCE(mt.Name,'') as MealTypeName,
		COALESCE(dish.Label,'') as DishLabel,
		COALESCE(d.Qty, 0) as Quantity,
		COALESCE(o.TotalAmount,0) as DishPrice,
		COALESCE(FORMAT (o.CollectionTime , 'dd-MMM-yyyy hh:mm tt'),'') AS CollectionTime,
		COALESCE(o.BentoCode, '') as BentoCode,
		COALESCE(FORMAT (o.ReturnTime , 'dd-MMM-yyyy hh:mm tt'),'') AS ReturnTime,
		s.IsFAS,
		(select top 1 [Name] from dbo.fnGetCurrentMealSession(s.OutletId, o.MealSessionDetailId, o.DeliveryDate, s.ClassId)) AS CurrentMealSession
		
		--u.UserName as ProcessedBy
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
	LEFT JOIN Vouchers v WITH (NOLOCK) ON v.Id = p.VoucherId
	--LEFT JOIN MealSessions ms WITH (NOLOCK) ON ms.Id = o.MealSessionDetailId
	--LEFT JOIN [User] u WITH (NOLOCK) ON u.Id = o.CreatedBy
	WHERE 
		CONVERT(DATE, o.DeliveryDate) BETWEEN @ReportDateFrom AND @ReportDateTo
		AND (ISNULL(@Status, '') ='' OR o.Status IN (@Status))
		AND (@IsFAS IS NULL OR s.IsFAS = @IsFAS)
		AND (ISNULL(@Keywords, '') = '' OR 
			s.Name LIKE '%' + @Keywords + '%' OR
			c.Name LIKE '%' + @Keywords + '%' OR 
			p.InvoiceNumber LIKE '%' + @Keywords + '%' OR
			v.Code LIKE '%' + @Keywords + '%')

	SELECT @Total = COUNT(Id) FROM #TempResults 

	
	SELECT @Total AS Total, * FROM #TempResults 
	--ORDER BY StudentName
	ORDER BY --InvoiceNumber DESC
			CASE WHEN @SortBy =  'paymentId' and @SortDirection = 1 THEN PaymentId END DESC,  
			CASE WHEN @SortBy =  'invoiceNumber' and @SortDirection = 1 THEN InvoiceNumber END DESC,
			CASE WHEN @SortBy =  'profileName' and @SortDirection = 1 THEN StudentName END DESC,
			CASE WHEN @SortBy =  'deliveryDate' and @SortDirection = 1 THEN DeliveryDate END DESC,
			CASE WHEN @SortBy =  'className' and @SortDirection = 1 THEN ClassName END DESC,
			CASE WHEN @SortBy =  'transactionTime' and @SortDirection = 1 THEN OrderDate END DESC,
			CASE WHEN @SortBy =  'mealSessionName' and @SortDirection = 1 THEN MealSessionDetailName END DESC,

			CASE WHEN @SortBy =  'paymentId' and @SortDirection = 0 THEN PaymentId END ASC,  
			CASE WHEN @SortBy =  'invoiceNumber' and @SortDirection = 0 THEN InvoiceNumber END ASC,
			CASE WHEN @SortBy =  'profileName' and @SortDirection = 0 THEN StudentName END ASC,
			CASE WHEN @SortBy =  'deliveryDate' and @SortDirection = 0 THEN DeliveryDate END ASC,
			CASE WHEN @SortBy =  'className' and @SortDirection = 0 THEN ClassName END ASC,
			CASE WHEN @SortBy =  'transactionTime' and @SortDirection = 0 THEN OrderDate END ASC,
			CASE WHEN @SortBy =  'mealSessionName' and @SortDirection = 0 THEN MealSessionDetailName END ASC

			--(CASE 
			--		WHEN @SortBy =  'paymentId' and @SortDirection = 'A' THEN PaymentId
			--		WHEN @SortBy =  'invoiceNumber' and @SortDirection = 'A' THEN InvoiceNumber
			--		WHEN @SortBy =  'profileName' and @SortDirection = 'A' THEN StudentName
			--		--ELSE DeliveryDate
			--END) ASC
			--,
			--(CASE 
			--		WHEN @SortBy =  'paymentId' and @SortDirection = 'D' THEN PaymentId
			--		WHEN @SortBy =  'invoiceNumber' and @SortDirection = 'D' THEN InvoiceNumber
			--		WHEN @SortBy =  'profileName' and @SortDirection = 'D' THEN StudentName
			--		--ELSE DeliveryDate
			--END) DESC

		--CASE WHEN @SortDirection = 0 THEN
		--	CASE 
		--		WHEN @SortBy = 'paymentId' THEN PaymentId 
		--		WHEN @SortBy = 'profileName' THEN StudentName 
		--		WHEN @SortBy = 'deliveryDate' THEN DeliveryDate
		--	END
		--END ASC
		--, CASE WHEN @SortDirection = 1 THEN
		--	CASE
		--		WHEN @SortBy = 'paymentId' THEN PaymentId 
		--		WHEN @SortBy = 'profileName' THEN StudentName  
		--		WHEN @SortBy = 'deliveryDate' THEN DeliveryDate
		--	END
		--END DESC
	OFFSET (@Page - 1) * @PageSize ROWS 
	FETCH NEXT @PageSize ROWS ONLY
END