using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class UserCardId : AuditableEntity
    {
        public UserCardId()
        {
        }

        [Key]
        public int Id { get; set; }
        public string CardId { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
