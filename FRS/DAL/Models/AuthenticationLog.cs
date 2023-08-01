using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class AuthenticationLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string InstitutionCode { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
