using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class SmartRoomResource
    {
        public SmartRoomResource()
        {
        }

        [Key]
        public int Id { get; set; }

        public int SmartRoomResourceID { get; set; }
        public string AlternateID { get; set; }
        public string Name { get; set; }
        public int? FloorID { get; set; }
        public string Floor { get; set; }
        public string ExchangeID { get; set; }
        public string Extension { get; set; }
        public string IP { get; set; }
        public string DeviceName { get; set; }
        public string Category { get; set; }
        public string Remarks { get; set; }
        public int? FloorLocationX { get; set; }
        public int? FloorLocationY { get; set; }
        public int SeatingCapacity { get; set; }
        public string Access { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? CreatedBy { get; set; }
    }
}
