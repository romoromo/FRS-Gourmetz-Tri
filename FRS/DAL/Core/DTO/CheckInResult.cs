using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class CheckInResult
    {
        public bool IsUserValid { get; set; }
        public string ReservationStatus { get; set; }
        public string Message { get; set; }
    }

    public class ExtendResult
    {
        public bool IsExtendValid { get; set; }
        public string Message { get; set; }
    }
}
