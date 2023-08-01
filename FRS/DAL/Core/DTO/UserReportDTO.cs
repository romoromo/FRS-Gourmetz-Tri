using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class UserReportDTO
    {
        public string UserName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string UserGroup { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public double InactiveDuration { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string Status { get; set; }
    }
}
