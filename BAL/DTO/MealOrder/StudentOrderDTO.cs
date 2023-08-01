using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class StudentOrderDTO
    {
        public StudentOrderDTO()
        {
            TokenOrders = new List<TokenOrderDTO>();
        }

        public StudentDTO Student { get; set; }
        public IEnumerable<TokenOrderDTO> TokenOrders { get; set; }
    }
}
