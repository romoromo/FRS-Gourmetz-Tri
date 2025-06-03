using Sieve.Attributes;

namespace DAL.Core.DTO
{
    public class DishLiteDTO
    {
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = false)]
        public int? CatererId { get; set; }
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
    }
}
