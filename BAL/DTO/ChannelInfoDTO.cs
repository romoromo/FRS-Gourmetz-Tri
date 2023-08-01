using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class ChannelInfoDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }

        public string Type { get; set; }

        public string Address { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }
    }
}
