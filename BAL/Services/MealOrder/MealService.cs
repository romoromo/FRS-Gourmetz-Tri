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
    public class MealService : IMealService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private IAccountManager _accountManager;

        public MealService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IAccountManager accountManager)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _accountManager = accountManager;
        }

        #region MealPeriod

        public async Task<PagedEntity<MealPeriodDTO>> GetMealPeriodsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<MealPeriodDTO>>(await this._uow.MealPeriods.GetMealPeriodsAsync(filter));
            return result;
        }

        public async Task<MealPeriodDTO> GetMealPeriodByIdAsync(int id)
        {
            return Mapper.Map<MealPeriodDTO>(await this._uow.MealPeriods.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateMealPeriodAsync(MealPeriodDTO dto)
        {
            return await this._uow.MealPeriods.CreateAsync(Mapper.Map<MealPeriod>(dto));
        }

        public async Task<BaseOperationResponse> UpdateMealPeriodAsync(MealPeriodDTO dto)
        {
            return await this._uow.MealPeriods.UpdateAsync(Mapper.Map<MealPeriod>(dto));
        }

        public async Task<BaseOperationResponse> DeleteMealPeriodAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.MealPeriods.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Meal Type

        public async Task<PagedEntity<MealTypeDTO>> GetMealTypesAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<MealTypeDTO>>(await this._uow.MealTypes.GetMealTypesAsync(filter));
            return result;
        }

        public async Task<MealTypeDTO> GetMealTypeByIdAsync(int id)
        {
            return Mapper.Map<MealTypeDTO>(await this._uow.MealTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateMealTypeAsync(MealTypeDTO dto)
        {
            return await this._uow.MealTypes.CreateAsync(Mapper.Map<MealType>(dto));
        }

        public async Task<BaseOperationResponse> UpdateMealTypeAsync(MealTypeDTO dto)
        {
            return await this._uow.MealTypes.UpdateAsync(Mapper.Map<MealType>(dto));
        }

        public async Task<BaseOperationResponse> DeleteMealTypeAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.MealTypes.DeleteAsync(id);
            return result;
        }

        #endregion

        #region MealSession

        public async Task<PagedEntity<MealSessionDTO>> GetMealSessionsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<MealSessionDTO>>(await this._uow.MealSessions.GetMealSessionsAsync(filter));
            return result;
        }

        public async Task<List<MealSessionMealPeriodDTO>> GetMealSessionsByMealPeriod(int outletId, int catererId, int outletProfileId)
        {
            var result = Mapper.Map<List<MealSessionMealPeriodDTO>>(this._uow.MealSessions.GetMealSessionsByMealPeriod(outletId, catererId, outletProfileId));
            return result;
        }

        public async Task<List<MealSessionMealPeriodDTO>> GetAllMealSessionsByMealPeriod(int outletId, int catererId, int outletProfileId)
        {
            var results = this._uow.MealSessions.GetMealSessionsByMealPeriod(outletId, catererId, outletProfileId);
            var result = results.Select(e => new MealSessionMealPeriodDTO
            {
                MealPeriod = Mapper.Map<MealPeriodDTO>(e.MealPeriod),
                MealSessions = e.MealSessions?.Select(f => new MealSessionDTO
                {
                    CatererId = f.CatererId,
                    ClassRosters = Mapper.Map<List<OutletClassRosterDTO>>(f.ClassRosters),
                    EndDate = f.EndDate,
                    Id = f.Id,
                    IsActive = f.IsActive,
                    MealPeriodId = f.MealPeriodId,
                    MealPeriodName = f.MealPeriod?.Name,
                    Name = f.Name,
                    OutletId = f.OutletId,
                    OutletName = f.Outlet?.Name,
                    Sequence = f.Sequence,
                    StartDate = f.StartDate,
                    Details = f.Details?.Select(x => new MealSessionDetailDTO
                    {
                        CalSourceTime = x.CalSourceTime,
                        EndDate = x.EndDate,
                        Id = x.Id,
                        IsActive = x.IsActive,
                        MealPeriodId = x.MealSession?.MealPeriodId,
                        MealPeriodName = x.MealSession.MealPeriod != null ? x.MealSession.MealPeriod.Name : string.Empty,
                        MealSessionId = x.MealSessionId,
                        MealSessionName = x.MealSession?.Name,
                        Name = x.Name,
                        OverheadInterval = x.OverheadInterval,
                        OverheadTime = x.OverheadTime,
                        RouteId = x.RouteId,
                        RouteInterval = x.RouteInterval,
                        RouteName = x.Route.Label,
                        RouteTime = x.RouteTime,
                        Sequence = x.Sequence,
                        StartDate = x.StartDate,
                    }).ToList()
                }).ToList()
            }).ToList();

            return result;
        }

        public async Task<List<MealSessionMealPeriodDTO>> GetMealSessionsByMealPeriodByOutlet(int outletId)
        {
            var result = Mapper.Map<List<MealSessionMealPeriodDTO>>(this._uow.MealSessions.GetMealSessionsByMealPeriodByOutlet(outletId));
            return result;
        }

        public async Task<MealSessionDTO> GetMealSessionByIdAsync(int id)
        {
            return Mapper.Map<MealSessionDTO>(await this._uow.MealSessions.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> BulkCreateMealSessionAsync(List<MealSessionDTO> dto)
        {
            return await this._uow.MealSessions.BulkCreateAsync(Mapper.Map<List<MealSession>>(dto));
        }

        public async Task<BaseOperationResponse> CreateMealSessionAsync(MealSessionDTO dto)
        {
            return await this._uow.MealSessions.CreateAsync(Mapper.Map<MealSession>(dto));
        }

        public async Task<BaseOperationResponse> UpdateMealSessionAsync(MealSessionDTO dto)
        {
            return await this._uow.MealSessions.UpdateAsync(Mapper.Map<MealSession>(dto));
        }

        public async Task<BaseOperationResponse> DeleteMealSessionAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.MealSessions.DeleteAsync(id);
            return result;
        }

        #endregion
    }
}
