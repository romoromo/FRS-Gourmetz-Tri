using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class EmailConfirm : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
        public DateTime? Date { get; set; }
    }
}
