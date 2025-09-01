using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class VoucherDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? OutletProfileId { get; set; }
        public int? InstitutionId { get; set; }
        public int VoucherTypeId { get; set; }
        public string VoucherTypeName { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string DiscountType { get; set; }
        public double DiscountAmount { get; set; }
        public double MinimumBasketPrice { get; set; }
        public int UsageQuantity { get; set; }
        public int UsageQuantityUsed { get; set; }
        public int MaxDistribution { get; set; }
        public bool IsDisplayAllPages { get; set; }
        public int UsedCount{ get; set; }
        public int MaxRedeemCheckout { get; set; }
        public string MaxRedeemCheckoutMessage { get; set; }
        public bool IsMaxRedeemed { get; set;  }
        public List<VoucherMealPeriodDTO> VoucherMealPeriods { get; set; }
        public List<VoucherDishDTO> VoucherDishes { get; set; }
    }

    public class VoucherMealPeriodDTO
    {
        public int VoucherId { get; set; }
        public int MealPeriodId { get; set; }
        public string MealPeriodName { get; set; }
    }

    public class VoucherDishDTO
    {
        public int VoucherId { get; set; }
        public int DishId { get; set; }
        public string DishName { get; set; }
        public string DishCode { get; set; }
    }

    public class VoucherTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
