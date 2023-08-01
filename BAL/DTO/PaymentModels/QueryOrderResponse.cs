using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

 
namespace SMV.FOMOPay.Model
{
    /// <summary>
    /// Used to hold body payload from FOMOPayment notification 
    /// </summary>
    public class QueryOrderResponse
    {
        public string Id { get; set; }          //OrderID

        public string SubMid { get; set; }      //Sub-Merchant ID
        public string OrderNo { get; set; }     // Order number not same as OrderID
        public string Mode { get; set; }        //Order mode
        public string Subject { get; set; }      //Short order description
        public string Description { get; set; } //Order description

        public string Amount { get; set; }      //Transaction amount
 
        public string CurrencyCode { get; set; }    //Transaction currency

        public string Status { get; set; }      //Transaction status
        public string CreatedAt { get; set; }   //Time of order creation, Unix time in seconds
        public string NotifyUrl { get; set; }   //URL to notify order result update
        public string ReturnUrl { get; set; }   //URL to return to after payment

        public string BackUrl { get; set; }     //URL to return to when user cancel the payment

        public string PrimaryTransactionId { get; set; }  //Default transaction linked to current order
        public string Url { get; set; } //Redirection URL for FOMOPay payment page



    }
 
 
}