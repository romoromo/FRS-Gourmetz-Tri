using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class StudentGroupMealPlanDTO
    {
        public int Id { get; set; }

        public string Label { get; set; }
        public float Price { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int? MealTypeId { get; set; }

        public int StudentGroupId { get; set; }
        public StudentGroupDTO StudentGroup { get; set; }

        public int MealSessionId { get; set; }

        public int DishId { get; set; }
        public DishDTO Dish { get; set; }

        public int MealSessionDetailId { get; set; }
        public MealSessionDetailDTO MealSessionDetail { get; set; }

        public int? StoreId { get; set; }
        public StoreInfoDTO Store { get; set; }
    }

    public class GroupedStudentGroupMealPlanDTO
    {
        public int StudentGroupId { get; set; }
        public string StudentGroupName { get; set; }
        public List<StudentGroupMealPlanDTO> Meals { get; set; }
    }
}
