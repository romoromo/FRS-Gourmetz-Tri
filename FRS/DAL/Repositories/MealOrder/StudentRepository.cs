using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces.MealOrder;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using DAL.Core.Interfaces;
using DAL.Core.DTO;
using System.ComponentModel.DataAnnotations;
using DAL.Core.Helpers;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using System.Data;
using IsolationLevel = System.Transactions.IsolationLevel;
using Microsoft.Extensions.Logging;
using DAL.Core.Logging;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories.MealOrder
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        private IPasswordHasher<ApplicationUser> _passwordHasher;
        private ILogger _logger;
        private IConfiguration _configuration;

        public StudentRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId,IConfiguration configuration) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentUserId;
            _passwordHasher = new PasswordHasher<ApplicationUser>();
            _logger = Logger.CreateLogger<DeviceRepository>();
            _configuration = configuration;
        }

        public async Task<IQueryable<Student>> GetAllStudentsAsync()
        {
            return _appContext.Students;
        }

        #region Sieved
        public async Task<PagedEntity<Student>> GetStudentsAsync(BaseFilter filter, bool noAccount = false)
        {
            IQueryable<Student> query = _appContext.Students.Include(e => e.Account).ThenInclude(e => e.User).ThenInclude(e => e.UserCardIds);

            if (noAccount) query = query.Where(q => q.Account == null && q.Email != null && q.Email != "");

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion

        public async Task<List<Student>> GetStudentsByUserAsync(int userId)
        {
            var user = _appContext.Users.FirstOrDefault(u => u.Id == userId && u.IsActive);

            var students = new List<Student>();

            if(user.Account != null && user.Account.Student != null) students.Add(user.Account.Student);

            if (user.Students != null)
            {
                var ss = user.Students.Where(s => s.IsActive && s.Student != null && s.Student.IsActive).Select(s => s.Student).ToList();

                var uniqueStudents = ss.Where(e => !students.Any(f => f.Id == e.Id));
                students.AddRange(uniqueStudents);
            }

            return students;
        }

        public async Task<BaseOperationResponse> AddStudentLinksByUserAsync(int userId, int studentId)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                var user = _appContext.Users.FirstOrDefault(u => u.Id == userId && u.IsActive);

                if (user != null)
                {
                    var student = _appContext.Students.FirstOrDefault(u => u.Id == studentId && u.IsActive);

                    if (student == null)
                    {
                        result.Message = "Failed to save! Student does not exist.";
                        return result;
                    }

                    if (user.Students == null || !user.Students.Any(e => e.StudentId == studentId))
                    {
                        await _appContext.StudentManageAccounts.AddAsync(new StudentManageAccount { StudentId = studentId, UserId = user.Id });
                        await _appContext.SaveChangesAsync();

                        result.Message = "Successfully saved!";
                        result.IsSuccess = true;
                        scope.Complete();
                    }
                    else
                    {
                        result.Message = "Failed to save! Email already linked to this student.";
                    }
                }
                else
                {
                    result.Message = "Failed to save! User does not exist.";
                }
            }

            return result;
        }

        public async Task<BaseOperationResponse> AddStudentAccountLinkRequestAsync(int studentId, string email, bool emailSent)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var student = _appContext.Students.FirstOrDefault(u => u.Id == studentId && u.IsActive);

                    if (student == null)
                    {
                        result.Message = "Failed to save! Student does not exist.";
                        return result;
                    }

                    await _appContext.StudentAccountLinkRequests.AddAsync(new StudentAccountLinkRequest { StudentId = studentId, Email = email, EmailSent = emailSent, Status = "Pending" });
                    await _appContext.SaveChangesAsync();

                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    scope.Complete();
                }
                catch (Exception)
                {
                    result.Message = "Failed to save account request.";
                }
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateStudentAccountLinkRequestAsync(int studentId, string email)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var request = _appContext.StudentAccountLinkRequests.FirstOrDefault(u => u.StudentId == studentId && u.IsActive && u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

                    if (request == null)
                    {
                        result.Message = "Failed to save! Request does not exist.";
                        return result;
                    }

                    request.Status = "Accepted";
                    _appContext.StudentAccountLinkRequests.Update(request);
                    await _appContext.SaveChangesAsync();

                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    scope.Complete();
                }
                catch (Exception)
                {
                    result.Message = "Failed to save account request.";
                }
            }

            return result;
        }

        public async Task<List<Student>> GetStudentsWithNoOrder(DateTime from, DateTime to)
        {
            var now = DateTime.Now;
            var studentHasOrdersForNextWeek = _appContext.TokenOrders.Where(u => u.IsActive &&
                                        u.DeliveryDate.Date >= from && u.DeliveryDate.Date <= to &&
                                        u.Status == "paid").Select(e => e.ProfileId).Distinct().ToList();

            var usersWithoutOrders = _appContext.Students.Where(u => u.IsActive && !studentHasOrdersForNextWeek.Any(e => e == u.Id) &&
                                        !u.Account.User.UserOrderAlerts.Any(f => f.NoOrderNextWeekSentDate.HasValue && f.NoOrderNextWeekSentDate.Value.Date >= now.Date))
                                        .ToList();
            //var usersWithoutOrders = _appContext.Students.Where(u => u.IsActive && !userIdsWithOrders.Any(f => f == u.Id));

            return usersWithoutOrders;
        }

        public async Task<List<Student>> GetStudentsWithAbandonedCart1(int hoursLeft)
        {
            var now = DateTime.Now;
            var studentWithPendingOrders = _appContext.TokenOrders.Where(u => u.IsActive &&
                                        u.Status == "pending" &&
                                        (now.Hour - u.CreatedDate.Hour > hoursLeft)).Select(e => e.ProfileId).Distinct().ToList();

            var usersWithPendingOrders = _appContext.Students.Where(u => u.IsActive && !studentWithPendingOrders.Any(e => e == u.Id) &&
                                        !u.Account.User.UserOrderAlerts.Any(f => f.AbandonedCart1SentDate.HasValue && f.AbandonedCart1SentDate.Value.Date >= now.Date))
                                        .ToList();

            //var userIdsWithPendingOrders = _appContext.TokenOrders.Where(u => u.IsActive &&
            //                            u.Status == "pending" &&
            //                            (now.Hour - u.CreatedDate.Hour > hoursLeft) &&
            //                            ((u.Student.Account.User.UserOrderAlerts == null || !u.Student.Account.User.UserOrderAlerts.Any()) || 
            //                                (!u.Student.Account.User.UserOrderAlerts.Any(f => f.AbandonedCart1SentDate.HasValue && f.AbandonedCart1SentDate.Value.Date >= now.Date)))
            //                            ).Select(e => e.ProfileId).Distinct().ToList();
            //var usersWithPendingOrders = _appContext.Students.Where(u => u.IsActive && !userIdsWithPendingOrders.Any(f => f == u.Id));

            return usersWithPendingOrders;
        }

        public async Task<List<Student>> GetStudentsWithAbandonedCart2(int daysBeforeCutOff)
        {
            var cutOffDay = DateTime.Now.AddDays(daysBeforeCutOff);
            var now = DateTime.Now;
            var studentWithPendingOrders = _appContext.TokenOrders.Where(u => u.IsActive &&
                                        u.Status == "pending" &&
                                        (u.DeliveryDate.Date.AddDays(-daysBeforeCutOff).Date == cutOffDay.Date))
                                        .Select(e => e.ProfileId).Distinct().ToList();

            var usersWithPendingOrders = _appContext.Students.Where(u => u.IsActive && !studentWithPendingOrders.Any(e => e == u.Id) &&
                                        !u.Account.User.UserOrderAlerts.Any(f => f.AbandonedCart2SentDate.HasValue && f.AbandonedCart2SentDate.Value.Date >= now.Date))
                                        .ToList();


            //var userIdsWithPendingOrders = _appContext.TokenOrders.Where(u => u.IsActive &&
            //                            u.Status == "pending" &&
            //                            (u.DeliveryDate.Date.AddDays(-daysBeforeCutOff).Date == cutOffDay.Date) &&
            //                            ((u.Student.Account.User.UserOrderAlerts == null || !u.Student.Account.User.UserOrderAlerts.Any()) ||
            //                                (!u.Student.Account.User.UserOrderAlerts.Any(f => f.AbandonedCart2SentDate.HasValue && f.AbandonedCart2SentDate.Value.Date >= now.Date)))
            //                            ).Select(e => e.ProfileId).Distinct().ToList();
            //var usersWithPendingOrders = _appContext.Students.Where(u => u.IsActive && !userIdsWithPendingOrders.Any(f => f == u.Id));

            return usersWithPendingOrders.ToList();
        }

        public async Task<List<StudentOrderModel>> GetStudentsWithOrdersNotCollected(int daysPassed)
        {
            var now = DateTime.Now;
            var orders = _appContext.TokenOrders.Where(u => u.IsActive &&
                                        u.Status == "paid" && !u.CollectionTime.HasValue && ((now.Date - u.DeliveryDate.Date).Days == daysPassed))
                                        .ToList();

            var userIdsWithOrdersNotCollected = orders.Select(e => e.ProfileId).Distinct().ToList();

            var usersWithOrdersNotCollected = _appContext.Students.Where(u => u.IsActive && !userIdsWithOrdersNotCollected.Any(e => e == u.Id) &&
                                        !u.Account.User.UserOrderAlerts.Any(f => f.MissedCollectedSentDate.HasValue && f.MissedCollectedSentDate.Value.Date >= now.Date))
                                        .Select(e =>
                                            new StudentOrderModel
                                            {
                                                Student = e,
                                                TokenOrders = orders.Where(x => x.ProfileId == e.Id)
                                            }).ToList();

            //var usersWithOrdersNotCollected = _appContext.TokenOrders.Where(u => u.IsActive &&
            //                            u.Status == "paid" && !u.CollectionTime.HasValue && ((now.Date - u.DeliveryDate.Date).Days == daysPassed) &&
            //                            ((u.Student.Account.User.UserOrderAlerts == null || !u.Student.Account.User.UserOrderAlerts.Any()) ||
            //                                (!u.Student.Account.User.UserOrderAlerts.Any(f => f.MissedCollectedSentDate.HasValue && f.MissedCollectedSentDate.Value.Date >= now.Date)))
            //                            ).GroupBy(e => new { StudentId = e.Student.Id, OrderId = e.Id })
            //                            .Select(e =>
            //                                new StudentOrderModel
            //                                {
            //                                    Student = e.First().Student,
            //                                    TokenOrders = e
            //                                }).ToList();

            return usersWithOrdersNotCollected;
        }
        public async Task<Student> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<Student> GetStudentByEmailAsync(string email)
        {
            return await GetFirstOrDefaultAsync(e => e.IsActive && email.ToLower() == e.Email.ToLower());
        }

        public async Task<List<Student>> GetByInterestGroupIdAsync(int id)
        {
            return (await FindAsync(e => e.IsActive && e.InterestGroups.Any(f => f.InterestGroupId == id))).ToList();
        }

        public async Task<BaseOperationResponse> CreateAsync(IAccountManager accountManager, Student student, ApplicationUser user, string newPassword, List<UserCardId> cards, List<StudentCard> studentCards)
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
                        if (student.Account == null)
                        {
                            student.Account = new StudentAccount();
                        }

                        student.Account.User = user;
                        student.Account.User.UserCardIds = cards;
                    }
                    else
                    {
                        result.Message = $"Failed to save user. Errors: {string.Join(Environment.NewLine, createUserResult.Item2)}";
                        return result;
                    }

                    student.Account.User = user;
                }

                var f = await AddAsync(student);
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

        public async Task<BaseOperationResponse> UpdateAsync(IAccountManager accountManager, Student student, ApplicationUser user, string currentPassword, string newPassword, List<UserCardId> cards, List<StudentCard> studentCards, List<StudentRestriction> restrictions, List<StudentInterestGroup> interestGroups)
        {
            var result = new BaseOperationResponse();
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                                    TransactionScopeAsyncFlowOption.Enabled))
                {
                    var f = await GetSingleOrDefaultAsync(e => e.Id == student.Id);

                    if (user != null && !string.IsNullOrEmpty(user.UserName))
                    {
                        var appUser = await accountManager.GetUserByIdAsync(user.Id);

                        if (appUser != null)
                        {
                            //var appUser = await _appContext.Users.SingleOrDefaultAsync(e => e.Id == user.Id);
                            var existingUsername = await accountManager.GetUserByUserNameAsync(user.UserName);
                            if (existingUsername != null && existingUsername.Id != appUser.Id && existingUsername.IsActive)
                            {
                                //username exists
                                result.Message = "Failed to save! Username already exists.";
                                return result;
                            }

                            appUser.UserType = user.UserType;
                            appUser.UserName = user.UserName;
                            appUser.Email = user.Email;
                            appUser.IsActive = true;
                            appUser.IsEnabled = true;
                            appUser.EmailConfirmed = true;
                            if (!appUser.InstitutionId.HasValue)
                            {
                                appUser.InstitutionId = (await accountManager.GetCurrentInstitution())?.Id;
                            }

                            var updateUserResult = await accountManager.UpdateUserAsync(appUser, new List<string>());
                            if (updateUserResult.Item1)
                            {
                                if (student.Account == null)
                                {
                                    student.Account = new StudentAccount();
                                }

                                student.Account.UserId = appUser.Id;

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
                                result.Message = "Failed to save! " + updateUserResult.Item2;
                                return result;
                            }

                            student.Account.User = appUser;

                            _appContext.UserCardIds.Where(e => e.UserId == appUser.Id).ToList().ForEach(e =>
                            {
                                e.IsActive = false;
                                _appContext.UserCardIds.Update(e);
                            });

                            //add/update cards here
                            foreach (var c in cards)
                            {
                                UserCardId card;
                                if (c.Id > 0)
                                {
                                    card = await _appContext.UserCardIds.SingleOrDefaultAsync(e => e.Id == c.Id);
                                }
                                else
                                {
                                    card = await _appContext.UserCardIds.SingleOrDefaultAsync(e => e.UserId == c.UserId && e.CardId.ToLower() == c.CardId.ToLower());
                                }

                                if (card == null || card.Id == 0)
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
                            appUser.UserCardIds = cards;
                            appUser.IsActive = true;
                            if (!appUser.InstitutionId.HasValue)
                            {
                                appUser.InstitutionId = (await accountManager.GetCurrentInstitution())?.Id;
                            }
                            var existingUsername = await accountManager.GetUserByUserNameAsync(user.UserName);
                            if (existingUsername != null && existingUsername.IsActive)
                            {
                                //username exists
                                result.Message = "Failed to save! Username already exists.";
                                return result;
                            }

                            var existingUser = await accountManager.GetUserByEmailAsync(appUser.Email);
                            if (existingUser != null && existingUser.IsActive)
                            {
                                appUser = existingUser;
                            }

                            if (appUser.Id == 0)
                            {
                                var createUserResult = await accountManager.CreateUserAsync(appUser, new List<string>(), newPassword);
                                if (!createUserResult.Item1)
                                {
                                    result.Message = $"Failed to save user. Errors: {string.Join(Environment.NewLine, createUserResult.Item2)}";
                                    return result;
                                }
                            }

                            if (student.Account == null)
                            {
                                student.Account = new StudentAccount();
                            }

                            student.Account.UserId = appUser.Id;
                            student.Account.StudentId = student.Id;
                            await _appContext.StudentAccounts.AddAsync(student.Account);
                        }

                    }

                    var studentCardIds = studentCards?.Select(e => e.Id).ToList();
                    var cardsToDelete = this._appContext.StudentCards.Where(x => x.StudentId == f.Id &&
                                        (studentCardIds == null || !studentCardIds.Any(a => a == x.Id)));

                    this._appContext.StudentCards.RemoveRange(cardsToDelete);

                    if (studentCards != null)
                    {
                        studentCards.ForEach(e =>
                        {
                            var sc = this._appContext.StudentCards.FirstOrDefault(x => x.Id == e.Id);
                            if (sc != null)
                            {
                                sc.CardId = e.CardId;
                                sc.Remarks = e.Remarks;
                                sc.Status = e.Status;
                                sc.IsActive = true;
                                this._appContext.StudentCards.Update(sc);
                            }
                            else
                            {
                                this._appContext.StudentCards.Add(e);
                            }
                        });
                    }

                    var restrictionsIds = restrictions?.Select(e => e.RestrictionId).ToList();
                    var restrictionsToDelete = this._appContext.StudentRestrictions.Where(x => x.StudentId == f.Id &&
                                        (restrictionsIds == null || !restrictionsIds.Any(a => a == x.RestrictionId)));

                    this._appContext.StudentRestrictions.RemoveRange(restrictionsToDelete);

                    if (restrictions != null)
                    {
                        restrictions.ForEach(e =>
                        {
                            var sr = this._appContext.StudentRestrictions.FirstOrDefault(x => x.StudentId == e.StudentId && x.RestrictionId == e.RestrictionId);
                            if (sr != null)
                            {
                                sr.IsActive = true;
                                this._appContext.StudentRestrictions.Update(sr);
                            }
                            else
                            {
                                this._appContext.StudentRestrictions.Add(e);
                            }
                        });
                    }

                    var interestGroupsIds = interestGroups?.Select(e => e.InterestGroupId).ToList();
                    var interestGroupsToDelete = this._appContext.StudentInterestGroups.Where(x => x.StudentId == f.Id &&
                                        (interestGroupsIds == null || !interestGroupsIds.Any(a => a == x.InterestGroupId)));

                    this._appContext.StudentInterestGroups.RemoveRange(interestGroupsToDelete);

                    if (interestGroups != null)
                    {
                        interestGroups.ForEach(e =>
                        {
                            var sr = this._appContext.StudentInterestGroups.FirstOrDefault(x => x.StudentId == e.StudentId && x.InterestGroupId == e.InterestGroupId);
                            if (sr != null)
                            {
                                sr.IsActive = true;
                                this._appContext.StudentInterestGroups.Update(sr);
                            }
                            else
                            {
                                this._appContext.StudentInterestGroups.Add(e);
                            }
                        });
                    }

                    if (student.Users != null)
                    {
                        var stus = new List<StudentManageAccount>();
                        foreach (var ug in student.Users)
                        {
                            var uw = await _appContext.StudentManageAccounts.FirstOrDefaultAsync(e => e.Id == ug.Id);
                            if (uw == null) uw = ug;
                            else uw.IsActive = ug.IsActive;

                            stus.Add(uw);
                        }
                        student.Users = stus;
                    }

                    //if (user.UserOutlets != null)
                    //{
                    //    var stus = new List<UserOutlet>();
                    //    foreach (var ug in user.UserOutlets)
                    //    {
                    //        if (ug.Id == 0 && ug.IsActive == false) continue;

                    //        var uw = await _appContext.UserOutlets.FirstOrDefaultAsync(e => e.UserId == ug.UserId && e.Id == ug.Id);
                    //        if (uw == null) uw = ug;
                    //        else uw.IsActive = ug.IsActive;


                    //        stus.Add(uw);
                    //    }
                    //    user.UserOutlets = stus;
                    //}

                    f.CopyFrom(student);

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
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error saving changes: {ex.Message}", ex);
                _logger.LogError($"Error saving changes: {ex.StackTrace}", ex);

                result.Message = "Failed to save due to a database update error!";
                result.IsSuccess = false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving changes: {ex.Message}", ex);
                _logger.LogError($"Error saving changes: {ex.StackTrace}", ex);

                result.Message = "Failed to save due to an unexpected error!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> CreateAccountAsync(List<int> ids, bool generateRandomPassword, IAccountManager accountManager, string defaultPassword)
        {
            var result = new BaseOperationResponse();

            foreach (var id in ids)
            {
                var student = await GetSingleOrDefaultAsync(r => r.Id == id);
                string newPassword = generateRandomPassword ? PasswordHelper.GenerateRandomPassword() : defaultPassword;
                student.newPassword = newPassword;

                var user = new ApplicationUser();
                user.IsEnabled = true;
                user.EmailConfirmed = true;
                user.UserName = student.Email?.Substring(0, student.Email.IndexOf('@') );
                user.Email = student.Email;
                user.IsActive = true;
                user.InstitutionId = (await accountManager.GetCurrentInstitution())?.Id;
                
                var createUserResult = await accountManager.CreateUserAsync(user, new List<string>(), newPassword);
                if (createUserResult.Item1)
                {
                   if (student.Account == null)
                    {
                        student.Account = new StudentAccount();
                    }

                    user = await accountManager.GetUserByEmailAsync(student.Email);
                    student.Account.UserId = user.Id;
                    student.Account.StudentId = student.Id;
                    await _appContext.StudentAccounts.AddAsync(student.Account);
                }
                else
                {
                    result.Message = $"Failed to create account. Errors: {string.Join(Environment.NewLine, createUserResult.Item2)}";
                    return result;
                }
            }

            await _appContext.SaveChangesAsync();

            result.Message = "Successfully create account!";
            result.IsSuccess = true;
            return result;
        }

        public async Task<BaseOperationResponse> DeleteAsync(int studentId)
        {
            var result = new BaseOperationResponse();
            var student = await GetSingleOrDefaultAsync(r => r.Id == studentId);

            if (student != null)
            {
                if (student?.Users.Any() ?? false)
                {
                    foreach (var item in student.Users)
                        item.IsActive = false;
                }
                return await Delete(student);
            }

            result.IsSuccess = false;
            result.Message = "Student not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Student student)
        {
            var result = new BaseOperationResponse();
            SoftDelete(student);
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

        public async Task<BaseOperationResponse> UpdateStudentEmail(int id, string email)
        {
            var result = new BaseOperationResponse();
            var student = await GetFirstOrDefaultAsync(r => r.Id == id);

            if(student != null)
            {
                student.Email = email;
                Update(student);

                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                }
                else
                {
                    result.Message = "Failed to save!";
                    result.IsSuccess = false;
                }
            }
            else
            {
                result.Message = "Student not found.";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> ImportStudentAsync(IAccountManager accountManager, List<StudentImportDTO> rows)
        {
            var result = new BaseOperationResponse();
            try
            {
                if (rows.Count > 0)
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = new System.TimeSpan(24, 0, 0) },
                            TransactionScopeAsyncFlowOption.Enabled))
                    {
                        List<int> studentIds = new List<int>();

                        foreach (var row in rows)
                        {
                            var className = row.Class?.ToLower() ?? "";
                            var sName = row.Name?.ToLower() ?? "";

                            var sClass = await _appContext.Classes.FirstOrDefaultAsync(e => e.IsActive && e.Name.ToLower() == className);
                            if (sClass == null)
                            {
                                throw new Exception(string.Format("Class not found. Please check the imported file."));
                            }

                            var batch = await _appContext.ClassBatches.FirstOrDefaultAsync(e => e.IsActive && e.Name.Equals(row.Batch, StringComparison.InvariantCultureIgnoreCase));

                            var student = await _appContext.Students.FirstOrDefaultAsync(e => e.IsActive &&
                                                e.Name.ToLower() == sName);

                            ApplicationUser user = null;
                            //check if student exists using account
                            if (student == null)
                            {
                                if (!string.IsNullOrEmpty(row.Email) && new EmailAddressAttribute().IsValid(row.Email))
                                {
                                    user = await accountManager.GetUserByEmailAsync(row.Email);
                                    if (user != null && user.Account != null)
                                    {
                                        student = await _appContext.Students.FirstOrDefaultAsync(e => e.IsActive && e.Id == user.Account.StudentId);
                                    }
                                }
                            }

                            if (student != null)
                            {
                                //update
                                student.Name = row.Name;
                                student.ClassBatchId = batch?.Id;
                                student.ClassId = sClass.Id;
                                student.Gender = row.Gender;
                                student.Email = row.Email;
                                student.IsFAS = row.IsFAS;
                                student.Weight = row.Weight;
                                student.Height = row.Height;

                                //check if account exists
                                if (user == null && !string.IsNullOrEmpty(row.Email) && new EmailAddressAttribute().IsValid(row.Email))
                                {
                                    //create account
                                    user = await accountManager.GetUserByEmailAsync(row.Email);
                                    if (user == null)
                                    {
                                        if (student.Account != null)
                                        {
                                            if (student.Account.User != null)
                                            {
                                                student.Account.User.Email = row.Email;
                                                _appContext.StudentAccounts.Update(student.Account);
                                            }
                                        }
                                        else
                                        {
                                            //user = new ApplicationUser();
                                            //user.IsEnabled = true;
                                            //user.EmailConfirmed = true;
                                            //user.UserName = row.Email.Substring(0, row.Email.IndexOf('@'));
                                            //user.Email = row.Email;
                                            //string newPassword = PasswordHelper.GenerateRandomPassword();
                                            //var createUserResult = await accountManager.CreateUserAsync(user, new List<string>(), newPassword);
                                            //if (createUserResult.Item1)
                                            //{
                                            //    if (student.Account == null)
                                            //    {
                                            //        student.Account = new StudentAccount();
                                            //    }

                                            //    student.Account.UserId = user.Id;
                                            //    student.Account.StudentId = student.Id;
                                            //    await _appContext.StudentAccounts.AddAsync(student.Account);
                                            //}
                                            //else
                                            //{
                                            //    result.Message = "Failed to create an account!";
                                            //    return result;
                                            //}
                                        }
                                    }
                                }

                                await _appContext.SaveChangesAsync();
                                studentIds.Add(student.Id);
                            }
                            else
                            {
                                //no student record and no account yet
                                student = new Student
                                {
                                    ClassBatchId = batch?.Id,
                                    ClassId = sClass.Id,
                                    Name = row.Name,
                                    Gender = row.Gender,
                                    Email = row.Email,
                                    IsFAS = row.IsFAS,
                                    Weight = row.Weight,
                                    Height = row.Height
                                };

                                if (user == null && !string.IsNullOrEmpty(row.Email) && new EmailAddressAttribute().IsValid(row.Email))
                                {
                                    //create account
                                    user = await accountManager.GetUserByEmailAsync(row.Email);
                                    if (user == null)
                                    {
                                        //user = new ApplicationUser();
                                        //user.IsEnabled = true;
                                        //user.EmailConfirmed = true;
                                        //user.UserName = row.Email.Substring(0, row.Email.IndexOf('@'));
                                        //user.Email = row.Email;
                                        //user.IsActive = true;
                                        //user.InstitutionId = (await accountManager.GetCurrentInstitution())?.Id;
                                        //string newPassword = PasswordHelper.GenerateRandomPassword();
                                        //var createUserResult = await accountManager.CreateUserAsync(user, new List<string>(), newPassword);
                                        //if (createUserResult.Item1)
                                        //{
                                        //    if (student.Account == null)
                                        //    {
                                        //        student.Account = new StudentAccount();
                                        //    }

                                        //    student.Account.User = user;
                                        //}
                                        //else
                                        //{
                                        //    result.Message = "Failed to create an account!";
                                        //    return result;
                                        //}
                                    }
                                }

                                var stud = await AddAsync(student);
                                await _appContext.SaveChangesAsync();
                                studentIds.Add(stud.Id);
                            }
                        }

                        //disable removed students
                        var studentsToDisable = _appContext.Students.Where(e => studentIds.All(f => f != e.Id));

                        foreach (var stud in studentsToDisable)
                        {
                            stud.IsActive = false;
                            Update(stud);
                            await _appContext.SaveChangesAsync();
                        }

                        scope.Complete();
                        result.IsSuccess = true;
                        result.Message = "File Imported!";
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }



            return result;
        }

        public async Task<BaseOperationResponse> ImportStudentCardAsyncDeprecated(IAccountManager accountManager, List<StudentCardImportDTO> rows)
        {
            var result = new BaseOperationResponse();
            try
            {
                if (rows.Count > 0)
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = new System.TimeSpan(24, 0, 0) },
                            TransactionScopeAsyncFlowOption.Enabled))
                    {
                        List<int> studentIds = new List<int>();

                        var allClasses = _appContext.Classes.Where(e => e.IsActive).ToList();
                        var classesToImport = rows.Select(e => e.Class).ToList();
                        // check if one of the classes is missing
                        var classExist = allClasses.Any(e => classesToImport.Any(c => e.Name.Equals(c, StringComparison.InvariantCultureIgnoreCase)));
                        if (!classExist)
                        {
                            throw new Exception(string.Format("Class not found. Please check the imported file."));
                        }

                        var allBatches = _appContext.ClassBatches.Where(e => e.IsActive).ToList();
                        var batchesToImport = rows.Select(e => e.Batch).ToList();
                        // check if one of the batches is missing
                        var batchExist = allBatches.Any(e => batchesToImport.Any(c => e.Year == c));
                        if (!batchExist)
                        {
                            throw new Exception(string.Format("Batch not found. Please check the imported file."));
                        }

                        var allClassLevels = _appContext.ClassLevels.Where(e => e.IsActive).ToList();

                        var allStudents = _appContext.Students.Where(e => e.IsActive && e.OutletId == rows.First().OutletId).ToList();
                        var allStudentCards = _appContext.StudentCards.Where(e => e.IsActive).ToList();
                        var allUsers = _appContext.Users.Where(e => e.IsActive);
                        var allAsociatedEmails = rows.Where(e => !string.IsNullOrEmpty(e.AssociatedEmail)).Select(e => e.AssociatedEmail);

                        var parentEmails = allAsociatedEmails.Select(email => email.ToLower()).ToList();
                        var allParentAccounts = allUsers
                            .Where(e => parentEmails.Contains(e.Email.ToLower()))
                            .ToList();

                        foreach (var row in rows)
                        {
                            var sClass = allClasses.FirstOrDefault(e => e.Name.Equals(row.Class, StringComparison.InvariantCultureIgnoreCase));
                            if (sClass == null)
                            {
                                throw new Exception(string.Format("Class not found. Please check the imported file."));
                            }

                            var classLevel = allClassLevels.FirstOrDefault(e => e.Id == sClass.ClassLevelId);

                            //class level not provided, create one from class name
                            if (classLevel == null)
                            {
                                classLevel = new ClassLevel { Name = row.Class, Year = DateTime.Now.Year, OutletId = row.OutletId };
                                _appContext.ClassLevels.Add(classLevel);
                                //await _appContext.SaveChangesAsync();
                            }

                            var batch = allBatches.FirstOrDefault(e => e.Year == row.Batch);

                            //batch not provided, create one from class name
                            if (batch == null)
                            {
                                batch = new ClassBatch { Name = row.Batch.ToString(), Year = row.Batch };
                                _appContext.ClassBatches.Add(batch);
                                //await _appContext.SaveChangesAsync();
                            }

                            var students = allStudents.Where(e => e.Name.Equals(row.Name, StringComparison.InvariantCultureIgnoreCase));

                            var associatedEmails = !string.IsNullOrEmpty(row.AssociatedEmail)
                                                ? row.AssociatedEmail.Split(',').Select(e => e.Trim().ToLower()).ToList()
                                                : new List<string>();

                            var parentAccounts = allParentAccounts
                                .Where(e => associatedEmails.Contains(e.Email.ToLower()))
                                .ToList();

                            var studentNameIds = students.Select(e => e.Id).ToList();
                            Student student = parentAccounts?.SelectMany(f => f.Students)?.FirstOrDefault(e => studentNameIds.Contains(e.StudentId))?.Student;

                            if (student == null)
                            {
                                // existing student with their own email
                                student = students.FirstOrDefault(e => !string.IsNullOrEmpty(e.Email) && e.Email.Equals(row.Email, StringComparison.InvariantCultureIgnoreCase));
                            }

                            if (student == null)
                            {
                                if (students.Any(e => !string.IsNullOrEmpty(e.Email) && associatedEmails.Any(x => e.Email.Equals(x, StringComparison.InvariantCultureIgnoreCase))))
                                {
                                    // student is existing and is using parent's email
                                    student = students.FirstOrDefault(e => !string.IsNullOrEmpty(e.Email) && associatedEmails.Any(x => e.Email.Equals(x, StringComparison.InvariantCultureIgnoreCase)));
                                }
                            }

                            string studentEmail = string.IsNullOrEmpty(row.Email) ? await GenerateStudentEmail(sClass.Name, row.Name, row.OutletId) : row.Email;

                            //check if student exists using account
                            if (student == null)
                            {
                                if (!string.IsNullOrEmpty(studentEmail) && new EmailAddressAttribute().IsValid(studentEmail))
                                {
                                    student = allStudents.FirstOrDefault(e => e.OutletId == row.OutletId && e.IsActive && e.Email == studentEmail);
                                }
                            }

                            if (student != null)
                            {
                                //update
                                student.Name = row.Name;
                                student.ClassBatchId = batch?.Id;
                                student.ClassId = sClass.Id;
                                student.Email = student.Email ?? studentEmail;

                                var studentCards = allStudentCards.Where(e => e.IsActive && e.StudentId == student.Id);
                                //disable existing cards
                                var cardToDelete = studentCards.Where(e => !(e.CardId.ToLower() == row.CardId.ToLower()));

                                foreach (var e in cardToDelete)
                                {
                                    e.IsActive = false;
                                    _appContext.StudentCards.Update(e);
                                }

                                var cardToInsert = allStudentCards.FirstOrDefault(e => e.CardId.ToLower() == row.CardId.ToLower());
                                if (cardToInsert == null)
                                {
                                    _appContext.StudentCards.Add(new StudentCard
                                    {
                                        StudentId = student.Id,
                                        CardId = row.CardId,
                                        Remarks = row.CardNumber,
                                        Status = StudentCardStatus.ACTIVE.ToString()
                                    });
                                }
                                else
                                {
                                    cardToInsert.Status = StudentCardStatus.ACTIVE.ToString();
                                    cardToInsert.Remarks = row.CardNumber;
                                    _appContext.StudentCards.Update(cardToInsert);
                                }

                                Update(student);


                                //await _appContext.SaveChangesAsync();
                                //studentIds.Add(student.Id);
                            }
                            else
                            {
                                //no student record and no account yet
                                var studentCards = allStudentCards.Where(e => e.StudentId == student.Id);

                                student = new Student
                                {
                                    ClassBatchId = batch?.Id,
                                    ClassId = sClass.Id,
                                    Name = row.Name,
                                    Email = studentEmail,
                                    OutletId = row.OutletId,
                                    StudentCards = new List<StudentCard>(),
                                    Users = new List<StudentManageAccount>()
                                };

                                // parent email exists
                                if (parentAccounts != null)
                                {
                                    foreach (var parentAccount in parentAccounts)
                                        student.Users.Add(new StudentManageAccount { UserId = parentAccount.Id });
                                }
                                else
                                {
                                    // create account
                                    if (associatedEmails != null && associatedEmails.Any())
                                    {
                                        foreach (var associatedEmail in associatedEmails)
                                        {
                                            //create account
                                            var user = await accountManager.GetUserByEmailAsync(associatedEmail);
                                            if (user == null)
                                            {
                                                user = new ApplicationUser();
                                                user.IsEnabled = true;
                                                user.EmailConfirmed = true;
                                                user.UserName = associatedEmail.Substring(0, associatedEmail.IndexOf('@'));
                                                user.Email = associatedEmail;
                                                user.IsActive = true;
                                                string newPassword = PasswordHelper.GenerateRandomPassword();
                                                var createUserResult = await accountManager.CreateUserWithPasswordAsync(user, new List<string>(), newPassword);
                                                if (createUserResult.Item1)
                                                {
                                                    student.Users.Add(new StudentManageAccount { UserId = user.Id });
                                                }
                                            }
                                            else
                                            {
                                                student.Users.Add(new StudentManageAccount { UserId = user.Id });
                                            }
                                        }
                                    }
                                }

                                student.StudentCards.Add(new StudentCard
                                {
                                    CardId = row.CardId,
                                    Remarks = row.CardNumber,
                                    Status = StudentCardStatus.ACTIVE.ToString()
                                });

                                var stud = await AddAsync(student);
                                //await _appContext.SaveChangesAsync();
                                //studentIds.Add(stud.Id);
                            }
                        }

                        await _appContext.SaveChangesAsync();
                        //disable removed students
                        //var studentsToDisable = _appContext.Students.Where(e => e.OutletId == rows.First().OutletId && studentIds.All(f => f != e.Id));

                        //foreach (var stud in studentsToDisable)
                        //{
                        //    stud.IsActive = false;
                        //    Update(stud);
                        //    await _appContext.SaveChangesAsync();
                        //}

                        scope.Complete();
                        result.IsSuccess = true;
                        result.Message = "File Imported!";
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }



            return result;
        }

        public async Task<BaseOperationResponse> ImportStudentCardAsync(IAccountManager accountManager, List<StudentCardImportDTO> rows)
        {
            var result = new BaseOperationResponse();
            try
            {
                if (rows.Count > 0)
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = new System.TimeSpan(24, 0, 0) },
                            TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var hashedPassword = _passwordHasher.HashPassword(null, _configuration["AppSettings:ONBOARDING_DEFAULT_PASSWORD"]);

                        if (rows.Count > 0)
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Columns.Add("OutletId", typeof(int));
                            dataTable.Columns.Add("BatchName", typeof(string));
                            dataTable.Columns.Add("ClassName", typeof(string));
                            dataTable.Columns.Add("IssueDate", typeof(DateTime));
                            dataTable.Columns.Add("CardId", typeof(string));
                            dataTable.Columns.Add("CardNumber", typeof(string));
                            dataTable.Columns.Add("Name", typeof(string));
                            dataTable.Columns.Add("Email", typeof(string));
                            dataTable.Columns.Add("AssociatedEmail", typeof(string));
                            dataTable.Columns.Add("InstitutionId", typeof(int));
                            dataTable.Columns.Add("AltEmail", typeof(string));
                            dataTable.Columns.Add("CreatedBy", typeof(int));
                            dataTable.Columns.Add("PasswordHashed", typeof(string));
                            dataTable.Columns.Add("SecurityStamp", typeof(string));
                            dataTable.Columns.Add("ConcurrencyStamp", typeof(string));


                            foreach (var row in rows)
                            {
                                dataTable.Rows.Add(row.OutletId, row.Batch.ToString(), row.Class, row.IssueDate, row.CardId, row.CardNumber, row.Name, row.Email, row.AssociatedEmail, _currentInstitutionId ?? _appContext.CurrentInstitutionId,
                                    string.IsNullOrEmpty(row.Email) ? await GenerateStudentEmail(row.Class, row.Name, row.OutletId) : row.Email, _currentUserId ?? _appContext.CurrentUserId,
                                    hashedPassword, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                            }


                            await _appContext.Database.ExecuteSqlRawAsync(
                                "EXEC dbo.ImportStudentData @StudentData",
                                new SqlParameter("@StudentData", SqlDbType.Structured)
                                {
                                    TypeName = "dbo.tvpStudents",
                                    Value = dataTable
                                }
                            );
                        }


                        scope.Complete();
                        result.IsSuccess = true;
                        result.Message = "File Imported!";
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }



            return result;
        }

        private async Task<string> GenerateStudentEmail(string className, string name, int outletId)
        {
            int classCount = _appContext.Students.Count(e => e.IsActive && e.Class.Name.ToLower() == className.ToLower());
            int retries = 20;

            //replace non a
            Regex rgx = new Regex("[^a-zA-Z -]");
            name = rgx.Replace(name, "");
            string[] names = name.ToLower().Split(' ');
            string email = string.Join("", names.Where(e => !string.IsNullOrEmpty(e)).Select(e => e));
            email = email.Remove(email.Length - 1);
            string originalEmail = string.Format("{0}{1}@example.org", email, names.Last());
            string studentEmail = originalEmail;
            while (retries > 0)
            {
                var exists = await _appContext.Students
                                .FirstOrDefaultAsync(e => e.OutletId == outletId
                                                        && e.IsActive
                                                        && e.Email != null
                                                        && e.Email.ToLower() == studentEmail.ToLower());
                if (exists != null)
                {
                    studentEmail = string.Format("{0}{1}", originalEmail, ++classCount);
                }
                else
                {
                    break;
                }

                retries--;
            }

            return studentEmail;
        }

        public async Task<List<Student>> GetStudentsLiteByIdsAsync(List<int> studentIds)
        {
            IQueryable<Student> query = _appContext.Students.AsNoTracking().Where(x => x.IsActive && studentIds.Contains(x.Id));
            var result = await query.ToListAsync();
            return result;
        }

        public async Task<bool> ImportStudentGroupAsync(List<int> studentIds,int studentGroupId,int userId)
        {
            try
            {
                var selectedStudentGroupDetail = await _appContext.StudentGroupDetails.AsNoTracking().Where(x => x.StudentGroupId == studentGroupId).Select(x => x.StudentId).ToListAsync();

                var selectedStudents = await _appContext.Students.AsNoTracking().Where(x => studentIds.Contains(x.Id) && x.IsActive).ToListAsync();
                foreach (var item in selectedStudents)
                {
                    var isExist = selectedStudentGroupDetail.Any(x => x == item.Id);
                    if (!isExist)
                    {
                        _appContext.StudentGroupDetails.Add(new StudentGroupDetail
                        {
                            StudentGroupId = studentGroupId,
                            StudentId = item.Id,
                            CreatedBy = userId,
                            UpdatedBy = userId
                        });
                    }
                }

                await _appContext.SaveChangesAsync();
                return true;
            }catch(Exception)
            {
                throw;
            }
        }

        public async Task<BaseOperationResponse> UpdateStudentClassByClassIdAsync(Student data,int originClassId)
        {
            var result = new BaseOperationResponse();

            try
            {
                int classId = originClassId;
                int destinationClassId = data?.ClassId ?? 0;
                int classLevelId = data?.ClassLevelId ?? 0;
                int batchId = data?.ClassBatchId ?? 0;

                var studentData = await _appContext.Students
                    .Where(u => u.IsActive && u.ClassId == classId && u.ClassBatchId == batchId)
                    .ToListAsync();

                if (studentData.Count == 0)
                {
                    result.IsSuccess = false;
                    result.Message = "No students to transfer from the class";
                    return result;
                }

                studentData.ForEach(x =>
                {
                    x.ClassId = destinationClassId;
                    x.ClassLevelId = classLevelId;
                });

                _appContext.Students.UpdateRange(studentData);
                await _appContext.SaveChangesAsync();

                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            catch (Exception)
            {
                result.IsSuccess = false;
                result.Message = "Failed to transfer the class";
                throw;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }

    //public static class IQueryableExtensions
    //{
    //    private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

    //    private static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");

    //    private static readonly FieldInfo QueryModelGeneratorField = QueryCompilerTypeInfo.DeclaredFields.First(x => x.Name == "_queryModelGenerator");

    //    private static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");

    //    private static readonly PropertyInfo DatabaseDependenciesField = typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

    //    public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
    //    {
    //        var queryCompiler = (QueryCompiler)QueryCompilerField.GetValue(query.Provider);
    //        var modelGenerator = (QueryModelGenerator)QueryModelGeneratorField.GetValue(queryCompiler);
    //        var queryModel = modelGenerator.ParseQuery(query.Expression);
    //        var database = (IDatabase)DataBaseField.GetValue(queryCompiler);
    //        var databaseDependencies = (DatabaseDependencies)DatabaseDependenciesField.GetValue(database);
    //        var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
    //        var modelVisitor = (RelationalQueryModelVisitor)queryCompilationContext.CreateQueryModelVisitor();
    //        modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
    //        var sql = modelVisitor.Queries.First().ToString();

    //        return sql;
    //    }
    //}
}
