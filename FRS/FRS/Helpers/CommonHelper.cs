using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
  
namespace SMV.FOMOPay.CommonHelper
{
    public class HelperMethod
    {
        static public string GetPseudoOrderNumber()
        {
            Random rnd = new Random();
            int num = rnd.Next();

            return num.ToString();
        }

        static public string EncodeTo64(string toEncode)
        {

            byte[] toEncodeAsBytes

                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);

            string returnValue

                  = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }

        public static string BinaryToASCII(string bin)
        {
            string ascii = string.Empty;

            for (int i = 0; i < bin.Length; i += 8)
                {
            ascii += (char)BinaryToDecimal(bin.Substring(i, 8));
            }

        return ascii;
        }

        private static int BinaryToDecimal(string bin)
        {
            int binLength = bin.Length;
            double dec = 0;

            for (int i = 0; i < binLength; ++i)
            {
            dec += ((byte)bin[i] - 48) * Math.Pow(2, ((binLength - i) - 1));
            }

            return (int)dec;
        }

    }

}