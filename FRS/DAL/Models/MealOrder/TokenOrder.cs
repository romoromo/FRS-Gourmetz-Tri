using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class TokenOrder : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime TransactionTime { get; set; }

        public int SchoolId { get; set; }

        public string OrderProfile { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? ProfileId { get; set; }

        public DateTime DeliveryDate { get; set; }

        public int? PeriodId { get; set; }

        [ForeignKey("PeriodId")]
        public virtual MealPeriod Period { get; set; }

        public int? PaymentId { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }

        public float TotalAmount { get; set; }

        public DateTime PaymentTime { get; set; }

        public float TotalPayment { get; set; }

        public string CancelRequestStatus { get; set; }

        public bool IsFAS { get; set; }
        public bool IsMealPlan { get; set; }
        public bool IsStudentGroupOrder { get; set; }

        public DateTime? CollectionTime { get; set; }

        public string BentoCode { get; set; }

        public DateTime? ReturnTime { get; set; }

        public int? CancelledById { get; set; }
        public DateTime? CancelledOn { get; set; }
        public string CancellationReason { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Status { get; set; }

        public string Remarks { get; set; }

        [ForeignKey("ProfileId")]
        public virtual Student Student { get; set; }
        public virtual ICollection<TokenOrdered> Tokens { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? StudentGroupId { get; set; }

        [ForeignKey("StudentGroupId")]
        public virtual StudentGroup StudentGroup { get; set; }

        public int? MealSessionDetailId { get; set; }

        [ForeignKey("MealSessionDetailId")]
        public virtual MealSessionDetail Session { get; set; }

        public int? StoreId{ get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreInfo Store { get; set; }

        //public virtual ICollection<UserOrderAlert> UserOrderAlerts { get; set; }

        public virtual ICollection<TokensOrderHistory> TokensOrderHistorys { get; set; }

    }
}
