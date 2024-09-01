using DAL;
using DAL.Core.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DAL.Core;
using DAL.Core.DTO;
using Microsoft.Extensions.Configuration;
using System.DirectoryServices;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Net.Http;
using DAL.Filters;
using Sieve.Services;
using DAL.Models.MealOrder;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using AutoMapper;
using OpenIddict.Abstractions;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace DAL.Core
{
    public class AccountManager : IAccountManager
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;
        private readonly IConfiguration _configuration;
        private ISieveProcessor _sieveProcessor;

        public AccountManager(
            ApplicationDbContext context,
            ApplicationUserManager userManager,
            ApplicationRoleManager roleManager,
            IHttpContextAccessor httpAccessor,
            IConfiguration configuration,
            ISieveProcessor sieveProcessor)
        {
            _context = context;
            if (httpAccessor.HttpContext != null && httpAccessor.HttpContext.User != null)
            {
                _context.CurrentUserId = Convert.ToInt32(httpAccessor.HttpContext?.User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value?.Trim());
            }
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _sieveProcessor = sieveProcessor;
        }

        #region Sieved
        public async Task<PagedEntity<ApplicationUser>> GetUsersAsync(BaseFilter filter)
        {
            IQueryable<ApplicationUser> query = _appContext.Users
                .Include(e => e.Roles)
                //.Include(u => u.UserPhonebooks)
                //.Include(u => u.UserVehicles)
                //.Include(u => u.UserGroupMembers)
                .Include(u => u.Icon);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            //query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            //int totalCount = query.Count();

            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            //var result = new PagedEntity<ApplicationUser>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync().ConfigureAwait(false)
            //};

            return result;
        }

        public async Task<PagedEntity<ApplicationRole>> GetRolesAsync(BaseFilter filter)
        {
            IQueryable<ApplicationRole> query = _appContext.Roles;

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();

            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<ApplicationRole>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = await query.ToListAsync().ConfigureAwait(false)
            };

            return result;
        }
        #endregion


        public async Task<ApplicationUser> GetUserByIdAsync(int userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<ApplicationUser> GetUserByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<Institution> GetCurrentInstitution()
        {
            return await _userManager.GetCurrentInstitution();
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }


        public async Task<Tuple<ApplicationUser, string[]>> GetUserAndRolesAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.UserPhonebooks)
                .Include(u => u.UserVehicles)
                .Include(u => u.UserCardIds)
                .Include(u => u.Icon)
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync();

            if (user == null)
                return null;

            var userRoleIds = user.Roles.Select(r => r.RoleId).ToList();

            var roles = await _context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToArrayAsync();

            return Tuple.Create(user, roles);
        }


        public async Task<List<Tuple<ApplicationUser, string[]>>> GetUsersAndRolesAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<ApplicationUser> usersQuery = _context.Users
                .Include(u => u.Roles)
                .Include(u => u.UserPhonebooks)
                .Include(u => u.UserVehicles)
                .Include(u => u.UserCardIds)
                .Include(u => u.Icon)
                .Where(u => u.IsActive && (!institutionId.HasValue || u.InstitutionId == institutionId))
                .OrderBy(u => u.UserName);

            if (page != -1)
                usersQuery = usersQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                usersQuery = usersQuery.Take(pageSize);

            var users = await usersQuery.ToListAsync();

            var userRoleIds = users.SelectMany(u => u.Roles.Select(r => r.RoleId)).ToList();

            var roles = await _context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .ToArrayAsync();

            return users.Select(u => Tuple.Create(u,
                roles.Where(r => u.Roles.Select(ur => ur.RoleId).Contains(r.Id)).Select(r => r.Name).ToArray()))
                .ToList();
        }


        public async Task<Tuple<bool, string[]>> CreateUserAsync(ApplicationUser user, IEnumerable<string> roles, string password, bool findByEmail = true)
        {
            var existingUser = findByEmail ? await _userManager.FindByEmailAsync(user.Email) : await _userManager.FindByNameAsync(user.UserName);

            if (existingUser != null)
            {
                //existingUser.DepartmentId = user.DepartmentId;
                //existingUser.Email = user.Email;
                //existingUser.EmailConfirmed = user.EmailConfirmed;
                //existingUser.EmployeeId = user.EmployeeId;
                //existingUser.FullName = user.FullName;
                //existingUser.HomeNo = user.HomeNo;
                //existingUser.IsEnabled = user.IsEnabled;
                //existingUser.PhoneNumber = user.PhoneNumber;
                //existingUser.Pin = user.Pin;
                //existingUser.RegisteredDepartment = user.RegisteredDepartment;
                //existingUser.RfId = user.RfId;
                //existingUser.TelNo = user.TelNo;
                //existingUser.UnitNumber = user.UnitNumber;
                //existingUser.UserName = user.UserName;
                existingUser.CopyFrom(user);
                existingUser.IsActive = true;
                if (existingUser.IsAD) existingUser.EmailConfirmed = true;
                return await UpdateUserAsync(existingUser, roles);
            }
            else
            {
                existingUser = await _appContext.Users.SingleOrDefaultAsync(e => e.Email == user.Email && e.InstitutionId == user.InstitutionId);
            }

            if(existingUser == null)
            {
                existingUser = user;
            }

            if (existingUser.IsAD) existingUser.EmailConfirmed = true;

            var result = existingUser.IsAD ? await _userManager.CreateAsync(existingUser) : await _userManager.CreateAsync(existingUser, password);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());


            existingUser = await _userManager.FindByNameAsync(user.UserName);

            if(existingUser == null)
            {
                existingUser = await _appContext.Users.SingleOrDefaultAsync(e => e.Email == user.Email && e.InstitutionId == user.InstitutionId);
            }

            if (roles != null && roles.Any())
            {
                try
                {
                    result = await this._userManager.AddToRolesAsync(user, roles.Distinct());
                }
                catch
                {
                    await DeleteUserAsync(existingUser);
                    throw;
                }
            }
            else
            {
                var defaultRole = await _appContext.Roles.FirstOrDefaultAsync(e => e.IsActive && e.IsDefault);
                try
                {
                    result = await this._userManager.AddToRolesAsync(user, new List<string> { defaultRole.Name });
                }
                catch
                {
                }
            }

            //check if user has a wallet
            try
            {
                var wallet = await this._appContext.Wallets.FirstOrDefaultAsync(e => e.IsActive && e.UserId == existingUser.Id);
                if (wallet == null)
                {
                    wallet = new Wallet { UserId = existingUser.Id, Balance = 0 };
                    await this._appContext.Wallets.AddAsync(wallet);
                    await _appContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
            }

            //check if user has reward
            try
            {
                var reward = await this._appContext.Rewards.FirstOrDefaultAsync(e => e.IsActive && e.UserId == existingUser.Id);
                if (reward == null)
                {
                    reward = new Reward { UserId = existingUser.Id, Balance = 0 };
                    await this._appContext.Rewards.AddAsync(reward);
                    await _appContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
            }

            if (!result.Succeeded)
            {
                await DeleteUserAsync(existingUser);
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
            }

            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> CreateUserWithPasswordAsync(ApplicationUser user, IEnumerable<string> roles, string password)
        {
            if (user.IsAD) user.EmailConfirmed = true;

            var result = user.IsAD ? await _userManager.CreateAsync(user) : await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());


            if (roles != null && roles.Any())
            {
                try
                {
                    result = await this._userManager.AddToRolesAsync(user, roles.Distinct());
                }
                catch
                {
                    await DeleteUserAsync(user);
                    throw;
                }
            }
            else
            {
                var defaultRole = await _appContext.Roles.FirstOrDefaultAsync(e => e.IsActive && e.IsDefault);
                try
                {
                    result = await this._userManager.AddToRolesAsync(user, new List<string> { defaultRole.Name });
                }
                catch
                {
                }
            }

            if (!result.Succeeded)
            {
                await DeleteUserAsync(user);
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
            }

            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> UpdateUserAsync(ApplicationUser user)
        {
            return await UpdateUserAsync(user, null);
        }


        public async Task<Tuple<bool, string[]>> UpdateUserAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            if (user.UserVehicles != null)
            {
                var userVehicles = new List<UserVehicle>();
                foreach (var v in user.UserVehicles)
                {
                    var vehicle = await _appContext.UserVehicles.FirstOrDefaultAsync(e => e.Id == v.Id);
                    if (vehicle == null)
                    {
                        vehicle = v;
                        vehicle.VehicleStatus = VehicleStatus.PENDING.ToString();
                    }

                    userVehicles.Add(vehicle);
                }
                user.UserVehicles = userVehicles;
            }

            if (user.UserCardIds != null)
            {
                var userCardIds = new List<UserCardId>();
                foreach (var c in user.UserCardIds)
                {
                    var cardId = await _appContext.UserCardIds.FirstOrDefaultAsync(e => e.Id == c.Id);
                    if (cardId != null)
                    {
                        cardId.CardId = c.CardId;
                        if (c.Status != null)
                        {
                            cardId.Status = c.Status;
                        }
                        else
                        {
                            cardId.Status = CardIdStatus.INACTIVE.ToString();
                        }
                        cardId.Remarks = c.Remarks;
                    } else
                    {
                        cardId = c;
                    }

                    userCardIds.Add(cardId);
                }
                user.UserCardIds = userCardIds;
            }

            if (user.UserGroupMembers != null)
            {
                var userGroupMembers = new List<UserGroupMember>();
                foreach (var ug in user.UserGroupMembers)
                {
                    var userGroup = await _appContext.UserGroupMembers.FirstOrDefaultAsync(e => e.UserId == ug.UserId && e.UserGroupId == ug.UserGroupId);
                    if (userGroup == null)
                    {
                        userGroup = ug;
                    }

                    userGroupMembers.Add(userGroup);
                }
                user.UserGroupMembers = userGroupMembers;
            }

            if (user.Icon != null)
            {
                var icon = await _appContext.Files.SingleOrDefaultAsync(e => e.Id == user.FileId);
                if (icon != null)
                {
                    user.Icon = icon;
                    user.FileId = icon.Id;
                }

                user.Icon.FileName = System.IO.Path.GetFileName(user.Icon.Path);
                user.Icon.Type = FileType.Icon.ToString();
            }

            if (!user.UserWallets.Any())
            {
                //check if user has a wallet
                var wallet = await this._appContext.Wallets.FirstOrDefaultAsync(e => e.IsActive && e.UserId == user.Id);
                if (wallet == null)
                {
                    wallet = new Wallet { UserId = user.Id, Balance = 0 };
                    user.UserWallets.Add(wallet);
                }
            }
            else
            {
                if (user.UserWallets != null)
                {
                    var userWallets = new List<Wallet>();
                    foreach (var ug in user.UserWallets)
                    {
                        var uw = await _appContext.Wallets.FirstOrDefaultAsync(e => e.UserId == ug.UserId && e.Id == ug.Id);
                        if (uw == null)
                        {
                            uw = ug;
                        }

                        userWallets.Add(uw);
                    }
                    user.UserWallets = userWallets;
                }
            }

            if (!user.UserRewards.Any())
            {
                //check if user has a wallet
                var reward = await this._appContext.Rewards.FirstOrDefaultAsync(e => e.IsActive && e.UserId == user.Id);
                if (reward == null)
                {
                    reward = new Reward { UserId = user.Id, Balance = 0 };
                    user.UserRewards.Add(reward);
                }
            }
            else
            {
                if (user.UserRewards != null)
                {
                    var userRewards = new List<Reward>();
                    foreach (var ug in user.UserRewards)
                    {
                        var uw = await _appContext.Rewards.FirstOrDefaultAsync(e => e.UserId == ug.UserId && e.Id == ug.Id);
                        if (uw == null)
                        {
                            uw = ug;
                        }

                        userRewards.Add(uw);
                    }
                    user.UserRewards = userRewards;
                }
            }

            if (user.Students != null)
            {
                var stus = new List<StudentManageAccount>();
                foreach (var ug in user.Students)
                {
                    if (ug.Id == 0 && ug.IsActive == false) continue;

                    var uw = await _appContext.StudentManageAccounts.FirstOrDefaultAsync(e => e.UserId == ug.UserId && e.Id == ug.Id);
                    if (uw == null) uw = ug;
                    else uw.IsActive = ug.IsActive;
                    

                    stus.Add(uw);
                }
                user.Students = stus;
            }

            if (user.UserOutlets != null)
            {
                var stus = new List<UserOutlet>();
                foreach (var ug in user.UserOutlets)
                {
                    var uw = await _appContext.UserOutlets.FirstOrDefaultAsync(e => e.UserId == ug.UserId && e.OutletId == ug.OutletId);
                    if (uw == null) uw = ug;
                    stus.Add(uw);
                }

                user.UserOutlets = stus;
            }

            if (user.UserCaterers != null)
            {
                var stus = new List<UserCaterer>();
                foreach (var ug in user.UserCaterers)
                {
                    var uw = await _appContext.UserCaterers.FirstOrDefaultAsync(e => e.UserId == ug.UserId && e.CatererId == ug.CatererId);
                    if (uw == null) uw = ug;
                    stus.Add(uw);
                }

                user.UserCaterers = stus;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());


            if (roles != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var rolesToRemove = userRoles.Except(roles).ToArray();
                var rolesToAdd = roles.Except(userRoles).Distinct().ToArray();

                if (rolesToRemove.Any())
                {
                    result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!result.Succeeded)
                        return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                }

                if (rolesToAdd.Any())
                {
                    result = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!result.Succeeded)
                        return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            //if (students != null)
            //{
            //    foreach(var s in students)
            //    {
            //        if(s.Id > 0)
            //        {
            //            var ori = user.Students.FirstOrDefault(x => x.Id == s.Id);

            //            if (ori != null) ori.IsActive = s.IsActive;
            //        } else if (s.IsActive)
            //        {

            //        }
            //    }

            //    var oriStudents = user.Students;

            //    var toRemove = oriStudents.Except(students).ToArray();
            //    var toAdd = students.Except(oriStudents).Distinct().ToArray();

            //    if (toRemove.Any())
            //    {
            //        foreach(var r in toRemove)
            //        {
            //            r.IsActive = 
            //        }
            //    }

            //    if (toAdd.Any())
            //    {
            //        result = await _userManager.AddToRolesAsync(user, rolesToAdd);
            //        if (!result.Succeeded)
            //            return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
            //    }
            //}

            return Tuple.Create(true, new string[] { });
        }


        public async Task<Tuple<bool, string[]>> ResetPasswordAsync(ApplicationUser user, string newPassword)
        {
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());

            await _userManager.AddToUsedPasswordAsync(user, newPassword);
            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> UpdatePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            if (await _userManager.IsUsedPassword(user.Id, newPassword))
            {
                return Tuple.Create(false, new string[] { "Password was previously used. "});
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());

            await _userManager.AddToUsedPasswordAsync(user, newPassword);
            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> UpdateEmailAsync(ApplicationUser user, string email)
        {
            if (await _userManager.IsUsedEmail(user.Id, email))
            {
                return Tuple.Create(false, new string[] { "Email is not available. " });
            }

            await _userManager.UpdateEmailAsync(user, email);
            return Tuple.Create(true, new string[] { });
        }

        public async Task<PagedEntity<UserReportDTO>> GetUserReportAsync(BaseFilter filter)
        {
            IQueryable<ApplicationUser> query = _appContext.Users;

            //query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            //int totalCount = query.Count();
            var queryResult = await this._sieveProcessor.GetPagedAsync(query, filter);
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            
            var result = new PagedEntity<UserReportDTO>
            {
                Filter = queryResult.Filter,
                TotalCount = queryResult.TotalCount,
                CurrentPage = queryResult.CurrentPage,
                PageSize = queryResult.PageSize,
                PagedData = queryResult.PagedData.Select(dt => new UserReportDTO
                {
                    CreatedDate = dt.CreatedDate,
                    DeletedDate = dt.DeletedDate,
                    Department = dt.Department != null ? dt.Department.Name : dt.RegisteredDepartment,
                    Designation = dt.JobTitle,
                    UserName = dt.UserName,
                    LastLoginTime = dt.LastLoginTime,
                    Status = dt.IsActive ? "Enabled" : "Deactivated",
                    UserGroup = string.Join(", ", dt.UserGroupMembers.Select(e => e.UserGroup).Where(e => e.IsActive).Select(e => e.Name)),
                    InactiveDuration = dt.LastLoginTime.HasValue && !dt.IsActive ? (int)DateTime.Now.Subtract(dt.LastLoginTime.Value).TotalDays : 0
                }).ToList()
            };

            return result;
        }

        public async Task<byte[]> GenerateUserReportXls(BaseFilter filter)
        {
            IQueryable<ApplicationUser> query = _appContext.Users;

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            var users = await query.ToListAsync();

            if (users != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Accounts");
                    var headers = new string[] { "User Name / ID", "Designation", "Department", "User Group", "Date of user creation", "Last Login Date/Time", "Inactive Duration (Days ago)", "Date of user deactivation", "Account Status" };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                    ICell cell;
                    for (var i = 0; i < headers.Length; i++)
                    {
                        cell = row.CreateCell(i);
                        cell.SetCellValue(headers[i]);
                        cell.CellStyle = borderedHeaderStyle;
                    }
                    sheet.AutoSizeColumn(0);

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    contentStyle.BorderTop = BorderStyle.Thin;
                    contentStyle.BorderBottom = BorderStyle.Thin;
                    contentStyle.BorderLeft = BorderStyle.Thin;
                    contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();
                    users.ForEach(dt =>
                    {
                        int i = 0;
                        row = sheet.CreateRow(++rowCount);

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.UserName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.JobTitle);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.Department != null ? dt.Department.Name : dt.RegisteredDepartment);
                        cell.CellStyle = contentStyle;

                        var userGroups = dt.UserGroupMembers.Select(e => e.UserGroup).Where(e => e.IsActive).Select(e => e.Name);
                        cell = row.CreateCell(i++);
                        cell.SetCellValue(string.Join(", ", userGroups));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.CreatedDate.ToString("dd/MM/yyyy hh:mm:ss tt"));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.LastLoginTime?.ToString("dd/MM/yyyy hh:mm:ss tt"));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.LastLoginTime.HasValue && !dt.IsActive ? ((int)DateTime.Now.Subtract(dt.LastLoginTime.Value).TotalDays).ToString() : "");
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.DeletedDate?.ToString("dd/MM/yyyy"));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.IsActive ? "Enabled" : "Deactivated");
                        cell.CellStyle = contentStyle;

                    });

                    #endregion

                    for (var i = 0; i < headers.Length; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<PagedEntity<RoleReportDTO>> GetRoleReportAsync(RoleReportFilter filter)
        {
            IQueryable<ApplicationRole> query = _appContext.Roles;

            if(filter.Permissions != null && filter.Permissions.Any())
            {
                query = query.Where(e => e.Claims.Select(f => f.ClaimValue).Intersect(filter.Permissions).Any());
            }

            var queryResult = await this._sieveProcessor.GetPagedAsync(query, filter);
            var allPermissions = ApplicationPermissionsTrees.AllPermissions;
            var result = new PagedEntity<RoleReportDTO>
            {
                Filter = queryResult.Filter,
                TotalCount = queryResult.TotalCount,
                CurrentPage = queryResult.CurrentPage,
                PageSize = queryResult.PageSize,
                PagedData = queryResult.PagedData.Select(dt => new RoleReportDTO
                {
                    CreatedDate = dt.CreatedDate,
                    RoleName = dt.Name,
                    RoleDescription = dt.Description,
                    LastUpdatedDate = dt.UpdatedDate,
                    Permissions = allPermissions.Where(e => dt.Claims.Any(f => f.ClaimValue == e.Value)).Select(f => f.Name).ToList()
                }).ToList()
            };

            return result;
        }

        public async Task<byte[]> GenerateRoleReportXls(RoleReportFilter filter)
        {
            //IQueryable<ApplicationRole> query = _appContext.Roles;

            //query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            //var users = await query.ToListAsync();
            var roles = await GetRoleReportAsync(filter);

            if (roles != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Roles");
                    var headers = new string[] { "S/N", "Role", "Functions", "Created Date", "Last Updated" };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                    ICell cell;
                    for (var i = 0; i < headers.Length; i++)
                    {
                        cell = row.CreateCell(i);
                        cell.SetCellValue(headers[i]);
                        cell.CellStyle = borderedHeaderStyle;
                    }
                    sheet.AutoSizeColumn(0);

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    contentStyle.BorderTop = BorderStyle.Thin;
                    contentStyle.BorderBottom = BorderStyle.Thin;
                    contentStyle.BorderLeft = BorderStyle.Thin;
                    contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();
                    roles.PagedData.ToList().ForEach(dt =>
                    {
                        int i = 0;
                        row = sheet.CreateRow(++rowCount);

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(i);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.RoleName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.PermissionNames);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.CreatedDate.ToString("dd/MM/yyyy"));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.LastUpdatedDate?.ToString("dd/MM/yyyy"));
                        cell.CellStyle = contentStyle;

                    });

                    #endregion

                    for (var i = 0; i < headers.Length; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                if (!_userManager.SupportsUserLockout)
                    await _userManager.AccessFailedAsync(user);

                return false;
            }

            return true;
        }


        public async Task<bool> TestCanDeleteUserAsync(int userId)
        {
            if (await _context.Reservations.Where(o => o.CreatedBy == userId && o.IsActive).AnyAsync())
                return false;

            //TODO: Add more if necessary

            //if (await _context.Files.Where(o => o.CreatedBy == userId).AnyAsync())
            //    return false;

            //if (await _context.Devices.Where(o => o.CreatedBy == userId).AnyAsync())
            //    return false;

            //if (await _context.Locations.Where(o => o.CreatedBy == userId).AnyAsync())
            //    return false;

            //if (await _context.Roles.Where(o => o.CreatedBy == userId).AnyAsync())
            //    return false;

            //if (await _context.Departments.Where(o => o.CreatedBy == userId).AnyAsync())
            //    return false;

            //canDelete = !await ; //Do other tests...

            return true;
        }


        public async Task<Tuple<bool, string[]>> DeleteUserAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user != null)
                return await DeleteUserAsync(user);

            return Tuple.Create(true, new string[] { });
        }


        public async Task<Tuple<bool, string[]>> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);
            return Tuple.Create(result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }


        public async Task<IEnumerable<string>> GetRolePermissionsByRoleName(List<string> roleNames)
        {
            var claims = new List<string>();
            foreach (var roleName in roleNames)
            {
                var role = await GetRoleByNameAsync(roleName);
                var roleClaims = (await _roleManager.GetClaimsAsync(role)).Where(c => c.Type == CustomClaimTypes.Permission);
                claims.AddRange(roleClaims.Select(e => e.Value));
            }
            
            return claims;
        }

        

        public async Task<ApplicationRole> GetRoleByIdAsync(int roleId)
        {
            return await _roleManager.FindByIdAsync(roleId.ToString());
        }


        public async Task<ApplicationRole> GetRoleByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }

        public async Task<List<ApplicationUser>> GetUserByClaimValueAsync(string claimValue)
        {
            return _context.Users.Where(u => u.IsActive && u.Roles.Any(r => r.Role != null && r.Role.IsActive && r.Role.Claims.Any(c => c.ClaimValue == claimValue))).ToList();
        }


        public async Task<ApplicationRole> GetRoleLoadRelatedAsync(string roleName, int? institutionId = null)
        {
            var role = await _context.Roles
                .Include(r => r.Claims)
                .Include(r => r.UserRoles)
                .Where(r => r.IsActive && r.Name == roleName && (!institutionId.HasValue || r.InstitutionId == institutionId))
                .SingleOrDefaultAsync();

            return role;
        }


        public async Task<List<ApplicationRole>> GetRolesLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<ApplicationRole> rolesQuery = _context.Roles
                .Include(r => r.Claims)
                .Include(r => r.UserRoles)
                .Where(r => r.IsActive && (!institutionId.HasValue || r.InstitutionId == institutionId));

            if (page != -1)
                rolesQuery = rolesQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                rolesQuery = rolesQuery.Take(pageSize);

            var roles = await rolesQuery.ToListAsync();

            return roles
                .OrderBy(r => r.Name).ToList();
        }


        public async Task<Tuple<bool, string[]>> CreateRoleAsync(ApplicationRole role, IEnumerable<string> claims)
        {
            if (claims == null)
                claims = new string[] { };

            string[] invalidClaims = claims.Where(c => ApplicationPermissionsTrees.GetPermissionByValue(c) == null).ToArray();
            if (invalidClaims.Any())
                return Tuple.Create(false, new[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });


            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());


            var existingRole = await _roleManager.FindByNameAsync(role.Name);

            if(existingRole == null)
            {
                existingRole = await _appContext.Roles.SingleOrDefaultAsync(e => e.Name == role.Name);
            }

            role = existingRole;

            foreach (string claim in claims.Distinct())
            {
                result = await this._roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, ApplicationPermissionsTrees.GetPermissionByValue(claim)));

                if (!result.Succeeded)
                {
                    await DeleteRoleAsync(role);
                    return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            return Tuple.Create(true, new string[] { });
        }

        public async Task<Tuple<bool, string[]>> UpdateRoleAsync(ApplicationRole role, IEnumerable<string> claims)
        {
            if (claims != null)
            {
                string[] invalidClaims = claims.Where(c => ApplicationPermissionsTrees.GetPermissionByValue(c) == null).ToArray();
                if (invalidClaims.Any())
                    return Tuple.Create(false, new[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });
            }


            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());


            if (claims != null)
            {
                var roleClaims = (await _roleManager.GetClaimsAsync(role)).Where(c => c.Type == CustomClaimTypes.Permission);
                var roleClaimValues = roleClaims.Select(c => c.Value).ToArray();

                var claimsToRemove = roleClaimValues.Except(claims).ToArray();
                var claimsToAdd = claims.Except(roleClaimValues).Distinct().ToArray();

                if (claimsToRemove.Any())
                {
                    foreach (string claim in claimsToRemove)
                    {
                        result = await _roleManager.RemoveClaimAsync(role, roleClaims.Where(c => c.Value == claim).FirstOrDefault());
                        if (!result.Succeeded)
                            return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                    }
                }

                if (claimsToAdd.Any())
                {
                    foreach (string claim in claimsToAdd)
                    {
                        result = await _roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, ApplicationPermissionsTrees.GetPermissionByValue(claim)));
                        if (!result.Succeeded)
                            return Tuple.Create(false, result.Errors.Select(e => e.Description).ToArray());
                    }
                }
            }

            return Tuple.Create(true, new string[] { });
        }


        public async Task<bool> TestCanDeleteRoleAsync(int roleId)
        {
            return !await _context.UserRoles.Where(r => r.RoleId == roleId).AnyAsync();
        }


        public async Task<Tuple<bool, string[]>> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role != null)
                return await DeleteRoleAsync(role);

            return Tuple.Create(true, new string[] { });
        }


        public async Task<Tuple<bool, string[]>> DeleteRoleAsync(ApplicationRole role)
        {
            var result = await _roleManager.DeleteAsync(role);
            return Tuple.Create(result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

        #region Phonebook
        public async Task<UserPhonebook> GetUserPhonebookByIdAsync(int userId)
        {
            return await _appContext.UserPhonebooks.FindAsync(userId);
        }

        public async Task<List<UserPhonebook>> GetUserPhonebooksLoadRelatedAsync(int page, int pageSize, int? userId = null)
        {
            IQueryable<UserPhonebook> userPhonebooksQuery = _context.UserPhonebooks
                .Include(r => r.User)
                .ThenInclude(e => e.UserVehicles)
                .Where(e => e.IsActive)
                .OrderBy(r => r.Name);

            if (userId.HasValue)
            {
                userPhonebooksQuery = userPhonebooksQuery.Where(e => e.UserId == userId);
            }

            if (page != -1)
                userPhonebooksQuery = userPhonebooksQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                userPhonebooksQuery = userPhonebooksQuery.Take(pageSize);

            var userPhonebooks = await userPhonebooksQuery.ToListAsync();

            return userPhonebooks;
        }


        public async Task<BaseOperationResponse> CreateUserPhonebookAsync(UserPhonebook userPhonebook, string filePath = null)
        {
            var result = new BaseOperationResponse();
            if (!string.IsNullOrEmpty(filePath))
            {
                userPhonebook.Icon = new File
                {
                    Path = filePath,
                    FileName = System.IO.Path.GetFileName(filePath),
                    Type = FileType.Icon.ToString()
                };
            }

            var f = await _appContext.UserPhonebooks.AddAsync(userPhonebook);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f.Entity;
            }
            else
            {
                result.Message = "Failed to save userPhonebook!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateUserPhonebookAsync(UserPhonebook userPhonebook, string filePath = null)
        {
            var result = new BaseOperationResponse();

            var f = await _appContext.UserPhonebooks.SingleOrDefaultAsync(e => e.Id == userPhonebook.Id);
            if (!string.IsNullOrEmpty(filePath))
            {
                if (f.Icon == null)
                {
                    //TODO: check why EF Core is not loading the Icon property; interim solution
                    var icon = await _appContext.Files.SingleOrDefaultAsync(e => e.Id == f.FileId);
                    if (icon == null)
                    {
                        f.Icon = new File();
                    }
                    else
                    {
                        f.Icon = icon;
                        f.FileId = icon.Id;
                    }
                }

                f.Icon.Path = filePath;
                f.Icon.FileName = System.IO.Path.GetFileName(filePath);
                f.Icon.Type = FileType.Icon.ToString();
            }

            var oldFileId = f.FileId;
            f.CopyFrom(userPhonebook);
            f.FileId = oldFileId;
            _appContext.UserPhonebooks.Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save userPhonebook!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<TestDeleteResult> TestCanDeleteUserPhonebookAsync(int userPhonebookId)
        {
            //TODO: add correct logic here, for now prohibit deletion of userPhonebook
            return new TestDeleteResult { IsDeletable = true, Message = string.Empty }; // !await _appContext.ReservationInvitees.AnyAsync(e => e == userPhonebookId);
        }


        public async Task<BaseOperationResponse> DeleteUserPhonebookAsync(int userPhonebookId)
        {
            var result = new BaseOperationResponse();
            var userPhonebook = await _appContext.UserPhonebooks.SingleOrDefaultAsync(r => r.Id == userPhonebookId);

            if (userPhonebook != null)
                return await DeleteUserPhonebook(userPhonebook);

            result.IsSuccess = false;
            result.Message = "User Phonebook not found.";
            return result;
        }

        public async Task<BaseOperationResponse> DeleteUserPhonebook(UserPhonebook userPhonebook)
        {
            var result = new BaseOperationResponse();
            userPhonebook.IsActive = false;
            _appContext.UserPhonebooks.Update(userPhonebook);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete userPhonebook!";
                result.IsSuccess = false;
            }

            return result;
        }
        #endregion

        #region Vehicle
        public async Task<UserVehicle> GetUserVehicleByIdAsync(int userId)
        {
            return await _appContext.UserVehicles.FindAsync(userId);
        }

        public async Task<List<UserVehicle>> GetUserVehiclesLoadRelatedAsync(int page, int pageSize, int? userId = null, string status = "")
        {
            IQueryable<UserVehicle> userVehiclesQuery = _context.UserVehicles
                .Include(r => r.User)
                .Where(e => e.IsActive && (string.IsNullOrEmpty(status) || status == e.VehicleStatus))
                .OrderBy(r => r.PlateNumber);

            if (userId.HasValue)
            {
                userVehiclesQuery = userVehiclesQuery.Where(e => e.UserId == userId);
            }

            if (page != -1)
                userVehiclesQuery = userVehiclesQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                userVehiclesQuery = userVehiclesQuery.Take(pageSize);

            var userVehicles = await userVehiclesQuery.ToListAsync();

            return userVehicles;
        }


        public async Task<BaseOperationResponse> CreateUserVehicleAsync(UserVehicle userVehicle)
        {
            var result = new BaseOperationResponse();
            var f = await _appContext.UserVehicles.AddAsync(userVehicle);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f.Entity;
            }
            else
            {
                result.Message = "Failed to save vehicle!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateUserVehicleAsync(UserVehicle userVehicle)
        {
            var result = new BaseOperationResponse();

            var f = await _appContext.UserVehicles.SingleOrDefaultAsync(e => e.Id == userVehicle.Id);
            bool isPending = f.VehicleStatus != VehicleStatus.APPROVED.ToString();
            f.CopyFrom(userVehicle);
            _appContext.UserVehicles.Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                if (isPending && userVehicle.VehicleStatus == VehicleStatus.APPROVED.ToString())
                {
                    //newly approved, send update to VMS
                    string format = "yyyy-MM-dd HH:mm";
                    string maxDate = "2038-01-01 23:59";
                    await RegisterVehicleToVMS(new List<VMSVehiclePostRequestModel>
                    {
                         new VMSVehiclePostRequestModel
                         {
                              expiryDate = f.ExpiryDate.HasValue && f.ExpiryDate.Value != DateTime.MinValue ? f.ExpiryDate.Value.ToString(format) : maxDate,
                              issueDate = f.IssueDate.HasValue && f.IssueDate.Value != DateTime.MinValue ? f.IssueDate.Value.ToString(format) : DateTime.Now.ToString(format),
                              personName = f.User != null ? f.User.FriendlyName : string.Empty,
                              vehicle = f.PlateNumber,
                              seasonId = VehicleSeasonType.S.ToString () + "_" + f.Id.ToString(),
                              cardType = !string.IsNullOrEmpty(f.CardType) ? f.CardType : VehicleCardType.STAFF.ToString(),
                         }
                    });
                }

                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save vehicle!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<TestDeleteResult> TestCanDeleteUserVehicleAsync(int userVehicleId)
        {
            //TODO: add correct logic here, for now prohibit deletion of userVehicle
            return new TestDeleteResult { IsDeletable = true, Message = string.Empty }; // !await _appContext.ReservationInvitees.AnyAsync(e => e == userVehicleId);
        }


        public async Task<BaseOperationResponse> DeleteUserVehicleAsync(int userVehicleId)
        {
            var result = new BaseOperationResponse();
            var userVehicle = await _appContext.UserVehicles.SingleOrDefaultAsync(r => r.Id == userVehicleId);

            if (userVehicle != null)
                return await DeleteUserVehicle(userVehicle);

            result.IsSuccess = false;
            result.Message = "User Vehicle not found.";
            return result;
        }

        public async Task<BaseOperationResponse> DeleteUserVehicle(UserVehicle userVehicle)
        {
            var result = new BaseOperationResponse();
            userVehicle.IsActive = false;
            _appContext.UserVehicles.Update(userVehicle);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                //newly deleted, send update to VMS
                string format = "yyyy-MM-dd HH:mm";
                string dt = DateTime.Now.AddDays(-1).ToString(format);
                await RegisterVehicleToVMS(new List<VMSVehiclePostRequestModel>
                    {
                         new VMSVehiclePostRequestModel
                         {
                              expiryDate = dt,
                              issueDate = userVehicle.IssueDate.HasValue ? userVehicle.IssueDate.Value.ToString(format) : dt,
                              personName = userVehicle.User != null ? userVehicle.User.FriendlyName : string.Empty,
                              vehicle = userVehicle.PlateNumber,
                              seasonId = VehicleSeasonType.S.ToString() + "_" + userVehicle.Id.ToString(),
                              cardType = !string.IsNullOrEmpty(userVehicle.CardType) ? userVehicle.CardType : VehicleCardType.STAFF.ToString(),
                         }
                    });

                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete userVehicle!";
                result.IsSuccess = false;
            }

            return result;
        }
        #endregion

        #region CardId
        public async Task<UserCardId> GetUserCardIdByIdAsync(int userId)
        {
            return await _appContext.UserCardIds.FindAsync(userId);
        }

        public async Task<UserCardId> GetUserByCardIdAsync(string cardId)
        {
            IQueryable<UserCardId> userCardIdQuery = _context.UserCardIds
                .Include(r => r.User)
                .Where(e => e.IsActive && e.CardId == cardId && e.Status == CardIdStatus.ACTIVE.ToString())
                .OrderBy(r => r.CreatedDate);


            var userCardIds = await userCardIdQuery.FirstOrDefaultAsync();

            return userCardIds;
        }



        public async Task<List<UserCardId>> GetUserCardIdsLoadRelatedAsync(int page, int pageSize, int? userId = null, string status = "")
        {
            IQueryable<UserCardId> userCardIdQuery = _context.UserCardIds
                .Include(r => r.User)
                .Where(e => e.IsActive && (string.IsNullOrEmpty(status) || status == e.Status))
                .OrderBy(r => r.CreatedDate);

            if (userId.HasValue)
            {
                userCardIdQuery = userCardIdQuery.Where(e => e.UserId == userId);
            }

            if (page != -1)
                userCardIdQuery = userCardIdQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                userCardIdQuery = userCardIdQuery.Take(pageSize);

            var userCardIds = await userCardIdQuery.ToListAsync();

            return userCardIds;
        }

        //public async Task<ApplicationUser> GetUserByCardIdAsync(string cardId)
        //{
        //    IQueryable<ApplicationUser> userCardIdQuery = _context.Users
        //        .Join(_context.UserCardIds,user => user.Id, userCardId => userCardId.UserId, (user, userCardId) => )
        //        .Where(e => e.IsActive && e.Status == CardIdStatus.ACTIVE.ToString());

        //    var userCardIds = await userCardIdQuery.ToListAsync();

        //    return userCardIds;
        //}


        public async Task<BaseOperationResponse> CreateUserCardIdAsync(UserCardId userCardId)
        {
            var result = new BaseOperationResponse();
            var existingCard = await _appContext.UserCardIds.FirstOrDefaultAsync(e => e.UserId == userCardId.UserId &&
                                e.IsActive &&
                                e.CardId.Equals(userCardId.CardId, StringComparison.CurrentCultureIgnoreCase));

            if (existingCard != null)
            {
                result.Message = "Card exists.";
                result.IsSuccess = false;
            }
            else
            {
                var f = await _appContext.UserCardIds.AddAsync(userCardId);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f.Entity;
                }
                else
                {
                    result.Message = "Failed to save Card Id!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        public async Task<UserCardId> GetActiveUserCardId(int userId)
        {
            return await _appContext.UserCardIds.Where(e => e.IsActive && e.UserId == userId && e.Status == CardIdStatus.ACTIVE.ToString()).FirstOrDefaultAsync();
        }

        public async Task<BaseOperationResponse> CreateUserCardIdActivateAsync(UserCardId userCardId)
        {
            var result = new BaseOperationResponse();
            var existingCard = await _appContext.UserCardIds.FirstOrDefaultAsync(e => e.UserId == userCardId.UserId && 
                                e.Status == CardIdStatus.ACTIVE.ToString() && 
                                e.CardId.Equals(userCardId.CardId, StringComparison.CurrentCultureIgnoreCase));

            if (existingCard != null)
            {
                result.Message = "Card is already activated.";
                result.IsSuccess = false;
            }
            else
            {
                _appContext.UserCardIds
                .Where(e => e.IsActive && e.UserId == userCardId.UserId)
                .ToList()
                .ForEach(a => a.Status = CardIdStatus.INACTIVE.ToString());

                var d = _appContext.SaveChanges();


                userCardId.Status = CardIdStatus.ACTIVE.ToString();
                var f = await _appContext.UserCardIds.AddAsync(userCardId);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f.Entity;
                }
                else
                {
                    result.Message = "Failed to save Card Id!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateUserCardIdAsync(UserCardId userCardId)
        {
            var result = new BaseOperationResponse();

            var f = await _appContext.UserCardIds.SingleOrDefaultAsync(e => e.Id == userCardId.Id);
            bool isPending = f.Status != CardIdStatus.ACTIVE.ToString();
            f.CopyFrom(userCardId);
            _appContext.UserCardIds.Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save card Id!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<TestDeleteResult> TestCanDeleteUserCardIdAsync(int userCardId)
        {
            //TODO: add correct logic here, for now prohibit deletion of userVehicle
            return new TestDeleteResult { IsDeletable = true, Message = string.Empty }; // !await _appContext.ReservationInvitees.AnyAsync(e => e == userVehicleId);
        }


        public async Task<BaseOperationResponse> DeleteUserCardIdAsync(int userCardIdId)
        {
            var result = new BaseOperationResponse();
            var userCardId = await _appContext.UserCardIds.SingleOrDefaultAsync(r => r.Id == userCardIdId);

            if (userCardId != null)
                return await DeleteUserCardId(userCardId);

            result.IsSuccess = false;
            result.Message = "User Card Id not found.";
            return result;
        }

        public async Task<BaseOperationResponse> DeleteUserCardId(UserCardId userCardId)
        {
            var result = new BaseOperationResponse();
            userCardId.IsActive = false;
            _appContext.UserCardIds.Update(userCardId);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                //newly deleted, send update to VMS
                //string format = "yyyy-MM-dd HH:mm";
                //string dt = DateTime.Now.AddDays(-1).ToString(format);
                //await RegisterVehicleToVMS(new List<VMSVehiclePostRequestModel>
                //    {
                //         new VMSVehiclePostRequestModel
                //         {
                //              expiryDate = dt,
                //              issueDate = userVehicle.IssueDate.HasValue ? userVehicle.IssueDate.Value.ToString(format) : dt,
                //              personName = userVehicle.User != null ? userVehicle.User.FriendlyName : string.Empty,
                //              vehicle = userVehicle.PlateNumber,
                //              seasonId = VehicleSeasonType.S.ToString() + "_" + userVehicle.Id.ToString(),
                //              cardType = !string.IsNullOrEmpty(userVehicle.CardType) ? userVehicle.CardType : VehicleCardType.STAFF.ToString(),
                //         }
                //    });

                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete user Card Id!";
                result.IsSuccess = false;
            }

            return result;
        }
        #endregion

        #region LDAP
        public async Task<BaseOperationResponse> SyncLdap(bool includeContactGroup = true)
        {
            var response = new BaseOperationResponse();
            var ldapInstitution = _configuration["AppSettings:LDAP_Institution"];
            var existingInstitution = await _appContext.Institutions.FirstOrDefaultAsync(e => e.Name.Equals(ldapInstitution, StringComparison.OrdinalIgnoreCase));

            if (existingInstitution != null)
            {
                try
                {
                    var ldapUsers = GetLdapUsers();
                    foreach (var ldapUser in ldapUsers)
                    {
                        if (!string.IsNullOrEmpty(ldapUser.OrgUnit))
                        {
                            //check if OU exist as department in department list
                            await ProcessLdapAsUser(ldapUser, existingInstitution);

                            await _appContext.SaveChangesAsync();

                            //department and user info added

                            if (includeContactGroup)
                            {
                                var existingUser = _appContext.Users.FirstOrDefault(e =>
                                               e.UserName.Equals(ldapUser.Email, StringComparison.OrdinalIgnoreCase) ||
                                               e.Email.Equals(ldapUser.Email, StringComparison.OrdinalIgnoreCase));
                                int? userId = existingUser != null ? existingUser.Id : (int?)null;

                                var existingDept = _appContext.Departments.FirstOrDefault(e =>
                                        e.InstitutionId == existingInstitution.Id && e.Name.Equals(ldapUser.OrgUnit, StringComparison.OrdinalIgnoreCase));

                                var existingCG = await _appContext.ContactGroups.FirstOrDefaultAsync(e =>
                                                   e.Name.Equals(ldapUser.OrgUnit, StringComparison.OrdinalIgnoreCase));
                                if (existingCG == null)
                                {
                                    existingCG = new ContactGroup
                                    {
                                        Name = ldapUser.OrgUnit,
                                        Description = ldapUser.OrgUnit,
                                        ContactGroupDepartments = new List<ContactGroupDepartment>(),
                                        InstitutionId = existingInstitution.Id
                                    };

                                    if (existingDept.Id > 0)
                                    {
                                        existingCG.ContactGroupDepartments.Add(new ContactGroupDepartment { DepartmentId = existingDept.Id });
                                    }
                                    else
                                    {
                                        existingCG.ContactGroupDepartments.Add(new ContactGroupDepartment { Department = existingDept });
                                    }

                                    await _appContext.ContactGroups.AddAsync(existingCG);
                                }
                                else
                                {
                                    //update
                                    if (existingCG.Name != ldapUser.OrgUnit || existingCG.Description != ldapUser.OrgUnit)
                                    {
                                        existingCG.Name = ldapUser.OrgUnit;
                                        existingCG.Description = ldapUser.OrgUnit;
                                        if (!existingCG.InstitutionId.HasValue)
                                        {
                                            existingCG.InstitutionId = existingInstitution.Id;
                                        }
                                        _appContext.ContactGroups.Update(existingCG);
                                    }
                                }

                                await _appContext.SaveChangesAsync(); //just insert regardless of members
                                                                      //check if email exists in contact group
                                var existingCGMember = await _appContext.ContactGroupMembers.FirstOrDefaultAsync(e => e.ContactGroupId == existingCG.Id &&
                                                    (!string.IsNullOrEmpty(e.Email) && e.Email.Equals(ldapUser.Email, StringComparison.OrdinalIgnoreCase)));
                                //((!string.IsNullOrEmpty(e.Name) && e.Name.Equals(ldapUser.DisplayName, StringComparison.OrdinalIgnoreCase)) ||
                                // (!string.IsNullOrEmpty(e.Name) && e.Name.Equals(ldapUser.FullName, StringComparison.OrdinalIgnoreCase))) &&
                                //((!string.IsNullOrEmpty(e.Email) && e.Email.Equals(ldapUser.Email, StringComparison.OrdinalIgnoreCase)) ||
                                //(!string.IsNullOrEmpty(e.HomeNo) && e.HomeNo.Equals(ldapUser.HomeNo, StringComparison.OrdinalIgnoreCase)) ||
                                //(!string.IsNullOrEmpty(e.MobileNo) && e.MobileNo.Equals(ldapUser.MobileNo, StringComparison.OrdinalIgnoreCase)) ||
                                //(!string.IsNullOrEmpty(e.PhoneNumber) && e.PhoneNumber.Equals(ldapUser.TelNo, StringComparison.OrdinalIgnoreCase))));

                                if (existingCGMember == null)
                                {
                                    existingCGMember = new ContactGroupMember
                                    {
                                        ContactGroupId = existingCG.Id,
                                        Designation = !string.IsNullOrEmpty(ldapUser.DesignationName) ? ldapUser.DesignationName : ldapUser.FullName,
                                        Department = ldapUser.Department,
                                        Email = ldapUser.Email,
                                        Name = !string.IsNullOrEmpty(ldapUser.DisplayName) ? ldapUser.DisplayName : ldapUser.FullName,
                                        PhoneNumber = ldapUser.TelNo,
                                        MobileNo = ldapUser.MobileNo,
                                        HomeNo = ldapUser.HomeNo,
                                        UserId = userId
                                    };

                                    if (IsValidEmailAddress(ldapUser.Email))
                                    {
                                        await _appContext.ContactGroupMembers.AddAsync(existingCGMember);
                                    }
                                }
                                else
                                {
                                    //update
                                    if (existingCGMember.Designation != ldapUser.DesignationName ||
                                        existingCGMember.Department != ldapUser.Department ||
                                        existingCGMember.Email != ldapUser.Email ||
                                        existingCGMember.Name != ldapUser.DisplayName ||
                                        existingCGMember.Name != ldapUser.FullName ||
                                        existingCGMember.HomeNo != ldapUser.HomeNo ||
                                        existingCGMember.MobileNo != ldapUser.MobileNo ||
                                        existingCGMember.PhoneNumber != ldapUser.TelNo ||
                                        (!existingCGMember.UserId.HasValue && userId.HasValue))
                                    {
                                        existingCGMember.Designation = ldapUser.DesignationName;
                                        existingCGMember.Department = ldapUser.Department;
                                        existingCGMember.Email = ldapUser.Email;
                                        existingCGMember.Name = !string.IsNullOrEmpty(ldapUser.DisplayName) ? ldapUser.DisplayName : ldapUser.FullName;
                                        existingCGMember.PhoneNumber = ldapUser.TelNo;
                                        existingCGMember.HomeNo = ldapUser.HomeNo;
                                        existingCGMember.MobileNo = ldapUser.MobileNo;
                                        existingCGMember.UserId = userId;

                                        if (IsValidEmailAddress(ldapUser.Email))
                                        {
                                            _appContext.ContactGroupMembers.Update(existingCGMember);
                                        }
                                    }
                                }

                            }
                            await _appContext.SaveChangesAsync();
                        }
                    }

                    response.IsSuccess = true;
                    response.Message = "Successfully synced.";
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "Institution Name not found. Please check the config file.";
            }
            return response;
        }

        private List<LdapInfo> GetLdapUsers()
        {
            var ldapUsers = new List<LdapInfo>();
            try
            {
                var ldapServer = _configuration["AppSettings:LDAP_Server"];
                var uname = _configuration["AppSettings:LDAP_Username"];
                var password = _configuration["AppSettings:LDAP_Password"];

                var root = new DirectoryEntry(ldapServer, uname, password, AuthenticationTypes.None);
                StringBuilder sb = new StringBuilder();
                foreach (DirectoryEntry entry in root.Children)
                {
                    PropertyCollection props = entry.Properties;
                    //Object obj = entry.NativeObject;

                    var ds = new DirectorySearcher(entry);
                    ds.CacheResults = false;
                    ds.SizeLimit = 0;
                    ds.PageSize = 999;
                    //ds.Filter = "(SAMAccountName=" + username + ")";
                    //string filter = ConfigurationManager.AppSettings["LDAP_Filter"];
                    //ds.Filter = filter; // "(&" + "(objectClass=user)(company=MOHH)(|(title=Medical Officer)(title=House Officer)(title=Consultant)(title=Senior Consultant)(title=Associate Consultant)(title=Registrar)(title=Senior Registrar)(title=Service Registrar)(title=Clinical Associate)(title=Fellow)(title=Senior Dental Officer)(title=Dental Officer)(title=Principal Staff Physician)(title=Senior Staff Physician)(title=Resident Physician)(title=Resident)(title=Senior Resident Physician)(title=Principal Resident Physician)))"; //TODO: hard-coded to MOHH for now

                    foreach (SearchResult sResultSet in ds.FindAll())
                    {
                        var memberOf = GetProperty(sResultSet, "memberOf");

                        var ldapInfo = new LdapInfo();
                        ldapInfo.DisplayName = GetProperty(sResultSet, "displayName");
                        ldapInfo.FullName = GetProperty(sResultSet, "name");
                        ldapInfo.Office = GetProperty(sResultSet, "physicalDeliveryOfficeName");
                        ldapInfo.TelNo = GetProperty(sResultSet, "telephoneNumber");
                        ldapInfo.HomeNo = GetProperty(sResultSet, "homePhone");
                        ldapInfo.MobileNo = GetProperty(sResultSet, "mobile");
                        ldapInfo.Department = GetProperty(sResultSet, "department");
                        ldapInfo.OrgUnit = entry.Properties["ou"].Value.ToString(); // GetProperty(sResultSet, "ou");
                        ldapInfo.DesignationName = GetProperty(sResultSet, "title");
                        ldapInfo.Email = GetProperty(sResultSet, "mail");
                        ldapInfo.Adid = GetProperty(sResultSet, "cn");

                        if (!string.IsNullOrEmpty(ldapInfo.Email) || !string.IsNullOrEmpty(ldapInfo.DisplayName) || !string.IsNullOrEmpty(ldapInfo.FullName))
                        {
                            ldapUsers.Add(ldapInfo);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            finally
            {
                //if (entry != null)
                //{
                //    entry.Close();
                //    entry.Dispose();
                //}


            }

            return ldapUsers;
        }

        public string GetProperty(SearchResult searchResult, string PropertyName)
        {
            if (searchResult.Properties.Contains(PropertyName))
            {
                return searchResult.Properties[PropertyName][0].ToString().Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        public bool IsValidEmailAddress(string address)
        {
            return !string.IsNullOrEmpty(address) && new EmailAddressAttribute().IsValid(address);
        }

        private async Task ProcessLdapAsUser(LdapInfo ldapUser, Institution institution)
        {
            Department existingDept = null;
            if (!string.IsNullOrEmpty(ldapUser.OrgUnit))
            {
                //check if OU exist as department in FirstOrDefaultAsync list
                existingDept = _appContext.Departments.FirstOrDefault(e =>
                                    e.InstitutionId == institution.Id && e.Name.Equals(ldapUser.OrgUnit, StringComparison.OrdinalIgnoreCase));

                if (existingDept == null)
                {
                    existingDept = new Department
                    {
                        InstitutionId = institution.Id,
                        Name = ldapUser.OrgUnit,
                        Description = ldapUser.OrgUnit
                    };

                    _appContext.Departments.Add(existingDept);
                }
                else
                {
                    //update
                    if (existingDept.Name != ldapUser.OrgUnit || existingDept.Description != ldapUser.OrgUnit)
                    {
                        existingDept.InstitutionId = institution.Id;
                        existingDept.Name = ldapUser.OrgUnit;
                        existingDept.Description = ldapUser.OrgUnit;
                        _appContext.Departments.Update(existingDept);
                    }
                }

                await _appContext.SaveChangesAsync();
            }

            string name = !string.IsNullOrEmpty(ldapUser.DisplayName) ? ldapUser.DisplayName : ldapUser.FullName;
            //string phoneNumber = !string.IsNullOrEmpty(ldapUser.HomeNo) ? ldapUser.HomeNo :
            //                (!string.IsNullOrEmpty(ldapUser.MobileNo) ? ldapUser.MobileNo : ldapUser.TelNo);

            var existingUser = _appContext.Users.FirstOrDefault(e =>
                               e.UserName.Equals(ldapUser.Email, StringComparison.OrdinalIgnoreCase) ||
                               e.Email.Equals(ldapUser.Email, StringComparison.OrdinalIgnoreCase));
            if (existingUser == null)
            {
                if (!string.IsNullOrEmpty(ldapUser.Email))
                {

                    const string userRoleName = "user";
                    await EnsureRoleAsync(userRoleName, "Default user", new string[] { });

                    await CreateLdapApplicationUserAsync(ldapUser.Email, "Serial@PIB123", name, ldapUser.Email, ldapUser.TelNo, ldapUser.HomeNo, ldapUser.MobileNo, new string[] { userRoleName }, institution, existingDept);
                }
            }
            else
            {
                //update contact numbers, full name
                if (existingDept != null && existingDept.Id > 0)
                {
                    existingUser.DepartmentId = existingDept.Id;
                }

                existingUser.PhoneNumber = ldapUser.TelNo;
                existingUser.HomeNo = ldapUser.HomeNo;
                existingUser.MobileNo = ldapUser.MobileNo;
                existingUser.FullName = name;
                _appContext.Users.Update(existingUser);
            }
        }

        private async Task EnsureRoleAsync(string roleName, string description, string[] claims)
        {
            if ((await GetRoleByNameAsync(roleName)) == null)
            {
                ApplicationRole applicationRole = new ApplicationRole(roleName, description);

                var result = await CreateRoleAsync(applicationRole, claims);
            }
        }

        private async Task CreateLdapApplicationUserAsync(string userName, string password, string fullName, string email, string phoneNumber, string homeNo, string mobileNo, string[] roles, Institution institution, Department department)
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
                UserName = userName,
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true,
                IsEnabled = true,
                HomeNo = homeNo,
                MobileNo = mobileNo
            };

            if (institution != null) applicationUser.InstitutionId = institution.Id;
            if (department != null) applicationUser.DepartmentId = department.Id;
            var result = await _userManager.CreateAsync(applicationUser, password);

            applicationUser = await _userManager.FindByNameAsync(applicationUser.UserName);

            if (roles != null && roles.Any())
            {
                try
                {
                    result = await this._userManager.AddToRolesAsync(applicationUser, roles.Distinct());
                }
                catch
                {
                }
            }

            //await CreateUserAsync(applicationUser, roles, password);
        }

        #endregion

        #region
        public async Task<List<UserConnectionStatusDTO>> GetUserConnectionsAsync(int page, int pageSize)
        {
            var usersQuery = _context.Users
                .Include(r => r.UserConnections)
                .Where(e => e.IsActive);

            if (page != -1)
                usersQuery = usersQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                usersQuery = usersQuery.Take(pageSize);

            List<UserConnectionStatusDTO> listUsers = new List<UserConnectionStatusDTO>();

            foreach (var user in usersQuery)
            {
                var connections = user.UserConnections.Where(e => e.IsActive);
                UserConnection connection;
                if (connections != null)
                {
                    if (connections.Any(e => e.Status == UserConnectionStatus.ENGAGED.ToString()))
                    {
                        connection = connections.FirstOrDefault(e => e.Status == UserConnectionStatus.ENGAGED.ToString());
                    }
                    else
                    {
                        connection = connections.FirstOrDefault(e => e.Status == UserConnectionStatus.ONLINE.ToString());
                    }
                }
                else
                {
                    connection = connections.FirstOrDefault(e => e.Status == UserConnectionStatus.OFFLINE.ToString());
                }

                listUsers.Add(new UserConnectionStatusDTO
                {
                    ConnectionID = connection.ConnectionID,
                    Identifier = connection.Identifier,
                    Name = user.FriendlyName,
                    Status = connection.Status,
                    UserId = user.Id
                });
            }

            return listUsers;
        }
        #endregion

        #region private methods
        public async Task<BaseOperationResponse> RegisterVehicleToVMS(List<VMSVehiclePostRequestModel> list)
        {
            string logName = "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy");
            var response = new BaseOperationResponse() { Data = new { } };

            try
            {
                string apiUrl = string.Empty;
                var appSetting = await _appContext.ApplicationSettings.FirstOrDefaultAsync(e => e.IsActive && e.Key == "VMS_API_URL");
                if (appSetting != null)
                {
                    apiUrl = appSetting.Value;
                }
                using (var client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(apiURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    await WriteLog("ACCOUNT START REQUEST - " + DateTime.Now.ToString(), logName);
                    var jsonObj = JsonConvert.SerializeObject(new
                    {
                        vehicles = list
                    });
                    await WriteLog("ACCOUNT REQUEST BODY: \n" + jsonObj, logName);
                    var stringContent = new StringContent(jsonObj, Encoding.UTF8, "application/json");

                    var resp = await client.PostAsync(apiUrl, stringContent);

                    //var response = client.GetAsync(string.Format("?locationCode={0}", locationCode)).Result;
                    resp.EnsureSuccessStatusCode();
                    var content = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    response.IsSuccess = true;
                    response.Message = "Successfully registered the vehicle/s";


                    await WriteLog("ACCOUNT SUCCESSFUL LOG STARTS HERE", logName);
                    await WriteLog(content, logName);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
                await WriteLog(response.Message, logName);
                await WriteLog(ex.InnerException != null ? ex.InnerException.StackTrace : string.Empty, logName);
            }

            await WriteLog("ACCOUNT END REQUEST - " + DateTime.Now.ToString(), logName);
            return response;
        }

        private async Task WriteLog(string strLog, string logName)
        {
            try
            {
                System.IO.StreamWriter log;
                System.IO.FileStream fileStream = null;
                System.IO.DirectoryInfo logDirInfo = null;
                System.IO.FileInfo logFileInfo;

                string logFilePath = string.Empty;
                var appSetting = await _appContext.ApplicationSettings.FirstOrDefaultAsync(e => e.IsActive && e.Key == "VMS_LOG");
                if (appSetting != null)
                {
                    logFilePath = appSetting.Value;
                }

                logFilePath = logFilePath + logName + "." + "txt";
                logFileInfo = new System.IO.FileInfo(logFilePath);
                logDirInfo = new System.IO.DirectoryInfo(logFileInfo.DirectoryName);
                if (!logDirInfo.Exists) logDirInfo.Create();
                if (!logFileInfo.Exists)
                {
                    fileStream = logFileInfo.Create();
                }
                else
                {
                    fileStream = new System.IO.FileStream(logFilePath, System.IO.FileMode.Append);
                }
                log = new System.IO.StreamWriter(fileStream);
                log.WriteLine(strLog);
                log.Close();
            }
            catch (Exception)
            {
            }
        }
        #endregion
        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
