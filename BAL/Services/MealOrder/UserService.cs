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
using BAL.Services.Interfaces.MealOrder;
using BAL.DTO.MealOrder;
using DAL.Models.MealOrder;

namespace BAL.Services.MealOrder
{
    public class UserService : IUserService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _mapper = mapper;
        }

        #region User Type

        public async Task<PagedEntity<StaffTypeDTO>> GetStaffTypesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<StaffTypeDTO>>(await this._uow.StaffTypes.GetStaffTypesAsync(filter));
            return result;
        }

        public async Task<StaffTypeDTO> GetStaffTypeByIdAsync(int id)
        {
            return _mapper.Map<StaffTypeDTO>(await this._uow.StaffTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateStaffTypeAsync(StaffTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.StaffTypes.CreateAsync(_mapper.Map<StaffType>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateStaffTypeAsync(StaffTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.StaffTypes.UpdateAsync(_mapper.Map<StaffType>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteStaffTypeAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.StaffTypes.DeleteAsync(id);
            return result;
        }

        #endregion

    }
}
