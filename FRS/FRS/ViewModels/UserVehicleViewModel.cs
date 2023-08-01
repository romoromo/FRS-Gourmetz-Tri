using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class UserVehicleViewModel
    {
        public int Id { get; set; }
        public string CardType { get; set; }
        [Required(ErrorMessage = "Plate number is required"), StringLength(200, ErrorMessage = "Plate number must be at most 200 characters")]
        public string PlateNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string VehicleStatus { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool IsApprove { get; set; }
    }

    public class UserVehicleGroupViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<UserVehicleViewModel> Vechicles { get; set; }
    }
}
