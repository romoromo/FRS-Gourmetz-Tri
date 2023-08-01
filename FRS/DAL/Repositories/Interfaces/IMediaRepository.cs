using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IMediaRepository : IRepository<Media>
    {
        Task<Media> GetByCodeAsync(string code);
        IEnumerable<Media> All();
        Task<List<Media>> GetMediasLoadRelatedAsync(int page, int pageSize);
        Task<BaseOperationResponse> GetApiMedias(int? mediaId = null, string name = null);
        Task<BaseOperationResponse> CreateAsync(Media media, string[] roles, string[] userGroups);
        Task<bool> TestCanDeleteAsync(int mediaId);
        Task<bool> TestCanCreateAsync(string name);
        Task<BaseOperationResponse> DeleteAsync(int mediaId);
        Task<Media> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(Media media, string[] roles, string[] userGroups);
    }
}
