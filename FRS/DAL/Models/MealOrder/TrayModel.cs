using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models.MealOrder
{
    public class TrayModel : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int DispenserId { get; set; }

        [ForeignKey(nameof(DispenserId))]
        public virtual DispenserOutlet DispenserOutlet { get; set; }

        public int PLCId { get; set; }

        [ForeignKey(nameof(PLCId))]
        public virtual PLCModel PLC { get; set; }

        public int MotorOutputNumber { get; set; }
        public int LEDOutputNumber { get; set; }
    }
}
