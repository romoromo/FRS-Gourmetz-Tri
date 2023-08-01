using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using DAL.Core.DTO;
using Microsoft.Extensions.Configuration;
using System.DirectoryServices;
using System.ComponentModel.DataAnnotations;
using DAL.Core.Interfaces;

namespace DAL.Repositories
{
    public class ContactGroupRepository : Repository<ContactGroup>, IContactGroupRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountManager _accountManager;

        public ContactGroupRepository(IConfiguration configuration, ApplicationDbContext context) : base(context)
        {
            _configuration = configuration;
        }

        public ContactGroupRepository(IConfiguration configuration, ApplicationDbContext context, IAccountManager accountManager) : base(context)
        {
            _configuration = configuration;
            _accountManager = accountManager;
        }


        public async Task<ContactGroup> GetByIdAsync(int id, bool isSimple = true)
        {
            if (isSimple)
            {
                return await _appContext.ContactGroups
                    .Include(e => e.ContactGroupDepartments)
                    .Include(e => e.Members)
                    .FirstOrDefaultAsync(e => e.Id == id);
            }

            return await GetAsync(id);
        }

        public IEnumerable<ContactGroup> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<List<ContactGroupMember>> GetMembers(int id)
        {
            return await _appContext.ContactGroupMembers
                    .Where(e => e.IsActive && e.ContactGroupId == id).ToListAsync();
        }

        public async Task<List<ContactGroup>> GetContactGroupsLoadRelatedAsync(int page, int pageSize, int? institutionId = null, int? userId = null, List<int> departmentIds = null, bool isSimple = false)
        {
            IQueryable<ContactGroup> query = _appContext.ContactGroups;

            if (!isSimple)
            {
                query = query
                    .Include(e => e.ContactGroupDepartments)
                    .Include(e => e.Members);
            }

            query = query.Where(e => e.IsActive && (!institutionId.HasValue || e.InstitutionId == institutionId));

            if (userId.HasValue)
            {
                var user = _appContext.Users.FirstOrDefault(e => e.Id == userId);
                if (user != null)
                {
                    query = query.Where(e => e.ContactGroupDepartments.Any(f => f.DepartmentId == user.DepartmentId));
                }
            }

            if (departmentIds != null && departmentIds.Any())
            {
                query = query.Where(e => e.ContactGroupDepartments.Any(f => departmentIds.Any(x => x == f.DepartmentId)));
            }

            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.Name);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            var roles = (await query.ToListAsync())
                .OrderBy(r => r.Name).ToList();

