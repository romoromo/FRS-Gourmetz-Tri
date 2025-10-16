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
using DAL.Core.DTO;

namespace BAL.Services.MealOrder
{
    public class MenuService : IMenuService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private IAccountManager _accountManager;
        private readonly IMapper _mapper;

        public MenuService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IAccountManager accountManager, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _accountManager = accountManager;
            _mapper = mapper;
        }

        #region Menu 

        public async Task<PagedEntity<MenuDTO>> GetMenusAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<MenuDTO>>(await this._uow.Menus.GetMenusAsync(filter));
            return result;
        }

        public async Task<MenuDTO> GetMenuByIdAsync(int id)
        {
            return _mapper.Map<MenuDTO>(await this._uow.Menus.GetByIdAsync(id));
        }

        public async Task<MenuDishMealTypeDTO> GetMenuDishMealTypesByIdAsync(int id, int page, int pageSize)
        {
            return _mapper.Map<MenuDishMealTypeDTO>(await this._uow.Menus.GetMenuDishMealTypesByIdAsync(id, page, pageSize));
        }

        public async Task<BaseOperationResponse> CreateMenuAsync(MenuDTO dto)
        {
            return await this._uow.Menus.CreateAsync(_mapper.Map<Menu>(dto));
        }

        public async Task<BaseOperationResponse> UpdateMenuAsync(MenuDTO dto)
        {
            return await this._uow.Menus.UpdateAsync(_mapper.Map<Menu>(dto));
        }

        public async Task<BaseOperationResponse> DeleteMenuAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Menus.DeleteAsync(id);
            return result;
        }

        #endregion


        #region Menu Cycle 

        public async Task<PagedEntity<MenuCycleDTO>> GetMenuCyclesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<MenuCycleDTO>>(await this._uow.MenuCycles.GetMenuCyclesAsync(filter));
            return result;
        }

        public async Task<MenuCycleDTO> GetMenuCycleByIdAsync(int id)
        {
            return _mapper.Map<MenuCycleDTO>(await this._uow.MenuCycles.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateMenuCycleAsync(MenuCycleDTO dto)
        {
            return await this._uow.MenuCycles.CreateAsync(_mapper.Map<MenuCycle>(dto));
        }

        public async Task<BaseOperationResponse> UpdateMenuCycleAsync(MenuCycleDTO dto)
        {
            return await this._uow.MenuCycles.UpdateAsync(_mapper.Map<MenuCycle>(dto));
        }

        public async Task<BaseOperationResponse> DeleteMenuCycleAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.MenuCycles.DeleteAsync(id);
            return result;
        }

        public async Task<BaseOperationResponse> BlockMenuCycleDate(List<MenuCycleBlockedDateDTO> dto)
        {
            return await this._uow.MenuCycles.BlockMenuCycleDate(_mapper.Map<List<MenuCycleBlockedDate>>(dto));
        }

        public async Task<BaseOperationResponse> UnblockMenuCycleDate(List<MenuCycleBlockedDateDTO> dto)
        {
            return await this._uow.MenuCycles.UnblockMenuCycleDate(_mapper.Map<List<MenuCycleBlockedDate>>(dto));
        }

        public async Task<List<MenuCycleSchedulePeriodDTO>> GetMenuCycleSchedulePeriods(int menuCycleId, int day)
        {
            var result = _mapper.Map<List<MenuCycleSchedulePeriodDTO>>(await this._uow.MenuCycles.GetMenuCycleSchedulePeriods(menuCycleId, day));
            return result;
        }

        public async Task<BaseOperationResponse> CreateSchedulePeriodMenus(List<MenuCycleSchedulePeriodDTO> model)
        {
            var result = await this._uow.MenuCycles.CreateSchedulePeriodMenus(_mapper.Map<List<MenuCycleSchedulePeriod>>(model));
            return result;
        }

        #endregion


        public async Task<List<MenuCycleDTO>> GetOutletMenuCyclesAsync(int outletId, int catererId)
        {
            var result = _mapper.Map<List<MenuCycleDTO>>(await this._uow.MenuCycles.GetOutletMenuCyclesAsync(outletId, catererId));
            return result;
        }

        public async Task<BaseOperationResponse> BlockOutletDate(List<OutletBlockedDateDTO> dto)
        {
            return await this._uow.MenuCycles.BlockOutletDate(_mapper.Map<List<OutletBlockedDate>>(dto));
        }

        public async Task<BaseOperationResponse> UnblockOutletDate(List<OutletBlockedDateDTO> dto)
        {
            return await this._uow.MenuCycles.UnblockOutletDate(_mapper.Map<List<OutletBlockedDate>>(dto));
        }

        public async Task<List<MenuCycleSchedulePeriodDTO>> GetOutletMenuCycleSchedulePeriods(int outletId, int catererId, int menuCycleId, int day)
        {
            var result = _mapper.Map<List<MenuCycleSchedulePeriodDTO>>(await this._uow.MenuCycles.GetOutletMenuCycleSchedulePeriods(outletId, catererId, menuCycleId, day));
            return result;
        }

        public async Task<BaseOperationResponse> CreateOutletSchedulePeriodMenus(List<MenuCycleSchedulePeriodDTO> model)
        {
            var result = await this._uow.MenuCycles.CreateOutletSchedulePeriodMenus(_mapper.Map<List<MenuCycleSchedulePeriod>>(model));
            return result;
        }

        #region Student Calendar
        public async Task<List<OutletClassRosterDTO>> GetStudentMenuCyclesAsync(int studentId, int outletId, int catererId)
        {
            var student = await this._uow.MenuCycles.GetStudentAsync(studentId);
            var schedules = await this._uow.MenuCycles.GetStudentMenuCyclesAsync(studentId);

            var result = schedules.Select(e => new OutletClassRosterScheduleDTO
            {
                Day = e.Day,
                Id = e.Id,
                OutletClassRosterId = e.OutletClassRosterId,
                OutletClassRoster = _mapper.Map<OutletClassRosterDTO>(e.OutletClassRoster),
                HasClass = e.Periods.Any(f => f.IsActive && f.Classes.Any(x => x.ClassId == student.ClassId)),
                Periods = e.Periods.Where(f => f.IsActive).Select(f => new OutletClassRosterSchedulePeriodDTO
                {
                    Classes = _mapper.Map<List<OutletClassRosterSchedulePeriodClassDTO>>(f.Classes.Where(x => x.IsActive && x.ClassId == student.ClassId)),
                    MealSessionDetailName = f.MealSessionDetail.Name,
                    MealSessionDetailId = f.MealSessionDetailId,
                    OutletClassRosterScheduleId = f.OutletClassRosterScheduleId,
                    MealSessionDetailEndDate = f.MealSessionDetail.EndDate,
                    MealSessionDetailStartDate = f.MealSessionDetail.StartDate
                }).ToList()
            });

            var grpResult = result.GroupBy(e => e.OutletClassRosterId).Select(e => new OutletClassRosterDTO
            {
                Id = e.Key,
                StartDate = e.First().OutletClassRoster.StartDate,
                EndDate = e.First().OutletClassRoster.EndDate,
                Schedules = e.ToList()
            });

            return grpResult.ToList();
        }

        public async Task<List<OutletClassRosterDTO>> GetStudentMenuCycles2Async(int classId,int studentId, int outletId, int catererId)
        {
            //var student = await this._uow.MenuCycles.GetStudentAsync(studentId);
            var schedules = await this._uow.MenuCycles.GetStudentMenuCycles2Async(studentId);

            var result = schedules.Select(e => new OutletClassRosterScheduleDTO
            {
                Day = e.Day,
                Id = e.Id,
                OutletClassRosterId = e.OutletClassRosterId,
                OutletClassRoster = _mapper.Map<OutletClassRosterDTO>(e.OutletClassRoster),
                HasClass = e.Periods.Any(f => f.IsActive && f.Classes.Any(x => x.ClassId == classId)),
                Periods = e.Periods.Where(f => f.IsActive).Select(f => new OutletClassRosterSchedulePeriodDTO
                {
                    Classes = _mapper.Map<List<OutletClassRosterSchedulePeriodClassDTO>>(f.Classes.Where(x => x.IsActive && x.ClassId == classId)),
                    MealSessionDetailName = f.MealSessionDetail.Name,
                    MealSessionDetailId = f.MealSessionDetailId,
                    OutletClassRosterScheduleId = f.OutletClassRosterScheduleId,
                    MealSessionDetailEndDate = f.MealSessionDetail.EndDate,
                    MealSessionDetailStartDate = f.MealSessionDetail.StartDate
                }).ToList()
            });

            var grpResult = result.GroupBy(e => e.OutletClassRosterId).Select(e => new OutletClassRosterDTO
            {
                Id = e.Key,
                StartDate = e.First().OutletClassRoster.StartDate,
                EndDate = e.First().OutletClassRoster.EndDate,
                Schedules = e.ToList()
            });

            return grpResult.ToList();
        }

        public async Task<List<MealSessionDetailDTO>> GetStudentSessions(int studentId, DateTime orderDate)
        {
            var student = await this._uow.MenuCycles.GetStudentAsync(studentId);
            var schedules = await this._uow.MenuCycles.GetStudentMenuCyclesAsync(studentId);

            var grpSchedules = schedules.Where(e=> orderDate >= e.OutletClassRoster.StartDate &&
                                orderDate <= e.OutletClassRoster.EndDate).GroupBy(e => e.OutletClassRosterId);
            var listOfSessions = new List<MealSessionDetail>();

            foreach(var grpSchedule in grpSchedules)
            {
                DateTime start = grpSchedule.First().OutletClassRoster.StartDate;
                DateTime? end = grpSchedule.First().OutletClassRoster.EndDate;
                var scheds = grpSchedule.Where(e => e.IsActive).OrderBy(e => e.Day).ToList();
                int lastDay = scheds.Max(e => e.Day);
                var day = (orderDate.Date.Subtract(start.Date).TotalDays + 1) % lastDay;
                if (day == 0) day = lastDay;

                var periods = scheds.Where(e => e.Day == day).SelectMany(e => e.Periods.Where(f => f.Classes.Any(x => x.IsActive && x.ClassId == student.ClassId)));
                var sessions = periods.Select(f => f.MealSessionDetail);
                listOfSessions.AddRange(sessions);
            }

            return _mapper.Map<List<MealSessionDetailDTO>>(listOfSessions);
        }

        public async Task<List<MealSessionDetailDTO>> GetOutletMealSessions(int outletId, DateTime orderDate, DateTime? startDate, DateTime? endDate, DateTime? orderDateTo = null)
        {
            var students = await this._uow.MenuCycles.GetStudentsByOutletAsync(outletId);
            var schedules = await this._uow.MenuCycles.GetStudentsMenuCyclesAsync(students.Select(e => e.Id).ToList());
            DateTime currentDate = orderDate;

            if (startDate.HasValue)
            {
                schedules = schedules.Where(e => startDate.Value.Date >= e.OutletClassRoster.StartDate).ToList();
            }

            if (endDate.HasValue)
            {
                schedules = schedules.Where(e => endDate.Value.Date <= e.OutletClassRoster.EndDate).ToList();
            }

            var grpSchedules = schedules.GroupBy(e => e.OutletClassRosterId);
            var listOfSessions = new List<MealSessionDetail>();

            foreach (var grpSchedule in grpSchedules)
            {
                DateTime start = grpSchedule.First().OutletClassRoster.StartDate;
                DateTime? end = grpSchedule.First().OutletClassRoster.EndDate;
                currentDate = orderDate;

                var scheds = grpSchedule.Where(e => e.IsActive).OrderBy(e => e.Day).ToList();
                int lastDay = scheds.Max(e => e.Day);

                if (orderDateTo.HasValue)
                {
                    while(currentDate.Date <= orderDateTo.Value.Date)
                    {
                        if (currentDate >= start.Date && (!end.HasValue || currentDate <= end.Value.Date))
                        {
                            int day = (int)((currentDate.Date.Subtract(start.Date).TotalDays + 1) % lastDay);
                            if (day == 0) day = lastDay;

                            var periods = scheds.Where(e => e.Day == day).SelectMany(e => e.Periods.Where(f => f.Classes.Any(x => x.IsActive && students.Any(a => a.ClassId == x.ClassId))));
                            var sessions = periods.Select(f => f.MealSessionDetail).ToList();
                            listOfSessions.AddRange(sessions);
                        }

                        currentDate = currentDate.AddDays(1);
                    }
                }
                else
                {
                    if (orderDate >= start.Date && (!end.HasValue || orderDate <= end.Value.Date))
                    {
                        //var scheds = grpSchedule.Where(e => e.IsActive).OrderBy(e => e.Day).ToList();
                        //int lastDay = scheds.Max(e => e.Day);
                        int day = (int)((orderDate.Date.Subtract(start.Date).TotalDays + 1) % lastDay);
                        if (day == 0) day = lastDay;

                        var periods = scheds.Where(e => e.Day == day).SelectMany(e => e.Periods.Where(f => f.Classes.Any(x => x.IsActive && students.Any(a => a.ClassId == x.ClassId))));
                        var sessions = periods.Select(f => f.MealSessionDetail);
                        listOfSessions.AddRange(sessions);
                    }
                }
            }

            return _mapper.Map<List<MealSessionDetailDTO>>(listOfSessions.Distinct());
        }


        public async Task<List<MealSessionDetailDTO>> GetOutleOnlyMealSessions(int outletId, DateTime orderDate)
        {
            var schedules = await this._uow.MenuCycles.GetOutletsMenuCyclesAsync(outletId);
            DateTime currentDate = orderDate;

            var grpSchedules = schedules.GroupBy(e => e.OutletClassRosterId);
            var listOfSessions = new List<MealSessionDetail>();

            foreach (var grpSchedule in grpSchedules)
            {
                DateTime start = grpSchedule.First().OutletClassRoster.StartDate;
                DateTime? end = grpSchedule.First().OutletClassRoster.EndDate;
                currentDate = orderDate;

                var scheds = grpSchedule.Where(e => e.IsActive).OrderBy(e => e.Day).ToList();

                if (orderDate >= start.Date && (!end.HasValue || orderDate <= end.Value.Date))
                {
                    var periods = scheds.SelectMany(e => e.Periods);
                    var sessions = periods.Select(f => f.MealSessionDetail);
                    listOfSessions.AddRange(sessions);
                }
            }

            return _mapper.Map<List<MealSessionDetailDTO>>(listOfSessions.Distinct());
        }



        #endregion

        #region Menu Cycle Calendar

        public async Task<PagedEntity<MenuCycleCalendarDTO>> GetMenuCycleCalendarsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<MenuCycleCalendarDTO>>(await this._uow.MenuCycleCalendars.GetMenuCycleCalendarsAsync(filter));
            return result;
        }

        public async Task<MenuCycleCalendarDTO> GetMenuCycleCalendarByIdAsync(int id)
        {
            return _mapper.Map<MenuCycleCalendarDTO>(await this._uow.MenuCycleCalendars.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateMenuCycleCalendarAsync(MenuCycleCalendarDTO dto)
        {
            return await this._uow.MenuCycleCalendars.CreateAsync(_mapper.Map<MenuCycleCalendar>(dto));
        }

        public async Task<BaseOperationResponse> UpdateMenuCycleCalendarAsync(MenuCycleCalendarDTO dto)
        {
            return await this._uow.MenuCycleCalendars.UpdateAsync(_mapper.Map<MenuCycleCalendar>(dto));
        }

        public async Task<BaseOperationResponse> DeleteMenuCycleCalendarAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.MenuCycleCalendars.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Menu Group

        public async Task<PagedEntity<MenuGroupSimpleDTO>> GetMenuGroupsSimpleAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<MenuGroupSimpleDTO>>(await this._uow.MenuGroups.GetMenuGroupsAsync(filter));
            return result;
        }

        public async Task<PagedEntity<MenuGroupDTO>> GetMenuGroupsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<MenuGroupDTO>>(await this._uow.MenuGroups.GetMenuGroupsAsync(filter));
            return result;
        }

        public async Task<List<DishCycleDTO>> GetMenuGroupDishCyclesAsync(int studentId, DateTime orderDate)
        {
            var result = _mapper.Map<List<DishCycleDTO>>(await this._uow.MenuGroups.GetMenuGroupDishCyclesAsync(studentId, orderDate));
            return result.Distinct().ToList();
        }

        public async Task<List<MenuGroupDTO>> GetActiveMenuGroupDishCyclesAsync(int studentId)
        {
            var result = _mapper.Map<List<MenuGroupDTO>>(await this._uow.MenuGroups.GetActiveMenuGroupDishCyclesAsync(studentId));
            return result.Distinct().ToList();
        }

        public async Task<List<DishByDateDTO>> GetMenuGroupDishesByDateAsync(int studentId, DateTime startDate, DateTime endDate)
        {
            var result = _mapper.Map<List<DishByDateDTO>>(await this._uow.MenuGroups.GetMenuGroupDishesByDateAsync(studentId, startDate, endDate));
            return result.Distinct().ToList();
        }

        public async Task<MenuGroupDTO> GetMenuGroupByIdAsync(int id)
        {
            return _mapper.Map<MenuGroupDTO>(await this._uow.MenuGroups.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateMenuGroupAsync(MenuGroupDTO dto)
        {
            return await this._uow.MenuGroups.CreateAsync(_mapper.Map<MenuGroup>(dto));
        }

        public async Task<BaseOperationResponse> UpdateMenuGroupAsync(MenuGroupDTO dto)
        {
            return await this._uow.MenuGroups.UpdateAsync(_mapper.Map<MenuGroup>(dto));
        }

        public async Task<BaseOperationResponse> DeleteMenuGroupAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.MenuGroups.DeleteAsync(id);
            return result;
        }

        #endregion
    }
}
