using BAL.DTO;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IImageReferenceColorService
    {
        Task<BaseOperationResponse> CreateAsync(ImageReferenceColorDTO dto);
        Task<BaseOperationResponse> DeleteAsync(int id);
        Task<ImageReferenceColorDTO> GetByIdAsync(int id);
        Task<PagedEntity<ImageReferenceColorDTO>> GetImageReferenceColorsAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(ImageReferenceColorDTO dto);
    }
}
