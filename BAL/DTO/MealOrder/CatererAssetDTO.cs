namespace BAL.DTO.MealOrder
{
    public class CatererAssetDTO
    {
        public int Id { get; set; }
        public string assetQRCode { get; set; }
        public string Description { get; set; }
        public int? CatererAssetTypeId { get; set; }
        public string CatererAssetTypeCode { get; set; }
        public int? FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
