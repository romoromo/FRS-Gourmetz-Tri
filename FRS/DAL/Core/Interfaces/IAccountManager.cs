using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Core.Interfaces
{
    public interface IAccountManager
    {

        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<Tuple<bool, string[]>> CreateRoleAsync(ApplicationRole role, IEnumerable<string> claims);
        Task<Tuple<bool, string[]>> CreateUserAsync(ApplicationUser user, IEnumerable<string> roles, string password, bool findByEmail = true);
        Task<Tuple<bool, string[]>> CreateUserWithPasswordAsync(ApplicationUser user, IEnumerable<string> roles, string password);
        Task<Tuple<bool, string[]>> DeleteRoleAsync(ApplicationRole role);
        Task<Tuple<bool, string[]>> DeleteRoleAsync(string roleName);
        Task<Tuple<bool, string[]>> DeleteUserAsync(ApplicationUser user);
        Task<Tuple<bool, string[]>> DeleteUserAsync(int userId);
        Task<ApplicationRole> GetRoleByIdAsync(int roleId);
        Task<ApplicationRole> GetRoleByNameAsync(string roleName);
        Task<ApplicationRole> GetRoleLoadRelatedAsync(string roleName, int? institutionId = null);
        Task<List<ApplicationRole>> GetRolesLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<Tuple<ApplicationUser, string[]>> GetUserAndRolesAsync(int userId);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<ApplicationUser> GetUserByIdAsync(int userId);
        Task<ApplicationUser> GetUserByUserNameAsync(string userName);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<Institution> GetCurrentInstitution();
        //Task<List<ApplicationRole>> GetUserRolesByUserIdAsync(int userId);
        Task<List<Tuple<ApplicationUser, string[]>>> GetUsersAndRolesAsync(int page, int pageSize, int? institutionId = null);
        Task<Tuple<bool, string[]>> ResetPasswordAsync(ApplicationUser user, string newPassword);
        Task<bool> TestCanDeleteRoleAsync(int roleId);
        Task<bool> TestCanDeleteUserAsync(int userId);
        Task<Tuple<bool, string[]>> UpdatePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task<Tuple<bool, string[]>> UpdateEmailAsync(ApplicationUser user, string email);
        Task<Tuple<bool, string[]>> UpdateRoleAsync(ApplicationRole role, IEnumerable<string> claims);
        Task<Tuple<bool, string[]>> UpdateUserAsync(ApplicationUser user);
        Task<Tuple<bool, string[]>> UpdateUserAsync(ApplicationUser user, IEnumerable<string> roles);
        Task<List<UserPhonebook>> GetUserPhonebooksLoadRelatedAsync(int page, int pageSize, int? userId = null);
        Task<BaseOperationResponse> CreateUserPhonebookAsync(UserPhonebook userPhonebook, string filePath = null);
        Task<BaseOperationResponse> UpdateUserPhonebookAsync(UserPhonebook userPhonebook, string filePath = null);
        Task<TestDeleteResult> TestCanDeleteUserPhonebookAsync(int userPhonebookId);
        Task<BaseOperationResponse> DeleteUserPhonebookAsync(int userPhonebookId);
        Task<BaseOperationResponse> DeleteUserPhonebook(UserPhonebook userPhonebook);
        Task<UserPhonebook> GetUserPhonebookByIdAsync(int userId);

        Task<List<UserVehicle>> GetUserVehiclesLoadRelatedAsync(int page, int pageSize, int? userId = null, string status = "");
        Task<BaseOperationResponse> CreateUserVehicleAsync(UserVehicle userVehicle);
        Task<BaseOperationResponse> UpdateUserVehicleAsync(UserVehicle userVehicle);
        Task<TestDeleteResult> TestCanDeleteUserVehicleAsync(int userVehicleId);
        Task<BaseOperationResponse> DeleteUserVehicleAsync(int userVehicleId);
        Task<BaseOperationResponse> DeleteUserVehicle(UserVehicle userVehicle);
        Task<UserVehicle> GetUserVehicleByIdAsync(int userId);

        Task<UserCardId> GetActiveUserCardId(int userId);
        Task<List<UserCardId>> GetUserCardIdsLoadRelatedAsync(int page, int pageSize, int? userId = null, string status = "");
        Task<UserCardId> GetUserByCardIdAsync(string cardId);
        Task<BaseOperationResponse> CreateUserCardIdAsync(UserCardId userCardId);
        Task<BaseOperationResponse> CreateUserCardIdActivateAsync(UserCardId userCardId);
        Task<BaseOperationResponse> UpdateUserCardIdAsync(UserCardId userCardId);
        Task<TestDeleteResult> TestCanDeleteUserCardIdAsync(int userCardId);
        Task<BaseOperationResponse> DeleteUserCardIdAsync(int userCardIdId);
        Task<BaseOperationResponse> DeleteUserCardId(UserCardId userCardId);
        Task<UserCardId> GetUserCardIdByIdAsync(int userId);


        Task<BaseOperationResponse> SyncLdap(bool includeContactGroup = true);
        Task<List<UserConnectionStatusDTO>> GetUserConnectionsAsync(int page, int pageSize);

        Task<PagedEntity<ApplicationUser>> GetUsersAsync(BaseFilter filter);
        Task<PagedEntity<ApplicationRole>> GetRolesAsync(BaseFilter filter);
        Task<List<ApplicationUser>> GetUserByClaimValueAsync(string claimValue);
        Task<PagedEntity<UserReportDTO>> GetUserReportAsync(BaseFilter filter);
        Task<byte[]> GenerateUserReportXls(BaseFilter filter);
        Task<PagedEntity<RoleReportDTO>> GetRoleReportAsync(RoleReportFilter filter);
        Task<byte[]> GenerateRoleReportXls(RoleReportFilter filter);
    }
}
