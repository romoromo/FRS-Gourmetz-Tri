using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class UpDownTimeLogDTO
    {
        public string DeviceIdentifier { get; set; }

        public bool IsUp { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
