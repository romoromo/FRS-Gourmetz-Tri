using DAL.Core;
using DAL.Core.DTO;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IPlaylistRepository : IRepository<Playlist>
    {
        IEnumerable<Playlist> All();
        Task<List<Playlist>> GetPlaylistsLoadRelatedAsync(int page, int pageSize, int? institutionId = null, int? userId = null, List<int> imageIds = null);
        Task<BaseOperationResponse> CreateAsync(Playlist playlist);
        //Task<TestDeleteResult> TestCanDeleteAsync(int playlistId);
        Task<BaseOperationResponse> DeleteAsync(int playlistId);
        Task<Playlist> GetByIdAsync(int id);
        //Task<Playlist> GetByCode(int playlistId, string code);
        Task<BaseOperationResponse> UpdateAsync(Playlist playlist);
    }
}
