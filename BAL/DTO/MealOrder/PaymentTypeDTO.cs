using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class PaymentTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSystem { get; set; }
    }
}
