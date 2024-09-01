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
using DAL.Core.Interfaces;

namespace BAL.Services.MealOrder
{
    public class RestrictionService : IRestrictionService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private IAccountManager _accountManager;
        private readonly IMapper _mapper;

        public RestrictionService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IAccountManager accountManager, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _accountManager = accountManager;
            _mapper = mapper;
        }

        #region Restriction

        public async Task<PagedEntity<RestrictionDTO>> GetRestrictionsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<RestrictionDTO>>(await this._uow.Restrictions.GetRestrictionsAsync(filter));
            return result;
        }

        public async Task<RestrictionDTO> GetRestrictionByIdAsync(int id)
        {
            return _mapper.Map<RestrictionDTO>(await this._uow.Restrictions.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateRestrictionAsync(RestrictionDTO dto)
        {
            return await this._uow.Restrictions.CreateAsync(_mapper.Map<Restriction>(dto));
        }

        public async Task<BaseOperationResponse> UpdateRestrictionAsync(RestrictionDTO dto)
        {
            return await this._uow.Restrictions.UpdateAsync(_mapper.Map<Restriction>(dto));
        }

        public async Task<BaseOperationResponse> DeleteRestrictionAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Restrictions.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Restriction Type

        public async Task<PagedEntity<RestrictionTypeDTO>> GetRestrictionTypesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<RestrictionTypeDTO>>(await this._uow.RestrictionTypes.GetRestrictionTypesAsync(filter));
            return result;
        }

        public async Task<RestrictionTypeDTO> GetRestrictionTypeByIdAsync(int id)
        {
            return _mapper.Map<RestrictionTypeDTO>(await this._uow.RestrictionTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateRestrictionTypeAsync(RestrictionTypeDTO dto)
        {
            return await this._uow.RestrictionTypes.CreateAsync(_mapper.Map<RestrictionType>(dto));
        }

        public async Task<BaseOperationResponse> UpdateRestrictionTypeAsync(RestrictionTypeDTO dto)
        {
            return await this._uow.RestrictionTypes.UpdateAsync(_mapper.Map<RestrictionType>(dto));
        }

        public async Task<BaseOperationResponse> DeleteRestrictionTypeAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.RestrictionTypes.DeleteAsync(id);
            return result;
        }

        #endregion

    }
}
