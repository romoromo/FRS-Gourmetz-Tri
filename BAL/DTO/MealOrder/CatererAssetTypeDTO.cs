namespace BAL.DTO.MealOrder
{
    public class CatererAssetTypeDTO
    {
        public int Id { get; set; }

        public int? CatererInfoId { get; set; }

        public string Code { get; set; }
        public string Description { get; set; }

        public int? FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }        
    }
}
