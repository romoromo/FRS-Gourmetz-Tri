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
using DAL.Core.DTO;
using System.ComponentModel.DataAnnotations;
using DAL.Core.Helpers;

namespace DAL.Repositories.MealOrder
{
    public class TokensOrderHistoryRepository : Repository<TokensOrderHistory>, ITokensOrderHistoryRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;

        public TokensOrderHistoryRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<TokensOrderHistory>> GetTokensOrderHistorysAsync(BaseFilter filter)
        {
            IQueryable<TokensOrderHistory> query = _appContext.TokensOrderHistorys;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion

        public async Task<List<TokensOrderHistory>> GetUnupdatedTokensOrderHistorysAsync()
        {
            IQueryable<TokensOrderHistory> query = _appContext.TokensOrderHistorys.Where(t => t.Status != "paid" && t.Payment != null && t.Payment.Status == "SUCCESS");

            return query.ToList();
        }

        public async Task<TokensOrderHistory> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync( TokensOrderHistory order)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {

                var f = await AddAsync(order);
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

        public async Task<BaseOperationResponse> UpdateAsync(TokensOrderHistory order)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                var f = await GetSingleOrDefaultAsync(e => e.Id == order.Id);

               
               

                f.CopyFrom(order);


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

        public async Task<BaseOperationResponse> DeleteAsync(int orderId)
        {
            var result = new BaseOperationResponse();
            var order = await GetSingleOrDefaultAsync(r => r.Id == orderId);

            if (order != null)
                return await Delete(order);

            result.IsSuccess = false;
            result.Message = "Order not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(TokensOrderHistory order)
        {
            var result = new BaseOperationResponse();
            SoftDelete(order);
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

        //public async Task<BaseOperationResponse> ImportStudentAsync(IAccountManager accountManager, List<StudentImportDTO> rows)
        //{
        //    var result = new BaseOperationResponse();
        //    try
        //    {
        //        if (rows.Count > 0)
        //        {
        //            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
        //                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = new System.TimeSpan(24, 0, 0) },
        //                    TransactionScopeAsyncFlowOption.Enabled))
        //            {
        //                List<int> studentIds = new List<int>();

        //                foreach (var row in rows)
        //                {
        //                    var sClass = await _appContext.Classes.FirstOrDefaultAsync(e => e.IsActive && e.Name.Equals(row.Class, StringComparison.InvariantCultureIgnoreCase));
        //                    if (sClass == null)
        //                    {
        //                        throw new Exception(string.Format("Class not found. Please check the imported file."));
        //                    }

        //                    var batch = await _appContext.ClassBatches.FirstOrDefaultAsync(e => e.IsActive && e.Name.Equals(row.Batch, StringComparison.InvariantCultureIgnoreCase));

        //                    var student = await _appContext.Students.FirstOrDefaultAsync(e => e.IsActive &&
        //                                        e.Name.Equals(row.Name, StringComparison.InvariantCultureIgnoreCase));

        //                    ApplicationUser user = null;
        //                    //check if student exists using account
        //                    if (student == null)
        //                    {
        //                        if (!string.IsNullOrEmpty(row.Email) && new EmailAddressAttribute().IsValid(row.Email))
        //                        {
        //                            user = await accountManager.GetUserByEmailAsync(row.Email);
        //                            if (user != null && user.Account != null)
        //                            {
        //                                student = await _appContext.Students.FirstOrDefaultAsync(e => e.IsActive && e.Id == user.Account.StudentId);
        //                            }
        //                        }
        //                    }

        //                    if (student != null)
        //                    {
        //                        //update
        //                        student.Name = row.Name;
        //                        student.ClassBatchId = batch?.Id;
        //                        student.ClassId = sClass.Id;
        //                        student.Gender = row.Gender;
        //                        student.IsFAS = row.IsFAS;
        //                        student.Weight = row.Weight;
        //                        student.Height = row.Height;

        //                        //check if account exists
        //                        if (user == null && !string.IsNullOrEmpty(row.Email) && new EmailAddressAttribute().IsValid(row.Email))
        //                        {
        //                            //create account
        //                            user = await accountManager.GetUserByEmailAsync(row.Email);
        //                            if (user == null)
        //                            {
        //                                if (student.Account != null)
        //                                {
        //                                    if (student.Account.User != null)
        //                                    {
        //                                        student.Account.User.Email = row.Email;
        //                                        _appContext.StudentAccounts.Update(student.Account);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    user = new ApplicationUser();
        //                                    user.IsEnabled = true;
        //                                    user.EmailConfirmed = true;
        //                                    user.UserName = row.Email.Substring(0, row.Email.IndexOf('@'));
        //                                    user.Email = row.Email;
        //                                    string newPassword = PasswordHelper.GenerateRandomPassword();
        //                                    var createUserResult = await accountManager.CreateUserAsync(user, new List<string>(), newPassword);
        //                                    if (createUserResult.Item1)
        //                                    {
        //                                        if (student.Account == null)
        //                                        {
        //                                            student.Account = new StudentAccount();
        //                                        }

        //                                        student.Account.UserId = user.Id;
        //                                        student.Account.StudentId = student.Id;
        //                                        await _appContext.StudentAccounts.AddAsync(student.Account);
        //                                    }
        //                                    else
        //                                    {
        //                                        result.Message = "Failed to create an account!";
        //                                        return result;
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        await _appContext.SaveChangesAsync();
        //                        studentIds.Add(student.Id);
        //                    }
        //                    else
        //                    {
        //                        //no student record and no account yet
        //                        student = new Student
        //                        {
        //                            ClassBatchId = batch?.Id,
        //                            ClassId = sClass.Id,
        //                            Name = row.Name,
        //                            Gender = row.Gender,
        //                            IsFAS = row.IsFAS,
        //                            Weight = row.Weight,
        //                            Height = row.Height
        //                        };

        //                        if (user == null && !string.IsNullOrEmpty(row.Email) && new EmailAddressAttribute().IsValid(row.Email))
        //                        {
        //                            //create account
        //                            user = await accountManager.GetUserByEmailAsync(row.Email);
        //                            if (user == null)
        //                            {
        //                                user = new ApplicationUser();
        //                                user.IsEnabled = true;
        //                                user.EmailConfirmed = true;
        //                                user.UserName = row.Email.Substring(0, row.Email.IndexOf('@'));
        //                                user.Email = row.Email;
        //                                user.IsActive = true;
        //                                user.InstitutionId = (await accountManager.GetCurrentInstitution())?.Id;
        //                                string newPassword = PasswordHelper.GenerateRandomPassword();
        //                                var createUserResult = await accountManager.CreateUserAsync(user, new List<string>(), newPassword);
        //                                if (createUserResult.Item1)
        //                                {
        //                                    if (student.Account == null)
        //                                    {
        //                                        student.Account = new StudentAccount();
        //                                    }

        //                                    student.Account.User = user;
        //                                }
        //                                else
        //                                {
        //                                    result.Message = "Failed to create an account!";
        //                                    return result;
        //                                }
        //                            }
        //                        }

        //                        var stud = await AddAsync(student);
        //                        await _appContext.SaveChangesAsync();
        //                        studentIds.Add(stud.Id);
        //                    }
        //                }

        //                //disable removed students
        //                var studentsToDisable = _appContext.Students.Where(e => studentIds.All(f => f != e.Id));

        //                foreach (var stud in studentsToDisable)
        //                {
        //                    stud.IsActive = false;
        //                    Update(stud);
        //                    await _appContext.SaveChangesAsync();
        //                }

        //                scope.Complete();
        //                result.IsSuccess = true;
        //                result.Message = "File Imported!";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccess = false;
        //        result.Message = ex.Message;
        //    }



        //    return result;
        //}

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
