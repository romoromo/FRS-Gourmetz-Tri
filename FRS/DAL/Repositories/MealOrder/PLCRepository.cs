using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces.MealOrder;
using Sieve.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.MealOrder
{
    public class PLCRepository : Repository<PLCModel>, IPLCRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;

        public PLCRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        public async Task<PagedEntity<PLCModel>> GetPaged(BaseFilter filter)
        {
            IQueryable<PLCModel> query = _appContext.PLCs;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            return result;
        }

        public async Task<BaseOperationResponse> CreateAsync(PLCModel model)
        {
            var result = new BaseOperationResponse();

            if (await Exists(e => e.IPAddress == model.IPAddress && e.Framework == model.Framework && e.IsActive))
            {
                result.Message = "PLC already exists!";
                result.IsSuccess = false;
            }
            else
            {

                var f = await AddAsync(model);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        public async Task<BaseOperationResponse> Delete(PLCModel model)
        {
            var result = new BaseOperationResponse();
            SoftDelete(model);
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

        public async Task<BaseOperationResponse> DeleteAsync(int id)
        {
            var result = new BaseOperationResponse();
            var dispenserOutlet = await GetSingleOrDefaultAsync(r => r.Id == id);

            if (dispenserOutlet != null)
                return await Delete(dispenserOutlet);

            result.IsSuccess = false;
            result.Message = "PLC not found.";
            return result;
        }

        public async Task<PLCModel> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> UpdateAsync(PLCModel model)
        {
            var result = new BaseOperationResponse();

            if (await Exists(e => e.IPAddress == model.IPAddress && e.Framework   == model.Framework && e.IsActive && e.Id != model.Id))
            {
                result.Message = "PLC already exists!";
                result.IsSuccess = false;
            }
            else
            {
                var f = await GetSingleOrDefaultAsync(e => e.Id == model.Id);

                f.CopyFrom(model);

                Update(f);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }
    }
}
