using DAL.Core.Audit.Attributes;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class TokenPaymentRequest : AuditableEntity
    {
        [Key]
        public long PaymentRequestId { get; set; }

        [ForeignKey("UserId")]
        public virtual int UserId { get; set; }

        [ForeignKey("institutionId")]
        public virtual int institutionId { get; set; }

        [ForeignKey("discountId")]
        public virtual long discountId { get; set; }

        [ForeignKey("priceId")]
        public virtual long priceId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public double amount { get; set; }

        public string currencyCode { get; set; }
        public string mode { get; set; }
        public string subject { get; set; }
        public string notifyUrl { get; set; }

        public string returnUrl { get; set; }
        public string backUrl { get; set; }
        public string orderNo { get; set; }
        public long tokenOrderId { get; set; }

        public string sourceOfFunds { get; set; }
        public DateTime transactionDate { get; set; }
 
    }
}
