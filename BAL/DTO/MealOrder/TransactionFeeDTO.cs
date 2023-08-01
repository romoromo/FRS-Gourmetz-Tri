using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class TransactionFeeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }
        public double Amount { get; set; }
        public bool IsFixed { get; set; }
        public List<TransactionFeeDetailDTO> TransactionFeeDetails { get; set; }
    }

    public class TransactionFeeDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public int TransactionFeeId { get; set; }
    }
}
