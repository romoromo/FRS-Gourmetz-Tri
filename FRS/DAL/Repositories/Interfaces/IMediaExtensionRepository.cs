using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IMediaExtensionRepository : IRepository<MediaExtension>
    {
        IEnumerable<MediaExtension> All();
        Task<List<MediaExtension>> GetMediaExtensionsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(MediaExtension mediaExtension);
        Task<BaseOperationResponse> DeleteAsync(int mediaExtensionId);
        Task<MediaExtension> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(MediaExtension mediaExtension);
    }
}
