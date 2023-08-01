using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class ExternalAppLoginLog
    {
        [Key]
        public int Id { get; set; }
        public string AppId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime EventDateTime { get; set; }
    }
}
