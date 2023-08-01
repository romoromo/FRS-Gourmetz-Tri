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
}
