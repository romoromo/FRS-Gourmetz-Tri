using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class UpDownTimeLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string DeviceIdentifier { get; set; }

        public bool IsUp { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
