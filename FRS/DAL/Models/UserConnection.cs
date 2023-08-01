using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class UserConnection : AuditableEntity
    {
        public UserConnection()
        {
        }

        [Key]
        public int Id { get; set; }
        public string ConnectionID { get; set; }
        public string Identifier { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
