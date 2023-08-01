using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IDeviceTypeRepository : IRepository<DeviceType>
    {
        Task<BaseOperationResponse> CreateAsync(DeviceType assetType);
        Task<BaseOperationResponse> DeleteAsync(int assetTypeId);
        Task<DeviceType> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(DeviceType assetType);
        Task<PagedEntity<DeviceType>> GetDeviceTypesAsync(BaseFilter filter);
    }
}
