using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class RouteDTO
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public string Details { get; set; }

        public DateTime Pickup { get; set; }

        public string Color { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public List<RouteNodeDTO> Nodes { get; set; }
    }

    public class RouteNodeDTO
    {
        public int RouteId { get; set; }
        public int StoreId { get; set; }
        public int order { get; set; }
        public DateTime interval { get; set; }
    }
}
