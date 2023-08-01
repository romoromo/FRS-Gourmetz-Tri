using DAL.Models.MealOrder;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class TokensOrderHistoryDTO
    {

        public DateTime TransactionTime { get; set; }

        public int SchoolId { get; set; }

        public string OrderProfile { get; set; }

        public int? ProfileId { get; set; }

        public DateTime DeliveryDate { get; set; }

        public int? PeriodId { get; set; }

        public int? PaymentId { get; set; }

        public float TotalAmount { get; set; }

        public DateTime PaymentTime { get; set; }

        public float TotalPayment { get; set; }

        public string Status { get; set; }

        public string Remarks { get; set; }

        public int? StudentGroupId { get; set; }
        public List<TokenOrderedDTO> Tokens { get; set; }

        public int? MealSessionDetailId { get; set; }
        public string MealSessionName { get; set; }
        public string MealSessionDetailName { get; set; }
        public string ClassName { get; set; }

        public int? MealPeriodId { get; set; }

        public DateTime MealSessionStartDate { get; set; }

        public int? StoreId { get; set; }

        public string CancelRequestStatus { get; set; }
        public string ProfileName { get; set; }
        public string PeriodName { get; set; }
        public string PaymentTypeName { get; set; }
        public int? CreatedBy { get; set; }
        public string ProcessedBy { get; set; }
        public string MealDescription { get; set; }
        public string MealOption { get; set; }
        public string PaymentStatus { get; set; }

        public int? TokenOrderId { get; set; }

        public int? Qty { get; set; }

        public int? DishId { get; set; }

        public DateTime UpdatedDate { get; set; }
        public string DishLabel { get; set; }
    }
}
