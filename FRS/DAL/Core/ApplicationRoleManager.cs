using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Core
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        private RoleManager<IdentityRole> _roleManager;
        private IUnitOfWork _unitOfWork;
        private IRoleStore<ApplicationRole> _store;
        public int InstitutionId { get; set; }
        public string InstitutionCode { get; set; }

        public ApplicationRoleManager(IRoleStore<ApplicationRole> store, IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
                ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<ApplicationRole>> logger,
                IUnitOfWork unitOfWork)
                : base(store, roleValidators, keyNormalizer, errors, logger)
        {
            _unitOfWork = unitOfWork;
            _store = store;
        }

        public override async Task<ApplicationRole> FindByNameAsync(string roleName)
        {
            var institution = await GetCurrentInstitution();
            if (institution != null)
            {
                return await base.Roles.FirstOrDefaultAsync(e => e.InstitutionId == institution.Id && e.Name == roleName);
            }
            else
            {
                return null;
            }

        }

        public override async Task<IdentityResult> CreateAsync(ApplicationRole role)
        {
            var existingRole = await FindByNameAsync(role.Name);
            if (existingRole != null && !existingRole.IsActive)
            {
                existingRole.CopyFrom(role);
                existingRole.IsActive = true;
                existingRole.Claims = role.Claims;
                return await UpdateAsync(existingRole);
            }
            else
            {
                return await base.CreateAsync(role);
            }

        }

        public override async Task<IdentityResult> DeleteAsync(ApplicationRole role)
        {
            role.IsActive = false;
            role.Claims = null;
            return await UpdateAsync(role);
        }

        private async Task<Institution> GetCurrentInstitution()
        {
            var institutions = await _unitOfWork.Institutions.FindAsync(e => e.IsActive && (e.Name == InstitutionCode || e.Id == InstitutionId));
            if (!institutions.Any())
            {
                institutions = await _unitOfWork.Institutions.FindAsync(e => e.IsActive && e.IsDefault);
            }

            return institutions.FirstOrDefault();
        }
    }
}
