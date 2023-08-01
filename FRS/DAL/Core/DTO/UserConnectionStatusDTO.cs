using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.DTO
{
    public class UserConnectionStatusDTO
    {
        public int Id { get; set; }
        public string ConnectionID { get; set; }
        public string Identifier { get; set; }
        public string Type { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public bool IsConnected { get; set; }
    }
}
