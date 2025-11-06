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
using NPOI.OpenXmlFormats.Dml;

namespace DAL.Repositories.MealOrder
{
    public class ClassLevelRepository : Repository<ClassLevel>, IClassLevelRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public ClassLevelRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<ClassLevel>> GetClassLevelsAsync(BaseFilter filter)
        {
            IQueryable<ClassLevel> query = _appContext.ClassLevels
                .AsNoTracking()
                .AsSplitQuery()
                .Include(e => e.Institution)
                .Include(e => e.MealSession)
                .Include(e => e.MealSessionDetail)
                .Include(e => e.ClassLevelDetails).ThenInclude(e => e.MealSession)
                .Include(e => e.ClassLevelDetails).ThenInclude(e => e.MealPeriod);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);
            //int totalCount = query.Count();
            //query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false, applyPagination: (filter.Page > 0 && filter.PageSize > 0));
            //var result = new PagedEntity<ClassLevel>
            //{
            //    Filter = filter,
            //    TotalCount = totalCount,
            //    PagedData = await query.ToListAsync()
            //};

            return result;
        }

        #endregion
        public async Task<ClassLevel> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(ClassLevel classLevel)
        {
            var result = new BaseOperationResponse();

            if (await Exists(e => e.OutletId == classLevel.OutletId && e.Name == classLevel.Name && e.IsActive))
            {
                result.Message = "Class level already exists!";
                result.IsSuccess = false;
            }
            else
            {

                var f = await AddAsync(classLevel);
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
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(ClassLevel classLevel)
        {
            var result = new BaseOperationResponse();

            if (await Exists(e => e.OutletId == classLevel.OutletId && e.Name == classLevel.Name && e.IsActive && e.Id != classLevel.Id))
            {
                result.Message = "Class level already exists!";
                result.IsSuccess = false;
            }
            else
            {
                var f = await GetSingleOrDefaultAsync(e => e.Id == classLevel.Id);

                f.CopyFrom(classLevel);
                UpdateDetail(f, classLevel.ClassLevelDetails);
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
            }

            return result;
        }

        private void UpdateDetail(ClassLevel classLevel, ICollection<ClassLevelDetail> detail)
        {
            if (detail == null)
                return;

            // Remove deleted trays
            var existingIds = detail.Where(t => t.Id > 0).Select(t => t.Id).ToList();
            var detailsToRemove = classLevel.ClassLevelDetails.Where(t => !existingIds.Contains(t.Id)).ToList();
            foreach (var data in detailsToRemove)
                _appContext.ClassLevelDetails.Remove(data);

            // Update or add
            foreach (var classLevelDetail in detail)
            {
                var existingTray = classLevel.ClassLevelDetails.FirstOrDefault(t => t.Id == classLevelDetail.Id);
                if (existingTray != null)
                {
                    existingTray.ClassLevelId = classLevelDetail.ClassLevelId;
                    existingTray.SessionId = classLevelDetail.SessionId;
                    existingTray.PeriodId = classLevelDetail.PeriodId;
                }
                else
                {
                    classLevel.ClassLevelDetails.Add(new ClassLevelDetail
                    {
                        ClassLevelId = classLevelDetail.ClassLevelId,
                        SessionId = classLevelDetail.SessionId,
                        PeriodId = classLevelDetail.PeriodId
                    });
                }
            }
        }

        public async Task<BaseOperationResponse> DeleteAsync(int classLevelId)
        {
            var result = new BaseOperationResponse();
            var classLevel = await GetSingleOrDefaultAsync(r => r.Id == classLevelId);

            if (classLevel != null)
                return await Delete(classLevel);

            result.IsSuccess = false;
            result.Message = "Class Level not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(ClassLevel classLevel)
        {
            var result = new BaseOperationResponse();
            SoftDelete(classLevel);
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

        public Task<List<ClassLevel>> GetClassLevelsByOutletIdAsync(int outletId)
        {
            return _appContext.ClassLevels
                .AsNoTracking()
                .Include(e => e.MealSessionDetail)
                .Where(e => e.OutletId == outletId && e.IsActive)
                .ToListAsync();
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
