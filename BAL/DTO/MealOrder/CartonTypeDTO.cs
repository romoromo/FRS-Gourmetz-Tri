using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAL.DTO.MealOrder
{
    public class CartonTypeDTO
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public int? InstitutionId { get; set; }

        public string InstitutionName { get; set; }

        public string Details { get; set; }

        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }

        public int? CatererInfoId { get; set; }

        public string CatererInfoName { get; set; }

        
    }

    public class CartonTypeDetailsDTO : CartonTypeDTO
    {
        public List<CartonAssetDTO> CartonAssets { get; set; }
    }
}
