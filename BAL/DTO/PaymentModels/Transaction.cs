using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SMV.FOMOPay.Model;

namespace SMV.FOMOPay.Model 
{


    /// <summary>
    /// Using view model approach to combine 2 models for use in our UI.
    /// Alternative is to use dynamic model via ExpandoObject().
    /// </summary>
    public class Transaction
    {
        public Order customerOrder { get; set; }
        public IEnumerable<PaymentDiscount> paymentDiscounts { get; set; }

    }

 
}