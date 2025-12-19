using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class StudentWalletTransactionDetail : AuditableEntity
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public double Amount { get; set; }
        public double AmountRefunded { get; set; }
        public string Type { get; set; }
        [ForeignKey("TransactionId")]
        public virtual StudentWalletTransaction Transaction { get; set; }
    }
}
