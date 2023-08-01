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
    public class SignageCompilationRepository : Repository<SignageCompilation>, ISignageCompilationRepository
    {
        public SignageCompilationRepository(ApplicationDbContext context) : base(context)
        { }


        public async Task<SignageCompilation> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<SignageCompilation> GetByCodeAsync(string code)
        {
            return await _appContext.SignageCompilations.FirstOrDefaultAsync(d => d.Name == code && d.IsActive);
        }

        public IEnumerable<SignageCompilation> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<List<SignageCompilation>> GetSignageCompilationsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<SignageCompilation> query = _appContext.SignageCompilations
                .Where(e => e.IsActive)
                .OrderBy(r => r.Name);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<List<SignageCompilation>> GetSignageCompilations(string ids)
        {
            var data = new List<SignageCompilation>();

            if (!string.IsNullOrWhiteSpace(ids))
            {
                var listOfId = ids.Split(',');

                IQueryable<SignageCompilation> query = _appContext.SignageCompilations
                .Where(e => listOfId.Any(l => l == e.Id.ToString()))
                .OrderBy(r => r.Name);

                data = await query.ToListAsync();
            }

            return data;
        }

        public async Task<BaseOperationResponse> AddOrUpdateCompilationComponentAsync(SignageCompilationComponent scc)
        {
            var result = new BaseOperationResponse();

            var ct = _appContext.SignageCompilationComponents.FirstOrDefault(c => scc.ComponentId == c.ComponentId && scc.CompilationId == c.CompilationId) ?? new SignageCompilationComponent();
            //ct.CopyFrom(scc);
            if (ct.CompilationId == null) ct.CompilationId = scc.CompilationId;
            if (ct.ComponentId == null) ct.ComponentId = scc.ComponentId;
            ct.IsActive = scc.IsActive;

            ct.x = scc.x;
            ct.y = scc.y;
            ct.width = scc.width;
            ct.height = scc.height;
            ct.order = scc.order;

            _appContext.SignageCompilationComponents.Update(ct);
            _appContext.SaveChanges();

            return result;
        }

        public async Task<BaseOperationResponse> CreateAsync(SignageCompilation signageCompilation)
        {
            var result = new BaseOperationResponse();
            if (Exists(e => e.Name == signageCompilation.Name && e.IsActive).Result)
            {
                result.Message = "Signage Compilation Name already exists!";
                result.IsSuccess = false;
            }
            else
            {
                var f = await AddAsync(signageCompilation);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save signage compilation!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(SignageCompilation signageCompilation)
        {
            var result = new BaseOperationResponse();

            var f = GetSingleOrDefault(e => e.Id == signageCompilation.Id);

            if (Exists(e => e.Id != f.Id && e.Name == signageCompilation.Name && e.IsActive).Result)
            {
                result.Message = "Signage Compilation Name already exists!";
                result.IsSuccess = false;
            }
            else
            {
                f.CopyFrom(signageCompilation);
                Update(f);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    signageCompilation.Components.ToList().ForEach(cSource =>
                    {
                        if (cSource.IsActive || cSource.Id > 0)
                        {
                            var ct = _appContext.SignageCompilationComponents.FirstOrDefault(c => cSource.Id == c.Id) ?? new SignageCompilationComponent();
                            ct.CopyFrom(cSource);
                            if (f.Id != ct.CompilationId) ct.CompilationId = f.Id;
                            ct.ComponentId = cSource.ComponentId;
                            ct.IsActive = cSource.IsActive;
                            _appContext.SignageCompilationComponents.Update(ct);
                            _appContext.SaveChanges();
                        }
                    });

                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save signage compilation!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int signageCompilationId)
        {
            var result = new BaseOperationResponse();
            var signageCompilation = await GetSingleOrDefaultAsync(r => r.Id == signageCompilationId);

            if (signageCompilation != null)
                return await Delete(signageCompilation);

            result.IsSuccess = false;
            result.Message = "Signage Compilation not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(SignageCompilation signageCompilation)
        {
            var result = new BaseOperationResponse();
            SoftDelete(signageCompilation);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete signage compilation!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
