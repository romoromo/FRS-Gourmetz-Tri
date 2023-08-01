using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core
{
    public class BaseOperationResponse
    {
        public BaseOperationResponse()
        {

        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
