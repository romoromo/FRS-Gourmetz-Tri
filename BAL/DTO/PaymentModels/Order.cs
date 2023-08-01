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
    public class Order
    {
        public string mode { get; set; } 
        public string orderNo { get; set; }
        public string tokenOrderId { get; set; }
        public string subject { get; set; }
        public string description { get; set; }
        public string amount { get; set; }
        public string currencyCode { get; set; }
        public string notifyUrl { get; set; }
        public string returnUrl { get; set; }
        public string backUrl { get; set; }
        public string[] sourceOfFunds { get; set; }

        // NB Passing null transactionoption will
        // result in a response BadRequest error. If not used 
        // just initialise with empty strings etc.
        public TransactionOptions transactionOptions { get; set; }

    }

 




}