using Sieve.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Filters
{
    public class BaseFilter : SieveModel
    {
        //public int? InstitutionId { get; set; }

        public bool IsActive { get; set; }
    }

    public class DashboardFilter
    {
        public BaseFilter Filter { get; set; }
        public string Status { get; set; }

    }

    public class RequestFilter : BaseFilter
    {
        public bool isrequest { get; set; }
        public int? studentId { get; set; }
    }

    public class ServiceContractFilter
    {
        public BaseFilter Filter { get; set; }
        public bool IncludeAssets { get; set; }

    }

    public class ClassRosterFilter : BaseFilter
    {
        public int CatererId { get; set; }
        public int OutletId { get; set; }
        public int ClassRosterId { get; set; }
        public int OutletProfileId { get; set; }
    }

    public class OrderCancellationFilter : BaseFilter
    {
        public string InvoiceNumber { get; set; }
        public string OrderNumber { get; set; }
        public int? OutletId { get; set; }
        public string Keyword { get; set; }
    }

    public class StudentOrderFilter : BaseFilter
    {
        public string InvoiceNumber { get; set; }
        public string OrderNumber { get; set; }
        public int? OutletId { get; set; }
        public int StudentId { get; set; }
        public string Keyword { get; set; }
    }

    public class SalesOrderReportFilter : BaseFilter
    {
        public DateTime ReportDateFrom { get; set; }
        public DateTime ReportDateTo { get; set; }
        public string Status { get; set; }
        public string OrderType { get; set; }
        public bool? IsFas { get; set; }
        public string Keyword { get; set; }
        public int ReportType { get; set; }
        public List<int> StudentGroupIds { get; set; }
        public List<string> CollectionStatuses { get; set; }
        public string OutletId { get; set; }
    }

    public class VoucherUtilisationReportFilter : BaseFilter
    {
        public DateTime ReportDateFrom { get; set; }
        public DateTime ReportDateTo { get; set; }
        public string Status { get; set; }
        public string Keyword { get; set; }
    }

    //public class OrderCollectionReportFilter : BaseFilter
    //{
    //    public DateTime ReportDateFrom { get; set; }
    //    public DateTime ReportDateTo { get; set; }
    //    public string Status { get; set; }
    //    public bool? IsFas { get; set; }
    //    public string Keyword { get; set; }
    //    public int ReportType { get; set; }
    //    public int StudentGroupId { get; set; }
    //}

    public class RoleReportFilter : BaseFilter
    {
        public List<string> Permissions { get; set; }
    }

    public class UserActivityLogReportFilter : BaseFilter
    {
        public DateTime ReportDateFrom { get; set; }
        public DateTime ReportDateTo { get; set; }
        public string Keyword { get; set; }
        public string ReportType { get; set; }
    }

    public class MealAllocationAdditionalDishFilter
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int OutletId { get; set; }
    }

    public class BentoAssetsFilter : BaseFilter
    {
        public int CatererInfoId { get; set; }
    }

    public class CartonAssetsFilter : BaseFilter
    {
        public int CatererInfoId { get; set; }
    }

    public class CatererAsserFilter : BaseFilter
    {
        public int catererInfoId { get; set; }
    }

    public class AssetComponentFilter : BaseFilter
    {
        public int catererInfoId { get; set; }
    }
}
