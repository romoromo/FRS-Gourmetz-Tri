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

namespace DAL.Repositories.MealOrder
{
    public class ContactUsSubjectRepository : Repository<ContactUsSubject>, IContactUsSubjectRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public ContactUsSubjectRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<ContactUsSubject>> GetContactUsSubjectsAsync(BaseFilter filter)
        {
            IQueryable<ContactUsSubject> query = _appContext.ContactUsSubjects
                .Include(e => e.Institution);

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<ContactUsSubject>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = await query.ToListAsync()
            };

            return result;
        }

        #endregion
        public async Task<ContactUsSubject> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(ContactUsSubject contactUsSubject)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(contactUsSubject);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save voucher type!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(ContactUsSubject contactUsSubject)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == contactUsSubject.Id);

            f.CopyFrom(contactUsSubject);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save voucher type!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int contactUsSubjectId)
        {
            var result = new BaseOperationResponse();
            var contactUsSubject = await GetSingleOrDefaultAsync(r => r.Id == contactUsSubjectId);

            if (contactUsSubject != null)
                return await Delete(contactUsSubject);

            result.IsSuccess = false;
            result.Message = "Voucher type not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(ContactUsSubject contactUsSubject)
        {
            var result = new BaseOperationResponse();
            SoftDelete(contactUsSubject);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete voucher type!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
