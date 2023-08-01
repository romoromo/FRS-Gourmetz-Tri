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

namespace DAL.Repositories
{
    public class ServiceContractRepository : Repository<ServiceContract>, IServiceContractRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public ServiceContractRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<ServiceContract>> GetServiceContractsAsync(BaseFilter filter)
        {
            IQueryable<ServiceContract> query = _appContext.ServiceContracts;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            return result;
        }

        #endregion
        public async Task<ServiceContract> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(ServiceContract serviceContract)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(serviceContract);
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

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(ServiceContract serviceContract)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == serviceContract.Id);
            f.CopyFrom(serviceContract);

            var serviceContractAssets = _appContext.ServiceContractAssets.Where(e => e.ServiceContractId == serviceContract.Id);
            await serviceContractAssets.ForEachAsync(e => { e.IsActive = false; });
            _appContext.ServiceContractAssets.UpdateRange(serviceContractAssets);

            if (serviceContract.ServiceContractAssets != null)
            {
                foreach (var ir in serviceContract.ServiceContractAssets)
                {
                    var exist = await serviceContractAssets.FirstOrDefaultAsync(e => e.AssetId == ir.AssetId);

                    if (exist != null)
                    {
                        exist.IsActive = true;
                        exist.AssetId = ir.AssetId;
                        _appContext.ServiceContractAssets.Update(exist);
                    }
                    else
                    {
                        var ass = new ServiceContractAsset
                        {
                            ServiceContractId = serviceContract.Id,
                            AssetId = ir.AssetId
                        };

                        await _appContext.ServiceContractAssets.AddAsync(ass);
                    }
                }
            }

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

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int serviceContractId)
        {
            var result = new BaseOperationResponse();
            var serviceContract = await GetSingleOrDefaultAsync(r => r.Id == serviceContractId);

            if (serviceContract != null)
                return await Delete(serviceContract);

            result.IsSuccess = false;
            result.Message = "Contract not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(ServiceContract serviceContract)
        {
            var result = new BaseOperationResponse();
            SoftDelete(serviceContract);
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

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
