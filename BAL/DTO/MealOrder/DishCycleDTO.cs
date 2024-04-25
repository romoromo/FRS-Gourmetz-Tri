using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class DishCycleSimpleDTO
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public int Sequence { get; set; }

        public int? DishTypeId { get; set; }

        public string CycleType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int NumOfDays { get; set; }

        public int NumOfSets { get; set; }

        public int? OutletProfileId { get; set; }

        public string OutletProfileName { get; set; }

        public int? CatererId { get; set; }

        public int? MealTypeId { get; set; }
    }

    public class DishCycleDTO: DishCycleSimpleDTO
    {
        public DishTypeDTO DishType { get; set; }

        public List<DishCycleScheduleDTO> Schedules { get; set; }

        public List<DishCycleBlockedDateDTO> BlockedDates { get; set; }

        public List<DishCycleScheduleSetDTO> Sets { get; set; }

        public List<OutletDishBlockedDateDTO> OutletDishBlockedDates { get; set; }
        public List<DishCyclePeriodDTO> DishCyclePeriods { get; set; }
    }
    public class DishCycleScheduleSetDTO
    {
        public int Id { get; set; }
        public int DishCycleId { get; set; }
        public int Sequence { get; set; }
        public string Label { get; set; }
        public string CycleType { get; set; }
        public int? CycleTypeId { get; set; }
        public int? CycleTypeSequence { get; set; }
        public string CycleTypeLabel { get; set; }
        public int? MealTypeId { get; set; }
        public string MealTypeLabel { get; set; }
        public double Price { get; set; }
        public List<DishCycleScheduleDetailMenuDTO> Menus { get; set; }
        public List<OutletDishCyclePeriodMenuDTO> ExcludedMenus { get; set; }
    }

    public class DishCycleScheduleDTO
    {
        public int Id { get; set; }
        public int DishCycleId { get; set; }
        public int Day { get; set; }
        public List<DishCycleScheduleDetailDTO> Details { get; set; }
    }

    public class DishCycleScheduleDetailDTO
    {
        public int Id { get; set; }
        public int DishCycleScheduleId { get; set; }
        public string Label { get; set; }
        public int Sequence { get; set; }
        public int? DishCycleId { get; set; }
        public DishCycleDTO DishCycle { get; set; }
        public List<DishCycleScheduleDetailMenuDTO> Menus { get; set; }
    }

    public class DishCycleScheduleDetailMenuDTO
    {
        public int DishCycleScheduleDetailId { get; set; }
        public int? DishId { get; set; }
        public string DishLabel { get; set; }
        public string DishCode { get; set; }
        public int? DishCycleId { get; set; }
        public string DishCycleLabel { get; set; }
        public string FilePath { get; set; }
        public string ProductionPicturePath { get; set; }
        public DishDTO DishObj { get; set; }
    }

    public class MealCreditSetDTO
    {
        public int MealTypeId { get; set; }
        public int DishCycleId { get; set; }
        public string Setname { get; set; }
        public List<DishCycleScheduleDetailMenuDTO> Menus { get; set; }
        public List<OutletDishCyclePeriodMenuDTO> ExcludedMenus { get; set; }
    }

    public class DishByDateDTO
    {
        public DateTime Date { get; set; }
        public List<DishCycleDTO> DishCycles { get; set; }
        public List<DishCycleScheduleSetDTO> DishSets { get; set; }
        public List<DishCycleScheduleDetailMenuDTO> Menus { get; set; }
    }
}
