using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class UserOrderAlert : AuditableEntity
    {
        public UserOrderAlert()
        {
        }

        [Key]
        public int Id { get; set; }
        //public int TokenOrderId { get; set; }
        public int UserId { get; set; }
        //public bool IsAbandonedCart1Sent { get; set; }
        //public bool IsAbandonedCart2Sent { get; set; }
        //public bool IsNoCollectionSent { get; set; }

        public DateTime? AbandonedCart1SentDate { get; set; }
        public DateTime? AbandonedCart2SentDate { get; set; }
        public DateTime? MissedCollectedSentDate { get; set; }
        public DateTime? NoOrderNextWeekSentDate { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        //[ForeignKey("TokenOrderId")]
        //public virtual TokenOrder TokenOrder { get; set; }
    }
}
