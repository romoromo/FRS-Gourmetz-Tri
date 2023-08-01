using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMV.FOMOPay.Model
{

    /// <summary>
    /// Payment  record of successful submission of payment info but before theshowing the final 
    /// payment page.
    /// </summary>
    public class FomoPaymentResponse
    {
        public string id { get; set; } // fomo payment trans id 
        public string orderNo { get; set; }
        public string tokenOrderId { get; set; }
        public string mode { get; set; }
        public string subject { get; set; }
        public string description { get; set; }
        public string returnUrl { get; set; }
        public string backUrl { get; set; }
        public string notifyUrl { get; set; }
        public string currencyCode { get; set; }
        public string amount { get; set; }
        public int createdAt { get; set; }

        public string status { get; set; }

        public string url { get; set; }
 
    }
}