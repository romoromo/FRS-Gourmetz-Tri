using DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class OutletSimpleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public int? OutletProfileId { get; set; }
        public string OutletProfileName { get; set; }
        public int DaysToFreezeOrdering { get; set; }
        public int? CreatedBy { get; set; }

        public string Passcode { get; set; }
    }

    public class OutletDTO : OutletSimpleDTO
    {
        public List<CatererOutletDTO> CatererOutlets { get; set; }
        public List<MealSessionDTO> MealSessions { get; set; }
        public List<StoreInfoDTO> Stores { get; set; }
    }
}
