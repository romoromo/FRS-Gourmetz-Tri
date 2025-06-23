using System;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;

namespace DAL.Repositories
{
    public class UserActivityRepository : IUserActivityRepository
    {
        private ApplicationDbContext _appContext;

        public UserActivityRepository(ApplicationDbContext context)
        {
            _appContext = context;
        }

        public async Task<BaseOperationResponse> CreateAsync(string message,int? userId)
        {
            var result = new BaseOperationResponse();
            var entityData = new UserActivity
            {
                Message = message,
                CreatedBy = userId,
                CreatedDate = DateTime.Now,
            };
            await _appContext.UserActivities.AddAsync(entityData);

            result.IsSuccess = true;
            result.Message = "Success";
            return result;
        }
    }
}
