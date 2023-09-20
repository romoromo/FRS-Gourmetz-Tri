using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class AuditLogDTO
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public DateTime EventDateTime { get; set; }

        public string LogType { get; set; }

        public string TableName { get; set; }

        public string RecordId { get; set; }

        public string GroupId { get; set; }

        public string ActionName { get; set; }

        public string Remarks { get; set; }
    }

    public class ExternalAppLoginLogDTO
    {
        public int Id { get; set; }
        public string AppId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime EventDateTime { get; set; }
    }

    public class UserActivityLogDTO
    {
        public string GroupId { get; set; }
        public string Username { get; set; }
        public string Remarks { get; set; }
        public DateTime EventDateTime { get; set; }
        public int Total { get; set; }
        public List<UserActivityLogDetailDTO> Details { get; set; }
    }

    public class UserActivityLogDetailDTO
    {
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
