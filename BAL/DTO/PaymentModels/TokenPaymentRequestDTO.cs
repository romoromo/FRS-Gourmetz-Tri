using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SMV.FOMOPay.Model;

namespace SMV.FOMOPay.Model
{

    /// <summary>
    /// NB Need tp initialise with empty string/0 where appropriate.
    /// </summary>
    public class TokenPaymentRequestDTO
    {
        public long PaymentRequestId { get; set; }

        public long userId { get; set; } 
        public long institutionId { get; set; }
        public long discountId { get; set; }
        public long priceId { get; set; }
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