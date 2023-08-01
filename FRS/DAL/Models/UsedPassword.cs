using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class UsedPassword
    {
        public UsedPassword()
        {
            CreatedDate = DateTime.Now;
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string HashPassword { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser AppUser { get; set; }
    }
}