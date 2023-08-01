using Sieve.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Filters
{
    public class BaseFilter : SieveModel
    {
        //public int? InstitutionId { get; set; }

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

    public class ClassRosterFilter: BaseFilter
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
    }

    public class SalesOrderReportFilter : BaseFilter
    {
        public DateTime ReportDateFrom { get; set; }
        public DateTime ReportDateTo { get; set; }
        public string Status { get; set; }
        public bool? IsFas { get; set; }
        public string Keyword { get; set; }
        public int ReportType { get; set; }
    }

    public class RoleReportFilter : BaseFilter
    {
        public List<string> Permissions { get; set; }
    }
}
