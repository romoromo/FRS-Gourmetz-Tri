using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

 
namespace SMV.FOMOPay.Model
{
    public class Address
    {
        public string city { get; set; } 
        public string country { get; set; }
        public string dependentLocality { get; set; }
        public string organization { get; set; }
        public string phone { get; set; }
        public string postalCode { get; set; }
        public string recipient { get; set; }
        public string region { get; set; }
        public string[] addressLine { get; set; }
    }
 
 
}