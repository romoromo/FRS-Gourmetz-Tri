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
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces.MealOrder;

namespace DAL.Repositories.MealOrder
{
    public class EmailConfirmRepository : Repository<EmailConfirm>, IEmailConfirmRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public EmailConfirmRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<EmailConfirm>> GetEmailConfirmsAsync(BaseFilter filter)
        {
            IQueryable<EmailConfirm> query = _appContext.EmailConfirms;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<EmailConfirm> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<EmailConfirm> GetByEmailAsync(string email)
        {
            return await GetSingleOrDefaultAsync(e => e.Email == email);
        }

        public async Task<BaseOperationResponse> CreateAsync(EmailConfirm ec)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(ec);
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

        public async Task<BaseOperationResponse> UpdateAsync(EmailConfirm ec)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == ec.Id);

            f.CopyFrom(ec);

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


        public async Task<BaseOperationResponse> DeleteAsync(int ecId)
        {
            var result = new BaseOperationResponse();
            var ec = await GetSingleOrDefaultAsync(r => r.Id == ecId);

            if (ec != null)
                return await Delete(ec);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(EmailConfirm ec)
        {
            var result = new BaseOperationResponse();
            SoftDelete(ec);
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
