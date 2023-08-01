using System.Threading.Tasks;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;

namespace DAL.Repositories.Interfaces.MealOrder
{
    public interface ICatererInfoRepository
    {
        Task<BaseOperationResponse> CreateAsync(CatererInfo data);
        Task<BaseOperationResponse> Delete(CatererInfo data);
        Task<BaseOperationResponse> DeleteAsync(int dataId);
        Task<CatererInfo> GetByIdAsync(int id);
        Task<PagedEntity<CatererInfo>> GetCatererInfosAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateAsync(CatererInfo data);
        Task<BaseOperationResponse> RequestOutlet(int catererId, int outletId, string status, bool isRsp, int? outletProfileId);
    }
}