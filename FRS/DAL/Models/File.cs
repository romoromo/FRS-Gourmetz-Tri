using DAL.Models.MealOrder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class File : AuditableEntity
    {
        public File()
        {
            this.Facilities = new HashSet<Facility>();
            this.Locations = new HashSet<Location>();
            this.Users = new HashSet<ApplicationUser>();
        }

        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Facility> Facilities { get; set; } 
        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Dish> Dishes { get; set; }
        public virtual ICollection<Dish> ProductionDishes { get; set; }
    }
}
