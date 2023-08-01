using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces.MealOrder;
using System.Transactions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DAL.Core.Interfaces;

namespace DAL.Repositories.MealOrder
{
    public class StaffRepository : Repository<Staff>, IStaffRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;

        public StaffRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<Staff>> GetStaffsAsync(BaseFilter filter)
        {
            IQueryable<Staff> query = _appContext.Staffs.Include(e => e.Account).ThenInclude(e => e.User).ThenInclude(e => e.UserCardIds);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<Staff> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(IAccountManager accountManager, Staff staff, ApplicationUser user, string newPassword, List<UserCardId> cards)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                if (user != null && !string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(newPassword))
                {
                    var existingUsername = await accountManager.GetUserByUserNameAsync(user.UserName);
                    if (existingUsername != null && existingUsername.IsActive)
                    {
                        //username exists
                        result.Message = "Failed to save! Username already exists.";
                        return result;
                    }

                    user.IsEnabled = true;
                    user.EmailConfirmed = true;
                    user.IsActive = true;

                    if (!user.InstitutionId.HasValue)
                    {
                        user.InstitutionId = (await accountManager.GetCurrentInstitution())?.Id;
                    }

                    var createUserResult = await accountManager.CreateUserAsync(user, new List<string>(), newPassword);
                    if (createUserResult.Item1)
                    {
                        if (staff.Account == null)
                        {
                            staff.Account = new StaffAccount();
                        }

                        staff.Account.User = user;
                        staff.Account.User.UserCardIds = cards;
                    }
                    else
                    {
                        result.Message = $"Failed to save user. Errors: {string.Join(Environment.NewLine, createUserResult.Item2)}";
                        return result;
                    }

                    staff.Account.User = user;
                }

                var f = await AddAsync(staff);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                    scope.Complete();
                }
                else
                {
                    result.Message = "Failed to save!";

                }
            }
            return result;
            
        }

        public async Task<BaseOperationResponse> UpdateAsync(IAccountManager accountManager, Staff staff, ApplicationUser user, string currentPassword, string newPassword, List<UserCardId> cards)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                var f = await GetSingleOrDefaultAsync(e => e.Id == staff.Id);

                if (user != null && !string.IsNullOrEmpty(user.UserName))
                {
                    var appUser = await accountManager.GetUserByIdAsync(user.Id);

                    if (appUser != null)
                    {
                        //var appUser = await _appContext.Users.SingleOrDefaultAsync(e => e.Id == user.Id);
                        var existingUsername = await accountManager.GetUserByUserNameAsync(user.UserName);
                        if(existingUsername != null && existingUsername.Id != appUser.Id && existingUsername.IsActive)
                        {
                            //username exists
                            result.Message = "Failed to save! Username already exists.";
                            return result;
                        }

                        appUser.UserType = user.UserType;
                        appUser.UserName = user.UserName;
                        appUser.IsActive = true;
                        if (!appUser.InstitutionId.HasValue)
                        {
                            appUser.InstitutionId = (await accountManager.GetCurrentInstitution())?.Id;
                        }

                        var updateUserResult = await accountManager.UpdateUserAsync(appUser, new List<string>());
                        if (updateUserResult.Item1)
                        {
                            if (staff.Account == null)
                            {
                                staff.Account = new StaffAccount();
                            }

                            staff.Account.UserId = appUser.Id;

                            if (!string.IsNullOrEmpty(newPassword))
                            {
                                if (!string.IsNullOrWhiteSpace(newPassword))
                                {
                                    if (!string.IsNullOrWhiteSpace(currentPassword))
                                        updateUserResult = await accountManager.UpdatePasswordAsync(appUser, currentPassword, newPassword);
                                    else
                                        updateUserResult = await accountManager.ResetPasswordAsync(appUser, newPassword);
                                }

                            }
                        }

                        if (!updateUserResult.Item1)
                        {
                            result.Message = "Failed to save!";
                            return result;
                        }

                        staff.Account.User = appUser;

                        _appContext.UserCardIds.Where(e => e.UserId == appUser.Id).ToList().ForEach(e => {
                            e.IsActive = false;
                            _appContext.UserCardIds.Update(e);
                        });

                        //add/update cards here
                        foreach (var c in cards)
                        {
                            var card = await _appContext.UserCardIds.SingleOrDefaultAsync(e => e.Id == c.Id || (e.UserId == c.UserId && e.CardId.Equals(c.CardId, StringComparison.CurrentCultureIgnoreCase)));
                            if(card == null || card.Id == 0)
                            {
                                c.IsActive = true;
                                await _appContext.UserCardIds.AddAsync(c);
                            }
                            else
                            {
                                int oldId = card.Id;
                                
                                card.CopyFrom(c);
                                card.Id = oldId;
                                card.IsActive = true;
                                _appContext.UserCardIds.Update(card);
                            }
                        }
                    }
                    else
                    {
                        appUser = user;
                        appUser.IsEnabled = true;
                        appUser.EmailConfirmed = true;
                        appUser.IsActive = true;
                        if (!appUser.InstitutionId.HasValue)
                        {
                            appUser.InstitutionId = (await accountManager.GetCurrentInstitution())?.Id;
                        }

                        appUser.UserCardIds = cards;
                        var existingUsername = await accountManager.GetUserByUserNameAsync(user.UserName);
                        if (existingUsername != null && existingUsername.IsActive)
                        {
                            //username exists
                            result.Message = "Failed to save! Username already exists.";
                            return result;
                        }

                        var createUserResult = await accountManager.CreateUserAsync(appUser, new List<string>(), newPassword, false);
                        if (createUserResult.Item1)
                        {
                            if (staff.Account == null)
                            {
                                staff.Account = new StaffAccount();
                            }

                            staff.Account.UserId = appUser.Id;
                        }
                        else
                        {
                            result.Message = $"Failed to save user. Errors: {string.Join(Environment.NewLine, createUserResult.Item2)}";
                            return result;
                        }

                        staff.Account.UserId = appUser.Id;
                        staff.Account.StaffId = staff.Id;
                        await _appContext.StaffAccounts.AddAsync(staff.Account);
                    }

                }

                f.CopyFrom(staff);

                Update(f);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;

                    scope.Complete();
                }
                else
                {
                    result.Message = "Failed to save!";
                    result.IsSuccess = false;
                }

            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int staffId)
        {
            var result = new BaseOperationResponse();
            var staff = await GetSingleOrDefaultAsync(r => r.Id == staffId);

            if (staff != null)
                return await Delete(staff);

            result.IsSuccess = false;
            result.Message = "Staff not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Staff staff)
        {
            var result = new BaseOperationResponse();
            SoftDelete(staff);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
