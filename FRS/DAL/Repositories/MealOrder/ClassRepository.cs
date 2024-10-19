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

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
