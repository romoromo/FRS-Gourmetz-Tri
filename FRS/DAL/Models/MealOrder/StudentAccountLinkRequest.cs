using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models.MealOrder
{
    public class StudentAccountLinkRequest : AuditableEntity
    {
        public StudentAccountLinkRequest()
        {
        }

        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public bool EmailSent { get; set; }
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}
