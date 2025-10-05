using DAL.Core.Audit.Attributes;
using Org.BouncyCastle.Asn1.Ocsp;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.MealOrder
{
    public class Student : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Gender { get; set; }

        public string Email { get; set; }
        public string newPassword { get; set; }

        public int ClassId { get; set; }

        public int? ClassLevelId { get; set; }

        public bool IsFAS { get; set; }

        public float Weight { get; set; }

        public float Height { get; set; }

        public bool ReceivePromotionalMaterials { get; set; }

        public float TargetWeeklyCalIntake { get; set; }

        public int? ClassBatchId { get; set; }

        public int? OutletId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public double WalletBalance { get; set; }

        public bool IsWalletFreeze { get; set; }

        public double WalletDailyLimit { get; set; }

        [SkipTracking]
        public byte[] ConcurrencyStamp { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public double PointBalance { get; set; }

        public bool isNotifNoOrderMadeForNextWeek { get; set; }
        public bool isNotifAbandonCart { get; set; }
        public bool isNotifCancellationRequestStatus { get; set; }
        public bool isNotifNoCardSetup { get; set; }
        public bool isNotifMissedCollection { get; set; }

        [ForeignKey("OutletId")]
        public virtual Outlet Outlet { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        [ForeignKey("ClassLevelId")]
        public virtual ClassLevel ClassLevel { get; set; }

        [ForeignKey("ClassBatchId")]
        public virtual ClassBatch ClassBatch { get; set; }

        public virtual StudentAccount Account { get; set; }
        public virtual ICollection<StudentCard> StudentCards { get; set; }
        public virtual ICollection<StudentRestriction> Restrictions { get; set; }
        public virtual ICollection<StudentInterestGroup> InterestGroups { get; set; }
        public virtual ICollection<StudentVoucher> Vouchers { get; set; }
        public virtual ICollection<StudentManageAccount> Users { get; set; }
        public virtual ICollection<StudentAccountLinkRequest> AccountLinkRequests { get; set; }

        public virtual ICollection<WalletPayment> WalletPayments { get; set; }


    }
}
