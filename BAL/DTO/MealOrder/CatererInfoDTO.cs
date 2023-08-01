using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class CatererInfoDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public List<CatererOutletDTO> CatererOutlets { get; set; }
    }

    public class CatererOutletDTO
    {
        public int CatererInfoId { get; set; }
        public int OutletId { get; set; }
        public int? OutletProfileId { get; set; }
        public bool IsRsp { get; set; }
        public string Status { get; set; }
        public OutletDTO Outlet { get; set; }
        public CatererInfoDTO Caterer { get; set; }
        public OutletProfileDTO OutletProfile { get; set; }
    }
}
