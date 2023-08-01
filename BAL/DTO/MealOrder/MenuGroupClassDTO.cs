using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MenuGroupClassDTO
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int MenuGroupId { get; set; }
        public string MenuGroupName { get; set; }
        public string ClassName { get; set; }
    }
}
