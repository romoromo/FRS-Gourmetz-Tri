using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class RoleReportDTO
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public List<string> Permissions { get; set; }
        public string PermissionNames
        {
            get
            {
                return string.Join(",", Permissions);
            }
        }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }
}
