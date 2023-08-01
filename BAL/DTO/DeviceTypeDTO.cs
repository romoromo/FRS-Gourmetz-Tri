using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class DeviceTypeDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }
    }
}
