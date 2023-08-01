using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMV.FOMOPay.Model
{
    public class OrderResponse  
    {
        public string id { get; set; } // fomo payment trans id
        public string subMid { get; set; }
        public string orderNo { get; set; }
        public string subject { get; set; }
        public string amount { get; set; }
        public string currencyCode { get; set; }
        public string status { get; set; }
        public int createdAt { get; set; }
        public string notifyUrl { get; set; }
        public string returnUrl { get; set; }
        public string backUrl { get; set; }
        public string primaryTransactionId { get; set; }
        public string url { get; set; }

    }
}