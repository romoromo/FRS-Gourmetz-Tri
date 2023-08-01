using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;

namespace DAL.Repositories
{
    public class EmailQueueRepository : Repository<EmailQueue>, IEmailQueueRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public EmailQueueRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<EmailQueue>> GetEmailQueuesAsync(BaseFilter filter)
        {
            IQueryable<EmailQueue> query = _appContext.EmailQueues
                .Include(e => e.Institution);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion

        public async Task<List<EmailQueue>> GetAllUnsentEmailAsync()
        {
            IQueryable<EmailQueue> query = _appContext.EmailQueues.Where(e => e.IsActive && !e.IsSent);
            return await query.ToListAsync();
        }

        public async Task<EmailQueue> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(EmailQueue emailQueue)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(emailQueue);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(EmailQueue emailQueue)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == emailQueue.Id);

            f.CopyFrom(emailQueue);

            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int emailQueueId)
        {
            var result = new BaseOperationResponse();
            var emailQueue = await GetSingleOrDefaultAsync(r => r.Id == emailQueueId);

            if (emailQueue != null)
                return await Delete(emailQueue);

            result.IsSuccess = false;
            result.Message = "Email Queue not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(EmailQueue emailQueue)
        {
            var result = new BaseOperationResponse();
            SoftDelete(emailQueue);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
