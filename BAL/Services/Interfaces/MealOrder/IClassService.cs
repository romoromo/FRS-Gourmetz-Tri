using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces.MealOrder
{
    public interface IClassService
    {
        Task<BaseOperationResponse> CreateClassAsync(ClassDTO dto);
        Task<BaseOperationResponse> DeleteClassAsync(int id);
        Task<ClassDTO> GetClassByIdAsync(int id);
        Task<PagedEntity<ClassDTO>> GetClassesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateClassAsync(ClassDTO dto);

        Task<BaseOperationResponse> CreateClassLevelAsync(ClassLevelDTO dto);
        Task<BaseOperationResponse> DeleteClassLevelAsync(int id);
        Task<ClassLevelDTO> GetClassLevelByIdAsync(int id);
        Task<PagedEntity<ClassLevelDTO>> GetClassLevelsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateClassLevelAsync(ClassLevelDTO dto);

        Task<BaseOperationResponse> CreateClassBatchAsync(ClassBatchDTO dto);
        Task<BaseOperationResponse> DeleteClassBatchAsync(int id);
        Task<ClassBatchDTO> GetClassBatchByIdAsync(int id);
        Task<PagedEntity<ClassBatchDTO>> GetClassBatchesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateClassBatchAsync(ClassBatchDTO dto);

        Task<BaseOperationResponse> CreateOutletClassRosterAsync(OutletClassRosterDTO dto);
        Task<List<OutletClassRosterDTO>> GetOutletClassRosterByIdAsync(int outletId, int catererId, int mealSessionId);
        Task<BaseOperationResponse> UpdateOutletClassRosterAsync(OutletClassRosterDTO outletClassRoster);
        Task<OutletClassRosterDTO> GetOutletClassRosterByIdAsync(int id);
        Task<PagedEntity<OutletClassRosterDTO>> GetOutletClassRostersAsync(ClassRosterFilter filter);
        Task<BaseOperationResponse> DeleteOutletClassRosterAsync(int id);

        Task<MealSessionDetailDTO> GetCurrentOrderMealSessionAsync(int? outletId, DateTime orderDate, int mealSessionId, int classId);
        Task<byte[]> GenerateClassRostersXls(ClassRosterFilter filter);

        Task<BaseOperationResponse> CreateDispenserOutletAsync(DispenserOutletDTO dto);
        Task<BaseOperationResponse> DeleteDispenserOutletAsync(int id);
        Task<DispenserOutletDTO> GetDispenserOutletByIdAsync(int id);
        Task<PagedEntity<DispenserOutletDTO>> GetDispenserOutletsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateDispenserOutletAsync(DispenserOutletDTO dto);

        Task<PagedEntity<PLCDTO>> GetPLCPagedAsync(BaseFilter filter);
        Task<PLCDTO> GetPLCByIdAsync(int id);
        Task<BaseOperationResponse> CreatePLCAsync(PLCDTO dto);
        Task<BaseOperationResponse> UpdatePLCAsync(PLCDTO dto);
        Task<BaseOperationResponse> DeletePLCAsync(int id);

        Task<List<MealSessionDetailDTO>> GetOutletPeriodMealSessions(int outletId);
    }
}