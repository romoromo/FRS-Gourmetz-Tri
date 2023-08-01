using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public bool IsRead { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
    }

    public class NotificationEventViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Template { get; set; }
    }
}
