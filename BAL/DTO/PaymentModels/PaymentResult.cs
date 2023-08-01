
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMV.FOMOPay.Model
{
    public class PaymentResult 
    {
        public Boolean isSuccessful { get; set; }
        public Boolean isCancelled { get; set; }
        public string message { get; set; }
        public string statusCode { get; set; }
        public string responseStatus { get; set; } // eg Error
        public string responseURI { get; set; }
        public string server { get; set; }
        public DateTime paymentTime { get; set; }

        public FomoPaymentResponse fomoPaymentResponse { get; set; }

        public TransException transException { get; set; }

        // Set default values
        public PaymentResult()
        {
            this.isCancelled = false;
            this.isSuccessful = false;
        }
 

    }
 
}