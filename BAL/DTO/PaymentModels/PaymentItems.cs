using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMV.FOMOPay.Model
{
    public class PaymentItem 
    {
        public string label { get; set; }
        public Amount amount { get; set; }
        public string sku { get; set; }
        public int quantity { get; set; }
    }
 
    public class Amount
    {
        public string currency { get; set; }
        public string value { get; set; }
    }
 
}