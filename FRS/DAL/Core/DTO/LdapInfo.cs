using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class LdapInfo
    {
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public long? DepartmentId { get; set; }
        public long? DesignationId { get; set; }
        public string Email { get; set; }
        public string OrgUnit { get; set; }
        public string DesignationName { get; set; }
        public string Adid { get; set; }
        public string Office { get; set; }
        public string TelNo { get; set; }
        public string HomeNo { get; set; }
        public string MobileNo { get; set; }
        public string Department { get; set; }
    }
}
