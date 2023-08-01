using System;

namespace SMV.FOMOPay.Model
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string RequestResult { get; set; }

    }
}
