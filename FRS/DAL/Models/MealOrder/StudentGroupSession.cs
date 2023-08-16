using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class StudentGroupSession : AuditableEntity
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey("StudentGroup")]
        public int StudentGroupId { get; set; }
        public virtual StudentGroup StudentGroup { get; set; }

        [ForeignKey("MealSession")]
        public int MealSessionId { get; set; }
        public virtual MealSession MealSession { get; set; }
    }
}
