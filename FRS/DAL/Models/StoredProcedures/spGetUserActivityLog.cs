using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.StoredProcedures
{
    [NotMapped]
    public class spGetUserActivityLogHeader
    {
        [Key]
        public string GroupId { get; set; }
        public string Username { get; set; }
        public string Remarks { get; set; }
        public int Total { get; set; }
    }

    [NotMapped]
    public class spGetUserActivityLogDetail
    {
        [Key]
        public int Id { get; set; }
        public string GroupId { get; set; }
        public string Username { get; set; }
        public string ActionName { get; set; }
        public int AuditLogId { get; set; }
        public DateTime EventDateTime { get; set; }
        public string RecordId { get; set; }
        public string LogType { get; set; }
        public string Remarks { get; set; }
        public string PropertyName { get; set; }
        public string OldVal { get; set; }
        public string NewVal { get; set; }
        public string DetailRemarks { get; set; }
    }
}
