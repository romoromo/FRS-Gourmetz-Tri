using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class EmployeeData : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public int? EmployeeDesignationId { get; set; }
        [ForeignKey("EmployeeDesignationId")]
        public virtual EmployeeDesignation EmployeeDesignation { get; set; }

        public string DisplayName { get; set; }

        public string picture_url { get; set; }
    }
}
