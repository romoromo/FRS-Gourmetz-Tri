using BAL.DTO.MealOrder;
using BAL.Services.Interfaces.MealOrder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IMealSessionResolver
    {
        Task<List<MealSessionDetailDTO>> GetMealSessionsByOutlet(MealSessionByOutletInput input);
        Task<List<MealSessionDetailDTO>> GetMealSessionsByStudentGroup(MealSessionByStudentGrouptInput input);
    }

    public class MealSessionResolver : IMealSessionResolver
    {
        private readonly IDeliveryService _deliveryService;
        private readonly IClassService _classService;
        private readonly MealSessionByClassLevel _mealSessionByClassLevel;
        private readonly MealSessionByClassRoaster _mealSessionByClassRoaster;
        private readonly MealSessionByStudentSelect _mealSessionByStudentSelect;
        public MealSessionResolver(
            IDeliveryService deliveryService,
            IClassService classService,
            MealSessionByClassLevel periodSessionByClassLevel,
            MealSessionByClassRoaster periodSessionByClassRoaster,
            MealSessionByStudentSelect periodSessionByStudentSelect)
        {
            _deliveryService = deliveryService;
            _classService = classService;

            _mealSessionByClassLevel = periodSessionByClassLevel;
            _mealSessionByClassRoaster = periodSessionByClassRoaster;
            _mealSessionByStudentSelect = periodSessionByStudentSelect;
        }

        public async Task<List<MealSessionDetailDTO>> GetMealSessionsByOutlet(MealSessionByOutletInput input)
        {
            var outlet = await _deliveryService.GetOutletByIdAsync(input.OutletId);
            if (outlet == null)
                throw new InvalidOperationException($"GetOutletMealSessions - Outlet NOT FOUND.");
            //TBD: Handle null meal collection type
            IPeriodSessionServiceSource service = _mealSessionByClassRoaster;
            //outlet.MealCollectionType = MealCollectionType.BY_CLASS_ROASTER;
            //
            //IPeriodSessionServiceSource service = outlet.MealCollectionType switch
            //{
            //    MealCollectionType.BY_CLASS_LEVEL => _periodSessionByClassLevel,
            //    MealCollectionType.BY_CLASS_ROASTER => _periodSessionByClassRoaster,
            //    MealCollectionType.STUDENT_SELECTS => _periodSessionByStudentSelect,
            //    _ => throw new InvalidOperationException($"Unknown source type: {outlet.MealCollectionType}")
            //};
            return await service.Resolve(input);
        }

        public async Task<List<MealSessionDetailDTO>> GetMealSessionsByStudentGroup(MealSessionByStudentGrouptInput input)
        {
            var results = new List<MealSessionDetailDTO>();

            var classes = await _classService.GetClassByStudentGroupId(input.StudentGroupId);
            foreach (var classInfo in classes)
            {
                IPeriodSessionServiceSource serviceSource;
                if (classInfo.MealCollectionType == DAL.Core.MealCollectionType.BY_CLASS_ROASTER)
                {
                    serviceSource = _mealSessionByClassRoaster;
                    results.AddRange(await serviceSource.Resolve(input));
                }
                else if (classInfo.MealCollectionType == DAL.Core.MealCollectionType.BY_CLASS_LEVEL)
                {
                    serviceSource = _mealSessionByClassLevel;

                    input.ClassLevelId = classInfo.ClassLevelId;
                    results.AddRange(await serviceSource.Resolve(input));
                }
                else if (classInfo.MealCollectionType == DAL.Core.MealCollectionType.STUDENT_SELECTS)
                {
                    serviceSource = _mealSessionByStudentSelect;
                    results.AddRange(await serviceSource.Resolve(input));
                }
            }

            return results;
        }
    }



    public interface IPeriodSessionServiceSource
    {
        Task<List<MealSessionDetailDTO>> Resolve(MealSessionByOutletInput input);
    }

    public class MealSessionByClassLevel : IPeriodSessionServiceSource
    {
        public IClassService _classService;
        public MealSessionByClassLevel(IClassService classService)
        {
            _classService = classService;
        }

        public async Task<List<MealSessionDetailDTO>> Resolve(MealSessionByOutletInput input)
        {
            return await _classService.GetMealSessionsByClassLevel(input.ClassLevelId, input.OutletId);
        }
    }

    public class MealSessionByClassRoaster : IPeriodSessionServiceSource
    {
        private IMenuService _menuService;
        public MealSessionByClassRoaster(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<List<MealSessionDetailDTO>> Resolve(MealSessionByOutletInput input)
        {
            return await _menuService.GetOutletMealSessions(input.OutletId, input.OrderDate, null, input.OrderDateTo);
        }
    }

    public class MealSessionByStudentSelect : IPeriodSessionServiceSource
    {
        private IMealService _mealService;
        public MealSessionByStudentSelect(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<List<MealSessionDetailDTO>> Resolve(MealSessionByOutletInput input)
        {
            return await _mealService.GetMealSessionByOutletId(input.OutletId);
        }
    }
}
