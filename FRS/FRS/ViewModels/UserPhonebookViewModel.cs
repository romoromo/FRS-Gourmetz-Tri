using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class UserPhonebookViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required"), StringLength(200, ErrorMessage = "Email must be at most 200 characters"), EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string HomeNo { get; set; }

        public string MobileNo { get; set; }

        public string Designation { get; set; }

        public int UserId { get; set; }

        public string FilePath { get; set; }

        public byte[] FileBytes { get; set; }

        public List<UserCardIdViewModel> CardIds { get; set; }

        public List<UserVehicleViewModel> Vehicles { get; set; }
    }
}
