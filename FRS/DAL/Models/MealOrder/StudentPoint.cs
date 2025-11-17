using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models.MealOrder
{
    public class StudentPoint : AuditableEntity
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public double Balance { get; set; }
        public string Type { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}
