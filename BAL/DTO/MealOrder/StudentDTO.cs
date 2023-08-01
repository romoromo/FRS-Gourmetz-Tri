using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class StudentDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        public int ClassId { get; set; }

        public int ClassLevelId { get; set; }

        public int? ClassBatchId { get; set; }

        public bool IsFAS { get; set; }

        public float Weight { get; set; }

        public float Height { get; set; }

        public string ClassName { get; set; }

        public string ClassLevelName { get; set; }

        public string ClassBatchName { get; set; }

        public int Year { get; set; }

        public int? OutletId { get; set; }

        public string OutletName { get; set; }

        public string UserType { get; set; }

        public int? UserId { get; set; }

        public int StudentId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string NewPassword { get; set; }

        public string CurrentPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public float TargetWeeklyCalIntake { get; set; }

        public bool ReceivePromotionalMaterials { get; set; }

        public bool IsChangePassword { get; set; }

        public bool isNotifNoOrderMadeForNextWeek { get; set; }
        public bool isNotifAbandonCart { get; set; }
        public bool isNotifCancellationRequestStatus { get; set; }
        public bool isNotifNoCardSetup { get; set; }
        public bool isNotifMissedCollection { get; set; }

        public List<UserCardIdDTO> Cards { get; set; }
        public List<StudentCardDTO> StudentCards { get; set; }
        public List<StudentRestrictionDTO> Restrictions { get; set; }
        public List<StudentInterestGroupDTO> InterestGroups { get; set; }
        public List<StudentVoucherDTO> Vouchers { get; set; }
        public List<StudentManageAccountDTO> Users { get; set; }
    }

    public class StudentManageAccountDTO
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }
    }

    public class StudentRestrictionDTO
    {
        public int StudentId { get; set; }
        public string RestrictionCode { get; set; }
        public string RestrictionTypeCode { get; set; }
        public int RestrictionId { get; set; }
    }

    public class StudentVoucherDTO
    {
        public int StudentId { get; set; }
        public string VoucherCode { get; set; }
        public int VoucherId { get; set; }
        public string Status { get; set; }
    }

    public class StudentInterestGroupDTO
    {
        public int StudentId { get; set; }
        public int InterestGroupId { get; set; }
    }

    public class CardDTO
    {
        public int Id { get; set; }
        public string CardId { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        
    }

    public class StudentCardDTO : CardDTO
    {
        public int StudentId { get; set; }
    }

    public class UserCardIdDTO: CardDTO
    {
        public int? UserId { get; set; }
    }
}
