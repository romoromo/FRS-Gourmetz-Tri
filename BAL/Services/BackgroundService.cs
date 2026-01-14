using BAL.Services.Interfaces;
using DAL;
using Hangfire;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class BackgroundService : IBackgroundService
    {
        private IUnitOfWork _uow;
        readonly ILogger _logger;        

        public BackgroundService(IUnitOfWork uow, ILogger<BackgroundService> logger)
        {
            _uow = uow; 
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task FASRechargeable(CancellationToken ct = default)
        {
            _logger.LogInformation("Background Service: FAS Rechargeable started.");
            await _uow.StudentWalletTransactions.FASRechargeable(ct);
            _logger.LogInformation("Background Service: FAS Rechargeable completed.");
        }
    }
}
