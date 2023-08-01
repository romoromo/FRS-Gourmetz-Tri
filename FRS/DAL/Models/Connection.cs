using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class Connection : AuditableEntity
    {
        public Connection()
        {
        }

        [Key]
        public int Id { get; set; }
        public string ConnectionID { get; set; }
        public string Identifier { get; set; }
        public string Type { get; set; }
    }
}
