using DAL.Core;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface ISignagePublicationRepository : IRepository<SignagePublication>
    {
        IEnumerable<SignagePublication> All();
        Task<List<SignagePublication>> GetSignagePublicationsLoadRelatedAsync(int page, int pageSize, int? institutionId = null);
        Task<BaseOperationResponse> CreateAsync(SignagePublication signagePublication);
        Task<BaseOperationResponse> DeleteAsync(int signagePublicationId);
        Task<SignagePublication> GetByIdAsync(int id);
        Task<BaseOperationResponse> UpdateAsync(SignagePublication signagePublication);
        Task<byte[]> GetHistory(int? publicationId);
    }
}
