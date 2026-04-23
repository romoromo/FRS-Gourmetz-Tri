using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Core
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        private RoleManager<IdentityRole> _roleManager;
        private IUnitOfWork _unitOfWork;
        private IUserStore<ApplicationUser> _store;
        private IPasswordHasher<ApplicationUser> _passwordHasher;

        public int InstitutionId { get; set; }
        public string InstitutionCode { get; set; }

        public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
                IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators,
                IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer,
                IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger,
                IUnitOfWork unitOfWork)
                : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _unitOfWork = unitOfWork;
            _store = store;
            _passwordHasher = passwordHasher;
        }

        public override async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var institution = await GetCurrentInstitution();
            ApplicationUser user = null;
            if (institution != null)
            {
                user = await base.Users.FirstOrDefaultAsync(e => e.InstitutionId == institution.Id && e.UserName.ToLower() == userName.ToLower());
            }

            return user ?? await base.Users.FirstOrDefaultAsync(e => e.UserName.ToLower() == userName.ToLower());
        }

        public override async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            //TODO: add institution here; temporarily removed checking of institution
            //return await base.Users.FirstOrDefaultAsync(e => e.InstitutionId == _unitOfWork.CurrentInstitutionId && e.Email == email);
            var institution = await GetCurrentInstitution();
            ApplicationUser user = null;
            if (institution != null)
            {
                user = await base.Users.FirstOrDefaultAsync(e => e.InstitutionId == institution.Id && e.Email.ToLower() == email.ToLower());
            }

            return user ?? await base.Users.FirstOrDefaultAsync(e => e.Email.ToLower() == email.ToLower());
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            var existingUser = await FindByEmailAsync(user.Email);
            if (existingUser != null && !existingUser.IsActive)
            {
                existingUser.CopyFrom(user);
                existingUser.IsActive = true;

                return await UpdateAsync(existingUser);
            }
            else
            {
                return await base.CreateAsync(user);
            }

        }

        public async Task<IdentityResult> CreateOrUpdatePassAsync(ApplicationUser user,string password)
        {
            var existingUser = await FindByEmailAsync(user.Email);
            if (existingUser != null && !existingUser.IsActive)
            {
                existingUser.CopyFrom(user);
                existingUser.IsActive = true;

                var remove = await base.RemovePasswordAsync(existingUser);

                var add = await base.AddPasswordAsync(existingUser, password);

                return await UpdateAsync(existingUser);
            }
            else
            {
                return await base.CreateAsync(user, password);
            }

        }

        public async Task<IdentityResult> UpdateWithPassAsync(ApplicationUser user, string password)
        {
            var remove = await base.RemovePasswordAsync(user);

            var add = await base.AddPasswordAsync(user, password);

            return await UpdateAsync(user);

        }

        public override async Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            user.IsActive = false;
            user.Claims = null;
            user.DeletedDate = DateTime.Now;
            if (user.UserPhonebooks.Any())
            {
                user.UserPhonebooks.ToList().ForEach(e => e.IsActive = false);
            }

            if (user.UserVehicles.Any())
            {
                user.UserVehicles.ToList().ForEach(e => e.IsActive = false);
            }

            if (user.UserCardIds.Any())
            {
                user.UserCardIds.ToList().ForEach(e => e.IsActive = false);
            }

            return await UpdateAsync(user);
        }

        public async Task<IdentityResult> TrueDeleteAsync(ApplicationUser user)
        {
            user.IsActive = false;
            user.Claims = null;
            user.DeletedDate = DateTime.Now;
            user.UserName = "delete_" + user.UserName;
            user.Email = "delete_" + user.Email;
            user.NormalizedUserName = "DELETE_" + user.NormalizedUserName;
            user.NormalizedEmail = "DELETE_" + user.NormalizedEmail;

            if (user.UserPhonebooks.Any())
            {
                user.UserPhonebooks.ToList().ForEach(e => e.IsActive = false);
            }

            if (user.UserVehicles.Any())
            {
                user.UserVehicles.ToList().ForEach(e => e.IsActive = false);
            }

            if (user.UserCardIds.Any())
            {
                user.UserCardIds.ToList().ForEach(e => e.IsActive = false);
            }

            return await UpdateAsync(user);
        }

        public async Task<Institution> GetCurrentInstitution()
        {
            var institutions = await _unitOfWork.Institutions.FindAsync(e => e.IsActive && (e.Name == InstitutionCode || e.Id == InstitutionId));
            if (!institutions.Any())
            {
                institutions = await _unitOfWork.Institutions.FindAsync(e => e.IsActive && e.IsDefault);
            }

            return institutions.FirstOrDefault();
        }

        public Task AddToUsedPasswordAsync(ApplicationUser appuser, string userpassword)
        {
            var hashedPassword = _passwordHasher.HashPassword(appuser, userpassword);
            appuser.UsedPasswords.Add(new UsedPassword() { UserId = appuser.Id, HashPassword = hashedPassword });
            return UpdateAsync(appuser);
        }

        public async Task<bool> IsUsedPassword(int userID, string newPassword)
        {
            var user = await FindByIdAsync(userID.ToString());

            if (user.UsedPasswords.OrderByDescending(up => up.CreatedDate)
                .Select(up => up.HashPassword).Take(7)
                .Where(up => _passwordHasher.VerifyHashedPassword(user, up, newPassword) != PasswordVerificationResult.Failed).Any())
            {
                return true;
            }

            return false;
        }

        public async Task<bool> IsUsedEmail(int userID, string email)
        {
            return await base.Users.AnyAsync(e => e.Id != userID && e.Email.ToLower() == email.ToLower());
        }

        public Task UpdateEmailAsync(ApplicationUser appuser, string email)
        {
            appuser.Email = email;
            return UpdateAsync(appuser);
        }
    }
}
