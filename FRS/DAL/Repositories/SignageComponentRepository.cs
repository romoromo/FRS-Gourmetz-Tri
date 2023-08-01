using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;

namespace DAL.Repositories
{
    public class SignageComponentRepository : Repository<SignageComponent>, ISignageComponentRepository
    {
        public SignageComponentRepository(ApplicationDbContext context) : base(context)
        { }


        public async Task<SignageComponent> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<SignageComponent> GetByCodeAsync(string code)
        {
            return await _appContext.SignageComponents.FirstOrDefaultAsync(d => d.Name == code && d.IsActive);
        }

        public IEnumerable<SignageComponent> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<List<SignageComponent>> GetSignageComponentsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<SignageComponent> query = _appContext.SignageComponents
                .Where(e => e.IsActive)
                .OrderBy(r => r.Name);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<List<SignageComponent>> GetSignageComponents(string ids)
        {
            var data = new List<SignageComponent>();

            if(!string.IsNullOrWhiteSpace(ids))
            {
                var listOfId = ids.Split(',');

                IQueryable<SignageComponent> query = _appContext.SignageComponents
                .Where(e => listOfId.Any(l => l == e.Id.ToString()))
                .OrderBy(r => r.Name);

                data = await query.ToListAsync();
            }

            return data;
        }

        public async Task<BaseOperationResponse> CreateAsync(SignageComponent signageComponent)
        {
            var result = new BaseOperationResponse();
            if (Exists(e => e.Name == signageComponent.Name && e.IsActive).Result)
            {
                result.Message = "Signage Component Name already exists!";
                result.IsSuccess = false;
            }
            else
            {
                var f = await AddAsync(signageComponent);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save signage component!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(SignageComponent signageComponent)
        {
            var result = new BaseOperationResponse();

            var f = GetSingleOrDefault(e => e.Id == signageComponent.Id);

            if (Exists(e => e.Id != f.Id && e.Name == signageComponent.Name && e.IsActive).Result)
            {
                result.Message = "Signage Component Name already exists!";
                result.IsSuccess = false;
            }
            else
            {
                f.CopyFrom(signageComponent);
                Update(f);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save signage component!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int signageComponentId)
        {
            var result = new BaseOperationResponse();
            var signageComponent = await GetSingleOrDefaultAsync(r => r.Id == signageComponentId);

            if (signageComponent != null)
                return await Delete(signageComponent);

            result.IsSuccess = false;
            result.Message = "Signage Component not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(SignageComponent signageComponent)
        {
            var result = new BaseOperationResponse();
            SoftDelete(signageComponent);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete signage component!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
