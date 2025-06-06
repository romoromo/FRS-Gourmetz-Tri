using System.Collections.Generic;

namespace DAL.Core.DTO
{

    public class DishImportInputDTO
    {
        public string Label { get; set; }
        public string ProductionDescription { get; set; }
        public string ExtraNote { get; set; }
        public double RPP { get; set; }
        public double Cost { get; set; }
        public string DishTypeName { get; set; }
        public string BentoBoxTypeName { get; set; }
        public string CuisineName { get; set; }
        public string KicthenName { get; set; }
        public string SapCode { get; set; }
        public float Protein { get; set; }
        public float Sugar { get; set; }
        public float TotalFat { get; set; }
        public float TotalCarb { get; set; }
        public float Calories { get; set; }
        public List<string> Restrictions { get; set; } = new List<string>();
        public bool IsEnabled { get; set; }

    }

    public class DishImportDTO
    {
        public bool IsSuccess { get; set; }
        public List<DishImportMessageDTO> Messages { get; set; } = new List<DishImportMessageDTO>();
    }

    public class DishImportMessageDTO
    {
        public int RowNumber { get; set; }
        public string Message { get; set; }
    }

    public class ValidatedDishInputDTO
    {
        public DishImportInputDTO OriginalData { get; set; }
        public int? DishTypeId { get; set; }
        public int? BentoBoxTypeId { get; set; }
        public int? CuisineId { get; set; }
        public int? StoreId { get; set; }
        public List<int> RestrictionsIds = new List<int>();
    }
}
