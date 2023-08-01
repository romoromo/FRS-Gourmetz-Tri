using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class VehicleViewModel
    {
        public string SeasonId { get; set; }

        public string PersonName { get; set; }

        public string CardType { get; set; }

        public string Vehicle { get; set; }

        public DateTime? IssueDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

    }

    public class VehicleEventViewModel
    {
        public int VehicleId { get; set; }

        public int EventId { get; set; }

        public string Email { get; set; }

        public string EventDescription { get; set; }
    }
}
