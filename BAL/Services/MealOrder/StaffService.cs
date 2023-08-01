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
    public class StaffService : IStaffService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private IAccountManager _accountManager;

        public StaffService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IAccountManager accountManager)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _accountManager = accountManager;
        }

        #region Staff

        public async Task<PagedEntity<StaffDTO>> GetStaffsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<StaffDTO>>(await this._uow.Staffs.GetStaffsAsync(filter));
            return result;
        }

        public async Task<StaffDTO> GetStaffByIdAsync(int id)
        {
            return Mapper.Map<StaffDTO>(await this._uow.Staffs.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateStaffAsync(StaffDTO dto)
        {
            var result = new BaseOperationResponse();
            var staff = Mapper.Map<Staff>(dto);
            var user = Mapper.Map<ApplicationUser>(dto);
            var cards = Mapper.Map<List<UserCardId>>(dto.Cards);
            result = await this._uow.Staffs.CreateAsync(this._accountManager, staff, user, dto.NewPassword, cards);
            return result;
        }

        public async Task<BaseOperationResponse> UpdateStaffAsync(StaffDTO dto)
        {
            var result = new BaseOperationResponse();
            var staff = Mapper.Map<Staff>(dto);
            var user = Mapper.Map<ApplicationUser>(dto);
            var cards = Mapper.Map<List<UserCardId>>(dto.Cards);
            result = await this._uow.Staffs.UpdateAsync(this._accountManager, staff, user, dto.CurrentPassword, dto.NewPassword, cards);
            return result;
        }

        public async Task<BaseOperationResponse> DeleteStaffAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Staffs.DeleteAsync(id);
            return result;
        }

        #endregion

    }
}
