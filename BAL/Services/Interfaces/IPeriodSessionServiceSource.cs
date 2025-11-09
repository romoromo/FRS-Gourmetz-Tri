using BAL.DTO.MealOrder;
using BAL.Services.Interfaces.MealOrder;
using DAL.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IPeriodSessionServiceSource
    {
        Task<List<MealSessionDetailDTO>> GetOutletMealSessions(OutletMealSessionInput input);
    }

    public interface IOutletMealSessionResolver
    {
        Task<List<MealSessionDetailDTO>> GetOutletMealSessions(OutletMealSessionInput input);
    }

    public class OutletMealSessionResolver : IOutletMealSessionResolver
    {
        private readonly IDeliveryService _deliveryService;
        private readonly PeriodSessionByClassLevel _periodSessionByClassLevel;
        private readonly PeriodSessionByClassRoaster _periodSessionByClassRoaster;
        private readonly PeriodSessionByStudentSelect _periodSessionByStudentSelect;
        public OutletMealSessionResolver(
            IDeliveryService deliveryService,
            PeriodSessionByClassLevel periodSessionByClassLevel,
            PeriodSessionByClassRoaster periodSessionByClassRoaster,
            PeriodSessionByStudentSelect periodSessionByStudentSelect)
        {
            _deliveryService = deliveryService;
            _periodSessionByClassLevel = periodSessionByClassLevel;
            _periodSessionByClassRoaster = periodSessionByClassRoaster;
            _periodSessionByStudentSelect = periodSessionByStudentSelect;
        }

        public async Task<List<MealSessionDetailDTO>> GetOutletMealSessions(OutletMealSessionInput input)
        {
            var outlet = await _deliveryService.GetOutletByIdAsync(input.OutletId);
            if (outlet == null)
                throw new InvalidOperationException($"GetOutletMealSessions - Outlet NOT FOUND.");
            //TBD: Handle null meal collection type
            outlet.MealCollectionType = MealCollectionType.BY_CLASS_ROASTER;

            IPeriodSessionServiceSource service = outlet.MealCollectionType switch
            {
                MealCollectionType.BY_CLASS_LEVEL => _periodSessionByClassLevel,
                MealCollectionType.BY_CLASS_ROASTER => _periodSessionByClassRoaster,
                MealCollectionType.STUDENT_SELECTS => _periodSessionByStudentSelect,
                _ => throw new InvalidOperationException($"Unknown source type: {outlet.MealCollectionType}")
            };
            return await service.GetOutletMealSessions(input);
        }
    }

    public class PeriodSessionByClassLevel : IPeriodSessionServiceSource
    {
        public IClassService _classService;
        public PeriodSessionByClassLevel(IClassService classService)
        {
            _classService = classService;
        }

        public async Task<List<MealSessionDetailDTO>> GetOutletMealSessions(OutletMealSessionInput input)
        {
            return await _classService.GetOutletPeriodMealSessions(input.OutletId);
        }
    }

    public class PeriodSessionByClassRoaster : IPeriodSessionServiceSource
    {
        private IMenuService _menuService;
        public PeriodSessionByClassRoaster(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<List<MealSessionDetailDTO>> GetOutletMealSessions(OutletMealSessionInput input)
        {
            return await _menuService.GetOutletMealSessions(input.OutletId, input.OrderDate, null, input.OrderDateTo);
        }
    }

    public class PeriodSessionByStudentSelect : IPeriodSessionServiceSource
    {
        private IMealService _mealService;
        public PeriodSessionByStudentSelect(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<List<MealSessionDetailDTO>> GetOutletMealSessions(OutletMealSessionInput input)
        {
            return await _mealService.GetMealSessionByOutletId(input.OutletId);
        }
    }
}
