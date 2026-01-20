using BAL.DTO.MealOrder;
using BAL.Services.Interfaces.MealOrder;
using DAL.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IMealSessionResolver
    {
        Task<List<MealSessionDetailDTO>> GetMealSessionsByOutlet(MealSessionByOutletInput input);
        Task<List<MealSessionDetailDTO>> GetMealSessionsByStudentGroup(MealSessionByStudentGrouptInput input);
        Task<List<MealSessionDetailDTO>> GetMealSessionsByStudent(int studentId, DateTime orderDate);
        Task<List<MealSessionDetailDTO>> GetMealSessionsByClass(int classId, int outletId, DateTime orderDate);
    }

    public class MealSessionResolver : IMealSessionResolver
    {
        private readonly IDeliveryService _deliveryService;
        private readonly IClassService _classService;

        private readonly MealSessionByClassLevel _mealSessionByClassLevel;
        private readonly MealSessionByClassRoaster _mealSessionByClassRoaster;
        private readonly MealSessionByStudentSelect _mealSessionByStudentSelect;

        private readonly Dictionary<MealCollectionType, IPeriodSessionServiceSource> _mealSources;
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

            _mealSources = new Dictionary<MealCollectionType, IPeriodSessionServiceSource>
            {
                { MealCollectionType.BY_CLASS_ROASTER, _mealSessionByClassRoaster },
                { MealCollectionType.BY_CLASS_LEVEL,   _mealSessionByClassLevel },
                { MealCollectionType.STUDENT_SELECTS,  _mealSessionByStudentSelect }
            };
        }

        private IPeriodSessionServiceSource GetService(MealCollectionType type)
        {
            if (_mealSources.TryGetValue(type, out var service))
                return service;

            return null;
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
                if (!classInfo.MealCollectionType.HasValue)
                    continue;

                var service = GetService(classInfo.MealCollectionType.Value);
                if (service == null) continue;

                var request = new MealSessionByStudentGrouptInput
                {
                    StudentGroupId = input.StudentGroupId,
                    ClassLevelId = classInfo.ClassLevelId,
                    OutletId = input.OutletId,
                    OrderDate = input.OrderDate,
                    OrderDateTo = input.OrderDateTo
                };

                var data = await service.Resolve(request);
                results.AddRange(data);
            }

            return results;
        }

        public async Task<List<MealSessionDetailDTO>> GetMealSessionsByStudent(int studentId, DateTime orderDate)
        {
            var (Class, outletId) = await _classService.GetClassByStudentId(studentId);
            if (!Class.MealCollectionType.HasValue) return [];

            var service = GetService(Class.MealCollectionType.Value);
            if (service == null) return [];
            var request = new MealSessionByOutletInput
            {
                ClassLevelId = Class?.ClassLevelId ?? 0,
                OrderDate = orderDate,
                OrderDateTo = null,
                OutletId = outletId
            };

            return await service.Resolve(request);
        }

        public async Task<List<MealSessionDetailDTO>> GetMealSessionsByClass(int classId,int outletId, DateTime orderDate)
        {
            var Class = await _classService.GetClassByIdAsync(classId);
            if (!Class.MealCollectionType.HasValue) return [];

            var service = GetService(Class.MealCollectionType.Value);
            if (service == null) return [];
            var request = new MealSessionByOutletInput
            {
                ClassLevelId = Class?.ClassLevelId ?? 0,
                OrderDate = orderDate,
                OrderDateTo = null,
                OutletId = outletId
            };

            return await service.Resolve(request);
        }
    }


    public interface IPeriodSessionServiceSource
    {
        Task<List<MealSessionDetailDTO>> Resolve(MealSessionByOutletInput input);
    }

    public class MealSessionByClassLevel(IClassService classService) : IPeriodSessionServiceSource
    {
        private readonly IClassService _classService = classService;

        public async Task<List<MealSessionDetailDTO>> Resolve(MealSessionByOutletInput input)
        {
            return await _classService.GetMealSessionsByClassLevel(input.ClassLevelId, input.OutletId, input.OrderDate);
        }
    }

    public class MealSessionByClassRoaster(IMenuService menuService) : IPeriodSessionServiceSource
    {
        private readonly IMenuService _menuService = menuService;

        public async Task<List<MealSessionDetailDTO>> Resolve(MealSessionByOutletInput input)
        {
            return await _menuService.GetOutletMealSessions(input.OutletId, input.OrderDate, null, input.OrderDateTo);
        }
    }

    public class MealSessionByStudentSelect(IMealService mealService) : IPeriodSessionServiceSource
    {
        private readonly IMealService _mealService = mealService;

        public async Task<List<MealSessionDetailDTO>> Resolve(MealSessionByOutletInput input)
        {
            return await _mealService.GetMealSessionByOutletId(input.OutletId);
        }
    }
}
