using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO
{
    public class ConnectionDTO
    {
        public int Id { get; set; }
        public string ConnectionID { get; set; }
        public string Identifier { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
    }
}
