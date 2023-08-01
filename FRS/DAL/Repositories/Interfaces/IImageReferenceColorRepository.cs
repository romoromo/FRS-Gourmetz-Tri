using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IImageReferenceColorRepository : IRepository<ImageReferenceColor>
    {
        Task<BaseOperationResponse> CreateAsync(ImageReferenceColor facility);
        Task<BaseOperationResponse> DeleteAsync(int ImageReferenceColorId);
        Task<ImageReferenceColor> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(ImageReferenceColor ImageReferenceColor);
        Task<PagedEntity<ImageReferenceColor>> GetImageReferenceColorsAsync(BaseFilter filter);
    }
}
