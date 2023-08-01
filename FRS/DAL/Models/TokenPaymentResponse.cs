using DAL.Core.Audit.Attributes;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class TokenPaymentResponse : AuditableEntity
    {
  
        [Key]
        public long PaymentResponseId { get; set; }

        [ForeignKey("PaymentRequestId")]
        public virtual long PaymentRequestId { get; set; }

        [ForeignKey("UserId")]
        public virtual int UserId { get; set; }

        [ForeignKey("institutionId")]
        public virtual int InstitutionId { get; set; }

        [ForeignKey("tokenOrderId")]
        public virtual int TokenOrderId { get; set; }     // id for the token being paid

        public Boolean IsSuccessful { get; set; }
        public Boolean IsCancelled { get; set; }
        public Boolean IsConfirmedPaid { get; set; }
        public string StatusCode { get; set; }
        public string ResponseStatus { get; set; }
        public string Mode { get; set; }

        public string PaymentTransactionId { get; set; }    // from fomopay
        public string OrderNo { get; set; }                 // our unique generate id for each request submitted to FOMOPay
 
        public string Subject { get; set; }
        public double Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string PaymentUrl { get; set; }
 
        public DateTime PaymentDate { get; set; }
 

    }
}
