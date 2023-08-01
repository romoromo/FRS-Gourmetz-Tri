using DAL;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.Jobs
{
    internal interface IScopedJobProcessingService
    {
        void DoWork();
    }

    internal class ScopedJobProcessingService : IScopedJobProcessingService
    {
        private readonly ILogger _logger;
        private IUnitOfWork _unitOfWork;
        public ScopedJobProcessingService(IUnitOfWork unitOfWork, ILogger<ScopedJobProcessingService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public void DoWork()
        {
            _logger.LogInformation("Scoped Processing Service is working.");
            _unitOfWork.Reservations.ProcessExpiredBookings();
        }
    }
}
