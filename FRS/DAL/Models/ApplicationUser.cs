using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using DAL.Models.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using Sieve.Attributes;
using DAL.Core.Audit.Attributes;
using DAL.Models.MealOrder;

namespace DAL.Models
{
    public class ApplicationUser : IdentityUser<int>, IAuditableEntity
    {
        public virtual string FriendlyName
        {
            get
            {
                string friendlyName = string.IsNullOrWhiteSpace(FullName) ? UserName : FullName;

                if (!string.IsNullOrWhiteSpace(JobTitle))
                    friendlyName = $"{JobTitle} {friendlyName}";

                return friendlyName;
            }
        }

        public string RegisteredDepartment { get; set; }
        public string EmployeeId { get; set; }
        public string JobTitle { get; set; }
        public string FullName { get; set; }
        [SkipTracking]
        public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
        public string Configuration { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsLockedOut => this.LockoutEnabled && this.LockoutEnd.HasValue;
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [SkipTracking]
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public string Pin { get; set; }
        public int? InstitutionId { get; set; }
        public int? DepartmentId { get; set; }
        public string UnitNumber { get; set; }
        public string HomeNo { get; set; }
        public string MobileNo { get; set; }
        public string Status { get; set; }
        public bool IsAD { get; set; }
        public string FirebaseToken { get; set; }
        public string UserType { get; set; }

        public bool NotFirstLogin { get; set; }
        public string ConfirmationCode { get; set; }

        public bool ConfirmReadTermsConditions { get; set; }
        public bool ConsentDataCollection { get; set; }
        public bool ReceivePromotionalMaterials { get; set; }
        public bool IsChangePassword { get; set; }
        public bool IsPasswordMustChange { get; set; } = false;

        [SkipTracking]
        public bool IsConnected { get; set; }
        [SkipTracking]
        public DateTime? LastLoginTime { get; set; }

        public DateTime? DeletedDate { get; set; }

        [SkipTracking]
        public DateTime? Last2FAValidatedTime { get; set; }

        [ForeignKey("InstitutionId")]
        public virtual Institution Institution { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        public int? FileId { get; set; }

        [ForeignKey("FileId")]
        public virtual File Icon { get; set; }

        public int? DirectoryListingId { get; set; }
        [ForeignKey("DirectoryListingId")]
        public virtual DirectoryListing DirectoryListing { get; set; }

        public virtual StudentAccount Account { get; set; }
        public virtual StaffAccount StaffAccount { get; set; }

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<ApplicationUserRole> Roles { get; set; }

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<UserClaim> Claims { get; set; }

        public virtual ICollection<UserPhonebook> UserPhonebooks { get; set; }

        public virtual ICollection<UserVehicle> UserVehicles { get; set; }

        public virtual ICollection<UserCardId> UserCardIds { get; set; }
        public virtual ICollection<ContactGroupMember> ContactGroupMembers { get; set; }
        public virtual ICollection<UserConnection> UserConnections { get; set; }
        public virtual ICollection<UserGroupMember> UserGroupMembers { get; set; }
        public virtual ICollection<Wallet> UserWallets { get; set; }
        public virtual ICollection<Reward> UserRewards { get; set; }

        public virtual ICollection<StudentManageAccount> Students { get; set; }
        public virtual ICollection<UsedPassword> UsedPasswords { get; set; }
        public virtual ICollection<UserOrderAlert> UserOrderAlerts { get; set; }
        public virtual ICollection<UserOutlet> UserOutlets { get; set; }
        public virtual ICollection<UserCaterer> UserCaterers { get; set; }
    }
}
