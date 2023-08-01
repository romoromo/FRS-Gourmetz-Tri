using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IImageRepository : IRepository<ImageFile>
    {
        IEnumerable<ImageFile> All();
        Task<List<ImageFile>> GetImagesLoadRelatedAsync(int page, int pageSize, int? institutionId = null, string institutionCode = null, int? userId = null);
        Task<List<ImageFile>> GetAllAsync(int? institutionId = null, string institutionCode = null);
        Task<BaseOperationResponse> CreateAsync(ImageFile image, string folder);
        Task<BaseOperationResponse> DeleteAsync(int imageId);
        Task<ImageFile> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(ImageFile image, string folder);
    }
}
