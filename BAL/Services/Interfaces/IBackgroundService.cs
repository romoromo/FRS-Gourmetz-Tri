using System.Threading;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IBackgroundService
    {
        Task FASRechargeable(CancellationToken ct = default);
    }
}
