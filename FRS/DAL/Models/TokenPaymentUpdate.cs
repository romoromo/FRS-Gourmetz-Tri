using DAL.Core.Audit.Attributes;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
 

namespace DAL.Models
{
    public class TokenPaymentUpdate : AuditableEntity
    {

        public string PaymentTransactionId { get; set; }    // from fomopay

        public string StatusCode { get; set; }

      //  public DateTime UpdatedDate { get; set; }
 
    }
}
