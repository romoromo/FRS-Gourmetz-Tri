using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class DispenserOutletDTO
    {
        public int Id { get; set; }

        public string DispenserCode { get; set; }
        public string CounterName { get; set; }

        public string Password { get; set; }

        public string LocationCode { get; set; }

        public string Color { get; set; }

        public string PLCIPAddress { get; set; }

        public string PLCPort { get; set; }

        public string PLCToken { get; set; }

        public int PLCApiVer { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public int? OutletId { get; set; }
        public string OutletName { get; set; }

        public List<TrayDTO> Trays { get; set; } = [];
    }
}
