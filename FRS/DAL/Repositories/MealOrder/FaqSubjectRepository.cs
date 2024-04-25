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
    public class FaqSubjectRepository : Repository<FaqSubject>, IFaqSubjectRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public FaqSubjectRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<FaqSubject>> GetFaqSubjectsAsync(BaseFilter filter)
        {
            IQueryable<FaqSubject> query = _appContext.FaqSubjects
                .Include(e => e.Institution)
                .Include(e => e.FaqDetails);

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<FaqSubject>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = await query.ToListAsync()
            };

            return result;
        }

        #endregion
        public async Task<FaqSubject> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(FaqSubject faqSubject)
        {
            var result = new BaseOperationResponse();
            // get last order

            var subj = (await FindAsync(e=> e.IsActive)).OrderByDescending(e => e.Order).FirstOrDefault();
            int order = subj == null ? 1 : subj.Order + 1;

            faqSubject.Order = order;
            var f = await AddAsync(faqSubject);
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

        public async Task<BaseOperationResponse> UpdateAsync(FaqSubject faqSubject)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == faqSubject.Id);

            f.CopyFrom(faqSubject);
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


        public async Task<BaseOperationResponse> DeleteAsync(int faqSubjectId)
        {
            var result = new BaseOperationResponse();
            var faqSubject = await GetSingleOrDefaultAsync(r => r.Id == faqSubjectId);

            if (faqSubject != null)
                return await Delete(faqSubject);

            result.IsSuccess = false;
            result.Message = "Subject not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(FaqSubject faqSubject)
        {
            var result = new BaseOperationResponse();
            SoftDelete(faqSubject);
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

        public async Task<BaseOperationResponse> OrderAsync(int id, bool isAsc)
        {
            var result = new BaseOperationResponse();
            var faqSubject = await GetSingleOrDefaultAsync(r => r.Id == id);

            if (faqSubject != null)
            {
                // get the others
                var subjects = (await FindAsync(e => e.IsActive)).OrderBy(e => e.Order).ToList();
                int index = subjects.FindIndex(e => e.Id == id);

                if (index < 1 && isAsc)
                {
                    result.Message = "Not allowed";
                    return result;
                }

                if (index >= subjects.Count - 1 && !isAsc)
                {
                    result.Message = "Not allowed";
                    return result;
                }

                int currentOrder = faqSubject.Order;
                FaqSubject recordToSwap = null;

                if (isAsc)
                {
                    recordToSwap = subjects.OrderByDescending(e => e.Order).FirstOrDefault(r => r.Order < faqSubject.Order);
                    faqSubject.Order = recordToSwap.Order;
                    recordToSwap.Order = currentOrder;
                }
                else
                {
                    recordToSwap = subjects.OrderBy(e => e.Order).FirstOrDefault(r => r.Order > faqSubject.Order);
                    faqSubject.Order = recordToSwap.Order;
                    recordToSwap.Order = currentOrder;
                }

                await UpdateAsync(recordToSwap);
                await UpdateAsync(faqSubject);

                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    return result;
                }
            }

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
