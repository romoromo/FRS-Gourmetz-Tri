using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

 
namespace SMV.FOMOPay.Model
{
    /// <summary>
    /// Used to hold info required to 
    /// update token order status.
    /// </summary>
    public class UpdateTokenOrderDTO
    {
        public string PaymentTransactionId { get; set; }
        public string StatusCode { get; set; }

      //  public DateTime UpdatedDate { get; set; }
    }
 
}