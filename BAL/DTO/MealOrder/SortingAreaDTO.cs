namespace BAL.DTO.MealOrder
{
    public class SortingAreaDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public int RouteId { get; set; }
        public string RouteColor { get; set; }
        public string RouteDetail { get; set; }
    }
}
