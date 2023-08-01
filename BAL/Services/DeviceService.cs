using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL;
using BAL.Services.Interfaces;
using BAL.DTO;
using AutoMapper;
using DAL.Repositories.Interfaces;
using System.IO;
using NPOI.HSSF.UserModel;

namespace BAL.Services
{
    public class DeviceService : IDeviceService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;

        public DeviceService(IUnitOfWork uow, ISieveProcessor sieveProcessor)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
        }

        #region Device Types

        public async Task<PagedEntity<DeviceTypeDTO>> GetDeviceTypesAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<DeviceTypeDTO>>(await this._uow.DeviceTypes.GetDeviceTypesAsync(filter));
            return result;
        }

        public async Task<DeviceTypeDTO> GetDeviceTypeByIdAsync(int id)
        {
            return Mapper.Map<DeviceTypeDTO>(await this._uow.DeviceTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateDeviceTypeAsync(DeviceTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.DeviceTypes.CreateAsync(Mapper.Map<DeviceType>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateDeviceTypeAsync(DeviceTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.DeviceTypes.UpdateAsync(Mapper.Map<DeviceType>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteDeviceTypeAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.DeviceTypes.DeleteAsync(id);
            return result;
        }

        #endregion
        
    }
}
