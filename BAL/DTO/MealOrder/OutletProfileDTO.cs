using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class OutletProfileDTO
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public int? CatererId { get; set; }
        public List<OutletDTO> Outlets { get; set; }
    }
}
