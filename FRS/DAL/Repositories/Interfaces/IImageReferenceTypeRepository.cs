using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IImageReferenceTypeRepository : IRepository<ImageReferenceType>
    {
        Task<BaseOperationResponse> CreateAsync(ImageReferenceType facility);
        Task<BaseOperationResponse> DeleteAsync(int ImageReferenceTypeId);
        Task<ImageReferenceType> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(ImageReferenceType ImageReferenceType);
        Task<PagedEntity<ImageReferenceType>> GetImageReferenceTypesAsync(BaseFilter filter);
    }
}
