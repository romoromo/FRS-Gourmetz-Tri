using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class StaffDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        public string UserType { get; set; }

        public int? StaffTypeId { get; set; }

        public string StaffTypeName { get; set; }

        public int? DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public int? UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string NewPassword { get; set; }

        public string CurrentPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public List<StaffCardDTO> Cards { get; set; }
    }

    public class StaffCardDTO
    {
        public int Id { get; set; }
        public string CardId { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public int? UserId { get; set; }
    }
}