            return roles;
        }

        public virtual Task<ContactGroup> GetByCode(int departmentId, string code)
        {
            return _entities.Include(e => e.ContactGroupDepartments)
                .FirstOrDefaultAsync(e => e.ContactGroupDepartments.Any(f => f.DepartmentId == departmentId) && e.Name.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<ContactGroupMember>> GetContactGroupMembersAsync(int page, int pageSize, int? userId = null)
        {
            IQueryable<ContactGroupMember> query = _appContext.ContactGroupMembers.Where(e => e.IsActive);

            if (userId.HasValue)
            {
                var user = _appContext.Users.FirstOrDefault(e => e.Id == userId);
                if (user != null)
                {
                    query = query.Where(e => e.ContactGroup.ContactGroupDepartments.Any(f => f.DepartmentId == user.DepartmentId));
                }
            }

            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.Name);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            var roles = (await query.ToListAsync())
                .OrderBy(r => r.Name).ToList();

            return roles;
        }

        public async Task<BaseOperationResponse> CreateAsync(ContactGroup contactGroup)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(contactGroup);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save contact group!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(ContactGroup contactGroup)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == contactGroup.Id);

            f.CopyFrom(contactGroup);
            Update(f);

            var entitiesToDelete = this._appContext.ContactGroupMembers.Where(e => e.ContactGroupId == contactGroup.Id &&
                !contactGroup.Members.Any(r => r.Email == e.Email));

            foreach (var toDelete in entitiesToDelete)
            {
                toDelete.IsActive = false;
                this._appContext.ContactGroupMembers.Update(toDelete);
            }

            contactGroup.Members = contactGroup.Members.GroupBy(x => x.Email).Select(x => x.FirstOrDefault()).ToList();
            foreach (var toInsert in contactGroup.Members)
            {
                var cg = await this._appContext.ContactGroupMembers.FirstOrDefaultAsync(r => r.ContactGroupId == contactGroup.Id &&
                    r.Email == toInsert.Email);

                if (cg == null)
                {
                    toInsert.ContactGroupId = contactGroup.Id;
                    await this._appContext.ContactGroupMembers.AddAsync(toInsert);
                }
                else if (!cg.IsActive)
                {
                    cg.IsActive = true;
                    this._appContext.ContactGroupMembers.Update(cg);
                }
                else
                {
                    if (!cg.UserId.HasValue)
                    {
                        if (cg.Icon != null)
                        {
                            //TODO: check why EF Core is not loading the Icon property; interim solution
                            var icon = await _appContext.Files.SingleOrDefaultAsync(e => e.Id == cg.FileId);
                            if (icon != null)
                            {
                                cg.Icon = icon;
                                cg.FileId = icon.Id;
                            }
                        }
                        else
                        {
                            cg.Icon = new File();
                        }

                        cg.Icon.Path = toInsert.Icon.Path;
                        cg.Icon.FileName = System.IO.Path.GetFileName(toInsert.Icon.Path);
                        cg.Icon.Type = FileType.Icon.ToString();
                    }

                    this._appContext.ContactGroupMembers.Update(cg);
                }
            }

            var cgdToDelete = this._appContext.ContactGroupDepartments.Where(e => e.ContactGroupId == contactGroup.Id &&
                !contactGroup.ContactGroupDepartments.Any(r => r.DepartmentId == e.DepartmentId));

            foreach (var toDelete in cgdToDelete)
            {
                toDelete.IsActive = false;
                this._appContext.ContactGroupDepartments.Update(toDelete);
            }

            foreach (var toInsert in contactGroup.ContactGroupDepartments)
            {
                var cg = this._appContext.ContactGroupDepartments.FirstOrDefault(r => r.ContactGroupId == contactGroup.Id &&
                    r.DepartmentId == toInsert.DepartmentId);
                if (cg == null)
                {
                    this._appContext.ContactGroupDepartments.Add(toInsert);
                }
                else
                {
                    if (!cg.IsActive)
                    {
                        cg.IsActive = true;
                        this._appContext.ContactGroupDepartments.Update(cg);
                    }
                }
            }

            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save contact group!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<TestDeleteResult> TestCanDeleteAsync(int contactGroupId)
        {
            var result = new TestDeleteResult();
            result.IsDeletable = true;
            if (await _appContext.ContactGroupMembers.AnyAsync(e => e.ContactGroupId == contactGroupId && e.IsActive))
            {
                result.IsDeletable = false;
                result.Message = "Remove all members from this contact group and try again.";
            }

            if (result.IsDeletable && await _appContext.ReservationContactGroups.AnyAsync(e => e.ContactGroupId == contactGroupId && e.IsActive))
            {
                result.IsDeletable = false;
                result.Message = "This contact group is being used by an event/booking.";
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int contactGroupId)
        {
            var result = new BaseOperationResponse();
            var contactGroup = await GetSingleOrDefaultAsync(r => r.Id == contactGroupId);

            if (contactGroup != null)
                return await Delete(contactGroup);

            result.IsSuccess = false;
            result.Message = "Contact Group not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(ContactGroup contactGroup)
        {
            var result = new BaseOperationResponse();
            SoftDelete(contactGroup);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete contact group!";
                result.IsSuccess = false;
            }

            return result;
        }



        //public async Task EnsureRoleAsync(string roleName, string description, string[] claims)
        //{
        //    if ((await GetRoleByNameAsync(roleName)) == null)
        //    {
        //        ApplicationRole applicationRole = new ApplicationRole(roleName, description);

        //        var result = await CreateRoleAsync(applicationRole, claims);
        //    }
        //}

        //public async Task CreateLdapApplicationUserAsync(string userName, string password, string fullName, string email, string phoneNumber, string homeNo, string mobileNo, string[] roles, Institution institution, Department department)
        //{
        //    ApplicationUser applicationUser = new ApplicationUser
        //    {
        //        UserName = userName,
        //        FullName = fullName,
        //        Email = email,
        //        PhoneNumber = phoneNumber,
        //        EmailConfirmed = true,
        //        IsEnabled = true,
        //        HomeNo = homeNo,
        //        MobileNo = mobileNo
        //    };

        //    if (institution != null) applicationUser.InstitutionId = institution.Id;
        //    if (department != null) applicationUser.DepartmentId = department.Id;
        //    var result = await _userManager.CreateAsync(applicationUser, password);

        //    applicationUser = await _userManager.FindByNameAsync(applicationUser.UserName);

        //    if (roles != null && roles.Any())
        //    {
        //        try
        //        {
        //            result = await this._userManager.AddToRolesAsync(applicationUser, roles.Distinct());
        //        }
        //        catch
        //        {
        //        }
        //    }

        //    //await CreateUserAsync(applicationUser, roles, password);
        //}

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
