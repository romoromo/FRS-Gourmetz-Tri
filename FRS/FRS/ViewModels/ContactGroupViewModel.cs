using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class ContactGroupSimpleViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public string DepartmentNames { get; set; }
    }

    public class ContactGroupViewModel
    {
        public ContactGroupViewModel()
        {
            Members = new List<ContactGroupMemberViewModel>();
            DepartmentIds = new List<int>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Description must be between 2 and 200 characters")]
        public string Description { get; set; }

        public int InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public string DepartmentNames { get; set; }
        public List<int> DepartmentIds { get; set; }

        public List<DepartmentViewModel> Departments { get; set; }

        public List<ContactGroupMemberViewModel> Members { get; set; }
    }

    public class ContactGroupMemberViewModel
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "Name is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required"), StringLength(200, ErrorMessage = "Email must be at most 200 characters"), EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public int ContactGroupId { get; set; }

        public string ContactGroupName { get; set; }

        public string Department { get; set; }

        public string Designation { get; set; }

        public string Company { get; set; }

        public string PhoneNumber { get; set; }

        public string HomeNo { get; set; }

        public string MobileNo { get; set; }

        public string FilePath { get; set; }

        public byte[] FileBytes { get; set; }

        public int? UserId { get; set; }

        public string Status { get; set; }

        public bool IsConnected { get; set; }
    }
}
