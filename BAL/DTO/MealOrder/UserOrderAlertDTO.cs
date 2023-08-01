using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class UserOrderAlertDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        //public bool IsAbandonedCart1Sent { get; set; }
        //public bool IsAbandonedCart2Sent { get; set; }
        //public bool IsMissedCollectedSent { get; set; }
        public DateTime? AbandonedCart1SentDate { get; set; }
        public DateTime? AbandonedCart2SentDate { get; set; }
        public DateTime? MissedCollectedSentDate { get; set; }
        public DateTime? NoOrderNextWeekSentDate { get; set; }
    }
}
