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
    public interface IImageReferenceTypeService
    {
        Task<BaseOperationResponse> CreateAsync(ImageReferenceTypeDTO dto);
        Task<BaseOperationResponse> DeleteAsync(int id);
        Task<ImageReferenceTypeDTO> GetByIdAsync(int id);
        Task<PagedEntity<ImageReferenceTypeDTO>> GetImageReferenceTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(ImageReferenceTypeDTO dto);
    }
}
