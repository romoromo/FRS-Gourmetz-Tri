using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 
 
namespace SMV.FOMOPay.MessageHelper
{

    static class TransactionFailure
    {
      //
      // Defines four public static readonly fields on a static class.
      //
      public static readonly String  PaymentError = "There was a system error when trying to process your payment. Please contact a member of staff for assistance.";

        //
      // public static readonly String PaymentBelowMinOrder = "Your order is below min order value of ";
    }
 

    static class TransactionSuccess
    {
        //
        // Defines four public static readonly fields on a static class.
        //
        public static readonly String PaymentSuccess = "Your payment was successfully processed.";
    }


}