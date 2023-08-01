using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class ApplicationSettingDTO
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public int? InstitutionId { get; set; }
    }
}
