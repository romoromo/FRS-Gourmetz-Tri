using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class MenuGroupClass : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Class")]
        public int ClassId { get; set; }
        public virtual Class Class { get; set; }

        [ForeignKey("MenuGroup")]
        public int MenuGroupId { get; set; }
        public virtual MenuGroup MenuGroup { get; set; }
    }
}
