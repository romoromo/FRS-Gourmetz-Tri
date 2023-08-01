using DAL.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class SmartRoomSchedulerLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime EventDateTime { get; set; }
        public string Status { get; set; }
        public int NoRecordsAffected { get; set; }
        public string Details { get; set; }
    }
}
