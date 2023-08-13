using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MealPlanOrderDTO 
    {
        
        public int Id { get; set; }

        public DateTime TransactionTime { get; set; }

        
        public int? ProfileId { get; set; }


        public decimal TotalAmount { get; set; }

        public int? StoreId { get; set; }
      

        public string Status { get; set; }

        public int? StudentGroupId { get; set; }

        public int? PaymentId { get; set; }
        public string StudentGroupCode { get; set; }
    }
}
