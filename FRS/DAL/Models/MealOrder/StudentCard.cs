using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class StudentCard : AuditableEntity
    {
        public StudentCard()
        {
        }

        [Key]
        public int Id { get; set; }
        public string CardId { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }

        public DateTime? IssueDate { get; set; }
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}
