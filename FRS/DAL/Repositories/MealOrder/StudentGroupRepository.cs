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
using System.Transactions;

namespace DAL.Repositories.MealOrder
{
    public class StudentGroupRepository : Repository<StudentGroup>, IStudentGroupRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public StudentGroupRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<StudentGroup>> GetStudentGroupsAsync(BaseFilter filter)
        {
            IQueryable<StudentGroup> query = _appContext.StudentGroups;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<StudentGroup> GetByIdAsync(int id)
        {
            var group = id > 0 ? await GetAsync(id) :
                            await _appContext.StudentGroups.FirstOrDefaultAsync(e => e.IsActive);
            return group;
        }

        public async Task<BaseOperationResponse> CreateAsync(StudentGroup group, List<StudentGroupDetail> groupDetails)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                var f = await AddAsync(group);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                    scope.Complete();
                }
                else
                {
                    result.Message = "Failed to save!";

                }
            }
            return result;

        }

        public async Task<BaseOperationResponse> UpdateAsync(StudentGroup group, List<StudentGroupDetail> groupDetails)
        {
            var result = new BaseOperationResponse();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                            TransactionScopeAsyncFlowOption.Enabled))
            {
                var f = await GetSingleOrDefaultAsync(e => e.Id == group.Id);

                var detailsToDelete = this._appContext.StudentGroupDetails.Where(x => x.StudentGroupId == f.Id);

                this._appContext.StudentGroupDetails.RemoveRange(detailsToDelete);

                if (groupDetails != null)
                {
                    groupDetails.ForEach(e =>
                    {
                        var sc = this._appContext.StudentGroupDetails.FirstOrDefault(x => x.Id == e.Id);
                        if (sc != null)
                        {
                            sc.StudentId = e.StudentId;
                            sc.StudentGroupId = e.StudentGroupId;
                            this._appContext.StudentGroupDetails.Update(sc);
                        }
                        else
                        {
                            this._appContext.StudentGroupDetails.Add(e);
                        }
                    });
                }

                f.CopyFrom(group);

                Update(f);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;

                    scope.Complete();
                }
                else
                {
                    result.Message = "Failed to save!";
                    result.IsSuccess = false;
                }

            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(StudentGroup group)
        {
            var result = new BaseOperationResponse();
            var f = await GetSingleOrDefaultAsync(e => e.Id == group.Id);
            
            f.CopyFrom(group);

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

        public async Task<BaseOperationResponse> DeleteAsync(int groupId)
        {
            var result = new BaseOperationResponse();
            var group = await GetSingleOrDefaultAsync(r => r.Id == groupId);

            if (group != null)
                return await Delete(group);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(StudentGroup group)
        {
            var result = new BaseOperationResponse();
            SoftDelete(group);
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
