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
    public class ModuleRepository : Repository<Module>, IModuleRepository
    {
        public ModuleRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<Module> GetByCodeAsync(string code)
        {
            return await _appContext.Modules
                .SingleOrDefaultAsync(e => e.IsActive && (e.Name.Equals(code, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<Module> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<Module> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<BaseOperationResponse> GetApiModules(int? moduleId = null)
        {
            var response = new BaseOperationResponse();
            try
            {
                IQueryable<Module> query = _appContext.Modules
                .Where(e => e.IsActive && (!moduleId.HasValue || e.Id == moduleId))
                .OrderBy(r => r.Name);

                response.Data = await query.ToListAsync();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public async Task<List<Module>> GetModulesLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<Module> query = _appContext.Modules.Where(e => e.IsActive)
                .OrderBy(r => r.Name);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<BaseOperationResponse> CreateAsync(Module module)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(module);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save module!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(Module module)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == module.Id);

            //Update tasks
            if (module.ModuleParameters != null)
            {
                var entitiesToDelete = this._appContext.ModuleParameters.Where(e => e.ModuleId == module.Id &&
                                        (!module.ModuleParameters.Any() || !module.ModuleParameters.Any(r => r.Id == e.Id)));

                foreach (var toDelete in entitiesToDelete)
                {
                    toDelete.IsActive = false;
                    this._appContext.ModuleParameters.Update(toDelete);
                }

                foreach (var parameter in module.ModuleParameters)
                {
                    var existingEntity = this._appContext.ModuleParameters.FirstOrDefault(r => r.Id == parameter.Id);

                    if (existingEntity == null)
                    {
                        this._appContext.ModuleParameters.Add(parameter);
                    }
                    else
                    {
                        existingEntity.CopyFrom(parameter);
                        this._appContext.ModuleParameters.Update(existingEntity);
                    }
                }
            }

            f.CopyFrom(module);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save module!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<bool> TestCanDeleteAsync(int moduleId)
        {
            //TODO: add correct logic here, for now prohibit deletion of module
            return false;//!await _appContext..AnyAsync(e => e == moduleId);
        }


        public async Task<BaseOperationResponse> DeleteAsync(int moduleId)
        {
            var result = new BaseOperationResponse();
            var module = await GetSingleOrDefaultAsync(r => r.Id == moduleId);

            if (module != null)
                return await Delete(module);

            result.IsSuccess = false;
            result.Message = "Module not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Module module)
        {
            var result = new BaseOperationResponse();
            SoftDelete(module);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete module!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
