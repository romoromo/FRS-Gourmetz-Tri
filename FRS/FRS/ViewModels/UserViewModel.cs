using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Models;
using FluentValidation;
using FRS.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace FRS.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "Username is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 200 characters")]
        public string UserName { get; set; }

        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required"), StringLength(200, ErrorMessage = "Email must be at most 200 characters"), EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public string RegisteredDepartment { get; set; }

        public string EmployeeId { get; set; }

        public string JobTitle { get; set; }

        public string PhoneNumber { get; set; }

        public string Configuration { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsLockedOut { get; set; }

        //[MinimumCount(1, ErrorMessage = "Roles cannot be empty")]
        public string[] Roles { get; set; }

        public string Pin { get; set; }
        public int? InstitutionId { get; set; }

        public string InstitutionCode { get; set; }

        public int? DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public string UnitNumber { get; set; }

        public string HomeNo { get; set; }

        public string MobileNo { get; set; }

        public string FilePath { get; set; }

        public int? FileId { get; set; }

        public string Status { get; set; }

        public bool IsAD { get; set; }

        public bool IsConnected { get; set; }

        public bool IsChangePassword { get; set; }

        public int? DirectoryListingId { get; set; }

        public string FirebaseToken { get; set; }

        public UserPhonebookViewModel[] Phonebooks { get; set; }
        public UserVehicleViewModel[] UserVehicles { get; set; }
        public UserCardIdViewModel[] UserCardIds { get; set; }
        public int[] UserGroupIds { get; set; }
        public UserGroupMemberViewModel[] UserGroupMembers { get; set; }
        public WalletDTO[] UserWallets { get; set; }
        public RewardDTO[] UserRewards { get; set; }

        public List<StudentManageAccountViewModel> Students { get; set; }
        public List<UserOutletViewModel> UserOutlets { get; set; }
        public List<UserCatererViewModel> UserCaterers { get; set; }
        public string DirectoryListingCode { get; set; }
        public string DirectoryListingLabel { get; set; }
        public bool IsPasswordMustChange { get; set; }

        public string ConfirmationCode { get; set; }
    }

    public class UserSimpleViewModel
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "Username is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 200 characters")]
        public string UserName { get; set; }

        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required"), StringLength(200, ErrorMessage = "Email must be at most 200 characters"), EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public string RegisteredDepartment { get; set; }

        public string EmployeeId { get; set; }

        public string JobTitle { get; set; }

        public string PhoneNumber { get; set; }

        public string Configuration { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsLockedOut { get; set; }


        public string Pin { get; set; }
        public int? InstitutionId { get; set; }

        public int? DepartmentId { get; set; }

        public string UnitNumber { get; set; }

        public string HomeNo { get; set; }

        public string MobileNo { get; set; }

        public int? FileId { get; set; }

        public bool IsAD { get; set; }

        public int? DirectoryListingId { get; set; }

        public string FirebaseToken { get; set; }

        public bool IsPasswordMustChange { get; set; }

        public string ConfirmationCode { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public string UserId { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }

    public class StudentManageAccountViewModel
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string Name { get; set; }

        public string OutletName { get; set; }

        public int Year { get; set; }

        public string ClassName { get; set; }

        public int UserId { get; set; }

        public bool IsActive { get; set; }
    }

    public class UserOutletViewModel
    {
        public int UserId { get; set; }
        public int OutletId { get; set; }
        public string OutletName { get; set; }
    }

    public class UserCatererViewModel
    {
        public int UserId { get; set; }
        public int CatererId { get; set; }
        public string CatererName { get; set; }
    }
    ////Todo: ***Using DataAnnotations for validations until Swashbuckle supports FluentValidation***
    //public class UserViewModelValidator : AbstractValidator<UserViewModel>
    //{
    //    public UserViewModelValidator()
    //    {
    //        //Validation logic here
    //        RuleFor(user => user.UserName).NotEmpty().WithMessage("Username cannot be empty");
    //        RuleFor(user => user.Email).EmailAddress().NotEmpty();
    //        RuleFor(user => user.Password).NotEmpty().WithMessage("Password cannot be empty").Length(4, 20);
    //    }
    //}
}
