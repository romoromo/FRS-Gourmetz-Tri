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
    public class TokenPaymentResponseDTO
    {
        public long PaymentResponseId { get; set; }
 
        public long PaymentRequestId { get; set; }
 
        public int UserId { get; set; }
 
        public int InstitutionId { get; set; }
 
        public  int TokenOrderId { get; set; }     // id for the token being paid

        public Boolean IsSuccessful { get; set; }
        public Boolean IsCancelled { get; set; }
        public Boolean IsConfirmedPaid { get; set; }
        public string StatusCode { get; set; }
        public string ResponseStatus { get; set; }
        public string Mode { get; set; }

        public string PaymentTransactionId { get; set; }    // from fomopay
        public string OrderNo { get; set; }                 // our unique generate id for each request submitted to FOMOPay

        public string Subject { get; set; }
        public double Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string PaymentUrl { get; set; }
        public DateTime PaymentDate { get; set; }

    }
 

}