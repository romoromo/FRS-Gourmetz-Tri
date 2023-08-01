using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SMV.FOMOPay.Model;

namespace SMV.FOMOPay.Model
{

    public class TransactionOptions 
    {
        public string payerName { get; set; }
        public string payerEmail { get; set; }
        public string payerPhone { get; set; }
        public Address shippingAddress { get; set; }
        public Address billingAddress { get; set; }
        public PaymentItem[] paymentItem { get; set; }

    }


    //public class Atome
    //{
    //    public string payerName { get; set; }
    //    public string payerEmail { get; set; }
    //    public string sku { get; set; }
    //    public string payerPhone { get; set; }
    //    public Address shippingAddress { get; set; }
    //    public Address billingAddress { get; set; }
    //    public PaymentItems[] paymentItems { get; set; }

    //}


    //public class Paypal
    //{
    //    public Address shippingAddress { get; set; }
    //    public PaymentItems paymentItems { get; set; }
    //}

 

}