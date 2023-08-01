using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class WaiverDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public int TransactionFeeId { get; set; }
        public string Label { get; set; }
        public string TransactionFeePaymentType { get; set; }
        public string TransactionFeeLabel { get; set; }
    }
}
