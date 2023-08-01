using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class AuthenticationLogDTO
    {
        public string InstitutionCode { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
