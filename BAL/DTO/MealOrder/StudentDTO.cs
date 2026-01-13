using DAL.Core.Audit.Attributes;
using DAL.Core.DTO;
using DAL.Models.MealOrder;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class StudentSimpleDTO
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
        public int DaysToFreezeOrdering { get; set; }

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

        public string ParentEmail { get; set; }

        public bool isFrontendVerified { get; set; }

        public bool isNotifNoOrderMadeForNextWeek { get; set; }
        public bool isNotifAbandonCart { get; set; }
        public bool isNotifCancellationRequestStatus { get; set; }
        public bool isNotifNoCardSetup { get; set; }
        public bool isNotifMissedCollection { get; set; }
        public double WalletBalance { get; set; }
        public byte[] ConcurrencyStamp { get; set; }
        public double PointBalance { get; set; }
        public bool IsWalletFreeze { get; set; }
        public double WalletDailyLimit { get; set; }
        public int? PhotoId { get; set; }
        public string PhotoName { get; set; }
        public string PhotoPath { get; set; }
        public string ImgUrl { get; set; }
    }

    public class StudentDTO : StudentSimpleDTO
    {
        public List<UserCardIdDTO> Cards { get; set; }
        public List<StudentCardDTO> StudentCards { get; set; }
        public List<StudentRestrictionDTO> Restrictions { get; set; }
        public List<StudentInterestGroupDTO> InterestGroups { get; set; }
        public List<StudentVoucherDTO> Vouchers { get; set; }
        public List<StudentManageAccountDTO> Users { get; set; }
        public List<StudentAccountLinkRequestDTO> AccountLinkRequests { get; set; }

        public List<WalletPaymentDTO> WalletPayments { get; set; }
        public List<StudentWalletDTO> StudentWallets { get; set; }
        public List<StudentPointDTO> StudentPoints { get; set; }
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
        public DateTime? IssueDate { get; set; }
        
    }

    public class StudentCardDTO : CardDTO
    {
        public int StudentId { get; set; }
    }

    public class UserCardIdDTO: CardDTO
    {
        public int? UserId { get; set; }
    }

    public class StudentAccountLinkRequestDTO
    {
        public string Email { get; set; }
        public string Status { get; set; }
        public bool EmailSent { get; set; }
    }


    public class StudentWalletTransactionDTO
    {
        public int Id { get; set; }

        public DateTime? TransactionDateTime { get; set; }

        public int StudentId { get; set; }

        public byte[] ConcurrencyStamp { get; set; }
        public double Amount { get; set; }

        public string TransactionType { get; set; }

        public string Description { get; set; }

        public string Remarks { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string StudentName { get; set; }

        public int? PaymentId { get; set; }
        public PaymentDTO Payment { get; set; }

        public int? TokenOrderId { get; set; }
        public TokenOrderDTO TokenOrder { get; set; }
        public List<StudentWalletTransactionDetailDTO> Details { get; set; }
    }

    public class StudentWalletTransactionDetailDTO
    {
        public int TransactionId { get; set; }
        public double Amount { get; set; }
        public double AmountRefunded { get; set; }
        public string Type { get; set; }
    }

    public class StudentPointTransactionDTO
    {
        public int Id { get; set; }

        public DateTime? TransactionDateTime { get; set; }

        public int StudentId { get; set; }

        public byte[] ConcurrencyStamp { get; set; }
        public double Amount { get; set; }

        public string TransactionType { get; set; }

        public string Description { get; set; }

        public string Remarks { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string StudentName { get; set; }
    }

    public class EmailConfirmDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
        public DateTime? Date { get; set; }
    }

    public class StudentLiteDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class StudentLiteWithClassDTO : StudentLiteDTO
    {
        public string Class { get; set; }
    }
}
