using DAL.Core;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IUserActivityRepository
    {
        Task<BaseOperationResponse> CreateAsync(string message, int? userId);
    }
}
