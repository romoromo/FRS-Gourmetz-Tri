using Sieve.Attributes;
using System.Collections.Generic;

namespace DAL.Core.DTO
{
    public class DishLiteDTO
    {
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = false)]
        public int? CatererId { get; set; }
        public string CatererName { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Code { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Label { get; set; }
        public string ProductionDescription { get; set; }
        public string DishTypeName { get; set; }
        public string CuisineName { get; set; }
        public string BentoBoxTypeCode { get; set; }
        public double RRPrice { get; set; }
        public double Cost { get; set; }
        public bool IsEnabled { get; set; }
        [Sieve(CanFilter = true, CanSort = false)]
        public bool IsActive { get; set; }
        public string ExtraNote { get; set; }
        public string KicthenName { get; set; }
        public string SapCode { get; set; }
        public float Protein { get; set; }
        public float Sugar { get; set; }
        public float TotalFat { get; set; }
        public float TotalCarb { get; set; }
        public float Calories { get; set; }
        public List<string> Restrictions { get; set; } = new List<string>();
    }
}
