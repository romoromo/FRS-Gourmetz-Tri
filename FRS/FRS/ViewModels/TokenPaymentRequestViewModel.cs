using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class TokenPaymentRequestViewModel
    {
 
        ///
        [Key]
        public long PaymentRequestId { get; set; }

        public virtual int UserId { get; set; }

        public virtual int institutionId { get; set; }

        public virtual long discountId { get; set; }

        public virtual long priceId { get; set; }

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
