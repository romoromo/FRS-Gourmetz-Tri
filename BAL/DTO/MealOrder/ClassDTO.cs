using DAL.Core;

namespace BAL.DTO.MealOrder
{
    public class ClassDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ClassLevelId { get; set; }

        public string ClassLevelName { get; set; }
        public MealCollectionType? MealCollectionType { get; set; }
    }
}
