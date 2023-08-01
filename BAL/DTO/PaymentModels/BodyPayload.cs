using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

 
namespace SMV.FOMOPay.Model
{
    /// <summary>
    /// Used to hold body payload from FOMOPayment notification 
    /// </summary>
    public class BodyPayload
    {
        public string OrderId { get; set; } 

        public string OrderNo { get; set; }
        public string TransactionId { get; set; }
        public string TransactionNo { get; set; }

    }
 
 
}