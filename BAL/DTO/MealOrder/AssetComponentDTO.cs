namespace BAL.DTO.MealOrder
{
    public class AssetComponentDTO
    {
        public int Id { get; set; }
        public int? CatererAssetId { get; set; }
        public string CatererAssetCode { get; set; }

        public string Description { get; set; }
        public int Qty { get; set; }
        public string Remarks { get; set; }

        public int? FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
