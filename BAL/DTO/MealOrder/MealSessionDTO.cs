using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class MealSessionDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Sequence { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? MealPeriodId { get; set; }

        public int? CatererId { get; set; }

        public int? OutletId { get; set; }

        public string MealPeriodName { get; set; }

        public string OutletName { get; set; }

        public bool IsActive { get; set; }

        public OutletDTO Outlet { get; set; }

        public List<MealSessionDetailDTO> Details { get; set; }

        public List<OutletClassRosterDTO> ClassRosters { get; set; }
    }

    public class MealSessionSimpleDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int? MealPeriodId { get; set; }
        public List<MealSessionDetailSimpleDTO> Details { get; set; }
    }

    public class MealSessionDetailSimpleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MealSessionMealPeriodDTO
    {
        public MealPeriodDTO MealPeriod { get; set; }
        public List<MealSessionDTO> MealSessions { get; set; }
    }
}
