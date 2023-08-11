using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class OutletTermDTO
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public int? OutletId { get; set; }
    }
}
