using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class UserCardIdViewModel
    {
        public int Id { get; set; }
        public string CardId { get; set; }
        [Required(ErrorMessage = "Card Id is required"), StringLength(200, ErrorMessage = "Card Id number must be at most 200 characters")]
        public string Status { get; set; }
        public string Remarks { get; set; }
        public DateTime? IssueDate { get; set; }
        public int UserId { get; set; }

        public UserViewModel UserObj { get; set; }
    }
}
