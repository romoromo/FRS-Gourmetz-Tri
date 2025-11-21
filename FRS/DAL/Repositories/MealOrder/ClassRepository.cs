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
    public class ClassRepository : Repository<Class>, IClassRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public ClassRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<Class>> GetClassesAsync(BaseFilter filter)
        {
            IQueryable<Class> query = _appContext.Classes;

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion
        public async Task<Class> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(Class classModel)
        {
            var result = new BaseOperationResponse();

            if (await Exists(e => e.Name == classModel.Name && e.IsActive && e.ClassLevelId == classModel.ClassLevelId))
            {
                result.Message = "Class already exists!";
                result.IsSuccess = false;
            }
            else
            {
                var f = await AddAsync(classModel);
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

        public async Task<BaseOperationResponse> UpdateAsync(Class classModel)
        {
            var result = new BaseOperationResponse();

            if (await Exists(e => e.Name == classModel.Name && e.IsActive && e.Id != classModel.Id && e.ClassLevelId == classModel.ClassLevelId))
            {
                result.Message = "Class already exists!";
                result.IsSuccess = false;
            }
            else
            {
                var f = await GetSingleOrDefaultAsync(e => e.Id == classModel.Id);

                f.CopyFrom(classModel);
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

        public async Task<BaseOperationResponse> DeleteAsync(int classModelId)
        {
            var result = new BaseOperationResponse();
            var classModel = await GetSingleOrDefaultAsync(r => r.Id == classModelId);

            if (classModel != null)
                return await Delete(classModel);

            result.IsSuccess = false;
            result.Message = "Class  not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Class classModel)
        {
            var result = new BaseOperationResponse();
            SoftDelete(classModel);
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

        public async Task<List<Class>> GetByOutlet(int outletId)
        {
            return await _appContext.Classes
                .Where(m => m.IsActive && m.ClassLevel.IsActive && m.ClassLevel.OutletId == outletId).ToListAsync();
        }

        public async Task<List<Class>> GetClassByStudentGroupId(int studentGroupId)
        {
            var classList = await (from sg in _appContext.StudentGroups
                                   join sgd in _appContext.StudentGroupDetails on sg.Id equals sgd.StudentGroupId
                                   join st in _appContext.Students on sgd.StudentId equals st.Id
                                   join c in _appContext.Classes on st.ClassId equals c.Id
                                   where sg.Id == studentGroupId && sg.IsActive && sgd.IsActive && st.IsActive && c.IsActive
                                   select c).Distinct().ToListAsync();
            return classList;
        }

        public async Task<(Class Class, int outletId)> GetClassByStudentId(int studentId)
        {
            var student = await _appContext.Students.Where(m => m.IsActive && m.Class.IsActive && m.Id == studentId)
                .Include(m => m.Class).ThenInclude(m => m.ClassLevel)
                .FirstOrDefaultAsync();
            return (student?.Class ?? new Class(), student?.OutletId ?? 0);
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
