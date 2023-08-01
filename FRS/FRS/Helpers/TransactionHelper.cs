using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
  
namespace SMV.FOMOPay.TransactionHelper
{
    class TransactionHelperMethod
    {
        static public string Encode()
        {
            string returnValue = "";

            return returnValue;
        }

        static public String[] GetSourceOfFunds(string paymentSelected)
        {
            String[] sourceOfFunds = new String[1];

            if (paymentSelected.Length > 0)
            {
                sourceOfFunds[0]=paymentSelected;
            }

            return sourceOfFunds;

        }



    }





}