using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SMV.FOMOPay.Model;

namespace SMV.FOMOPay.Model
{
    public class TransException  
    {
        public string content { get; set; }
        public long hiResult { get; set; }
        public string helpDescription { get; set; }
        public string helpLink { get; set; }
        public string message { get; set; }
        public string source { get; set; }
        public string statusCode { get; set; }
        public string ResponseStatus { get; set; }
        public string statusDescription { get; set; }
        public string ResponseAbsoluteURL { get; set; }
        public int URLPort { get; set; }
        public DateTime dateLogged { get; set; }


    }






}