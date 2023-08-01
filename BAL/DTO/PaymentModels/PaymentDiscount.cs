using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMV.FOMOPay.Model
{
    public class PaymentDiscount
    {
        public string payment { get; set; } 
        public string discount { get; set; }
       // public Boolean IsEnabled { get; set; }

        public string hideCSS { get; set; }
        public Boolean isVisible { get; set; }

        public Boolean isDefault { get; set; }
 

    }



}