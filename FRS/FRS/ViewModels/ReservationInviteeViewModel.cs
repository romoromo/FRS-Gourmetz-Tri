using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class ReservationInviteeViewModel
    {
        private string _userId;
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserId
        {
            get
            {
                return string.IsNullOrEmpty(_userId) ? null : _userId;
            }

            set { _userId = value; }
        }

        public string Name { get; set; }
        public int? ContactGroupId { get; set; }


        public string Department { get; set; }

        public string Designation { get; set; }

        public string Company { get; set; }

        public string PhoneNumber { get; set; }

        public string Status { get; set; }

        public string PlateNumber { get; set; }

        public string CardType { get; set; }

        public string PersonName { get; set; }

        public string Vehicle { get; set; }

        public DateTime? IssueDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string VehicleStatus { get; set; }

        public string FilePath { get; set; }

        public int? FileId { get; set; }
    }
}
