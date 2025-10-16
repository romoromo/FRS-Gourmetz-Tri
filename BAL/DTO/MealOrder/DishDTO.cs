using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class DishDTO
    {
        public DishDTO()
        {
            this.SubDishes = new List<DishDetailDTO>();
        }

        public int Id { get; set; }

        public string Code { get; set; }
        public string Label { get; set; }
        public int DishTypeId { get; set; }
        public double RRPrice { get; set; }
        public double Cost { get; set; }
        public bool IsEnabled { get; set; }

        public int? FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public int? ProductionPictureId { get; set; }
        public string ProductionPictureName { get; set; }
        public string ProductionPicturePath { get; set; }

        public int? ParentDishId { get; set; }
        public string ParentDishLabel { get; set; }
        public int? BentoBoxTypeId { get; set; }
        public string BentoBoxTypeCode { get; set; }
        public bool BentoBoxTypeRFID { get; set; }
        public string BentoBoxTypeDetails { get; set; }
        public string BentoBoxTypePicture { get; set; }
        public string ExtraNotes { get; set; }
        public float CookedWeight { get; set; }
        public float Protein { get; set; }
        public float Sugar { get; set; }
        public float TotalFat { get; set; }
        public float TotalCarb { get; set; }
        public float Calories { get; set; }

        public string DishTypeName { get; set; }
        public string DishTypeOrder { get; set; }
        public string DishPeriodNames { get { return DishPeriods != null ? string.Join(",", DishPeriods.Select(e => e.PeriodName)) : string.Empty; } }

        public int? CuisineId { get; set; }
        public string CuisineName { get; set; }

        public string CuisineIconFileName { get; set; }
        public string CuisineIconFilePath { get; set; }
        public string CuisineLicenseCode { get; set; }



        public string SapCode { get; set; }
        public int? StoreInfoId { get; set; }
        public string StoreInfoCode { get; set; }
        public string StoreInfoName { get; set; }

        public int? CatererId { get; set; }
        public string CatererName { get; set; }

        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }

        public string UpdatedByName { get; set; }
        public DateTime UpdatedDate { get; set; }

        public string ProductionDescription { get; set; }
        public List<DishPeriodDTO> DishPeriods { get; set; }
        public List<DishRestrictionDTO> Restrictions { get; set; }
        public List<DishDetailDTO> SubDishes { get; set; }
        public List<DishComponentDTO> DishComponents { get; set; }
    }

    public class DishComponentDTO
    {
        public int DishId { get; set; }
        public int Id { get; set; }
        public string Category { get; set; }
        public string Component { get; set; }
        public float Weight { get; set; }
        public int Quantity { get; set; }
        public string SapProductCode { get; set; }
    }

    public class DishRestrictionDTO
    {
        public int DishId { get; set; }
        public int RestrictionId { get; set; }
        public string RestrictionCode { get; set; }
        public string RestrictionLabel { get; set; }
    }

    public class DishDetailDTO
    {
        public int DishId { get; set; }
        public int ParentId { get; set; }
        public DishDTO Dish { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }
        public float CookedWeight { get; set; }
    }

    public class DishSimple
    {
        public int Id { get; set; }

        public string Code { get; set; }
        public string Label { get; set; }
    }
}
