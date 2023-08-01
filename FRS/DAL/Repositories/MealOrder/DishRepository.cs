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
    public class DishRepository : Repository<Dish>, IDishRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public DishRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<Dish>> GetDishesAsync(BaseFilter filter)
        {
            IQueryable<Dish> query = _appContext.Dishes
                .Include(e => e.DishType);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion

        public async Task<List<Dish>> GetDishChangesAsync(DateTime updatedAfter, DateTime? updatedBefore)
        {
            IQueryable<Dish> query = _appContext.Dishes.Where(d => d.IsActive && d.UpdatedDate >= updatedAfter && (updatedBefore == null || d.UpdatedDate <= updatedBefore))
                .Include(e => e.DishType);

            var result = await query.ToListAsync();

            return result;
        }

        public async Task<string> GenerateCode(int id)
        {
            string code = string.Empty;
            var caterer = await _appContext.CatererInfos.FirstOrDefaultAsync(e => e.Id == id);

            if(caterer != null)
            {
                int dishCount = await _appContext.Dishes.CountAsync(e => e.CatererId == id && e.IsActive) + 1;
                code = string.Format("{0}{1}{2}", caterer.Code, DateTime.UtcNow.ToString("yyyyMMddHHmm"), dishCount);
            }

            return code;
        }

        public async Task<Dish> GetByIdAsync(int id)
        {
            var records = await FindWithIncludeAsync(e => e.Id == id, e => e.DishComponents, e => e.Restrictions);

            return records.FirstOrDefault();
        }

        public async Task<BaseOperationResponse> CreateAsync(Dish dish)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(dish);
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

        public async Task<BaseOperationResponse> UpdateAsync(Dish dish)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == dish.Id);

            //delete old dish periods
            var selectedPeriodIds = dish.DishPeriods.Select(a => a.PeriodId);
            var periods = this._appContext.DishPeriods.Where(e => e.DishId == dish.Id);

            var toBeDeleted = periods.Where(e => !selectedPeriodIds.Contains(e.PeriodId));
            this._appContext.DishPeriods.RemoveRange(toBeDeleted);

            var toBeAdded = selectedPeriodIds.Except(periods.Select(e => e.PeriodId));

            toBeAdded.ToList().ForEach(e => {
                this._appContext.DishPeriods.AddAsync(new DishPeriod { DishId = dish.Id, PeriodId = e });
            });

            //delete old dish restrictions
            var selectedRestrictionIds = dish.Restrictions.Select(a => a.RestrictionId);
            var restrictions = this._appContext.DishRestrictions.Where(e => e.DishId == dish.Id);

            var rtoBeDeleted = restrictions.Where(e => !selectedRestrictionIds.Contains(e.RestrictionId));
            this._appContext.DishRestrictions.RemoveRange(rtoBeDeleted);

            var rtoBeAdded = selectedRestrictionIds.Except(restrictions.Select(e => e.RestrictionId));

            rtoBeAdded.ToList().ForEach(e => {
                this._appContext.DishRestrictions.AddAsync(new DishRestriction { DishId = dish.Id, RestrictionId = e });
            });

            //delete old subdishes
            var selectedSubDishIds = dish.SubDishes.Select(a => a.DishId);
            var currentSubdishes = this._appContext.DishDetails.Where(e => e.ParentDishId == dish.Id);

            var sdtoBeDeleted = currentSubdishes.Where(e => !selectedSubDishIds.Contains(e.DishId));
            this._appContext.DishDetails.RemoveRange(sdtoBeDeleted);

            var sdtoBeAdded = selectedSubDishIds.Except(currentSubdishes.Select(e => e.DishId));

            sdtoBeAdded.ToList().ForEach(e => {
                this._appContext.DishDetails.AddAsync(new DishDetail { ParentDishId = dish.Id, DishId = e });
            });

            //delete old components
            
            var selectedComponentIds = dish.DishComponents.Select(a => a.Id);
            var currentComponents = this._appContext.DishComponents.Where(e => e.DishId == dish.Id);

            var dctoBeDeleted = currentComponents.Where(e => !selectedComponentIds.Contains(e.Id));
            this._appContext.DishComponents.RemoveRange(dctoBeDeleted);

            foreach (var component in dish.DishComponents)
            {
                var existComponent = this._appContext.DishComponents.FirstOrDefault(e => e.Id == component.Id);
                if (existComponent == null || existComponent.Id == 0)
                {
                    await this._appContext.DishComponents.AddAsync(component);
                }
                else
                {
                    existComponent.CopyFrom(component);
                    this._appContext.DishComponents.Update(existComponent);
                }

            }

            //var dctoBeAdded = selectedComponentIds.Except(currentComponents.Select(e => e.Id));

            //dctoBeAdded.ToList().ForEach(e => {
            //    var dc = dish.DishComponents.First(x => x.Id == e);
            //    dc.DishId = dish.Id;
            //    this._appContext.DishComponents.AddAsync(dc);
            //});

            f.CopyFrom(dish);

            if (dish.Icon != null && !string.IsNullOrEmpty(dish.Icon.Path))
            {
                if (!f.FileId.HasValue)
                {
                    f.Icon = dish.Icon;
                }
                else
                {
                    if (f.Icon == null)
                    {
                        //TODO: check why EF Core is not loading the Icon property; interim solution
                        var icon = await _appContext.Files.SingleOrDefaultAsync(e => e.Id == f.FileId);
                        if (icon == null)
                        {
                            f.Icon = new File();
                        }
                        else
                        {
                            f.Icon = icon;
                            f.FileId = icon.Id;
                        }
                    }

                    f.Icon.Path = dish.Icon.Path;
                    f.Icon.FileName = dish.Icon.FileName ?? System.IO.Path.GetFileName(dish.Icon.Path);
                    f.Icon.Type = FileType.Icon.ToString();
                }
            }

            if (dish.ProductionPicture != null && !string.IsNullOrEmpty(dish.ProductionPicture.Path))
            {
                if (!f.ProductionPictureId.HasValue)
                {
                    f.ProductionPicture = dish.ProductionPicture;
                }
                else
                {
                    if (f.ProductionPicture == null)
                    {
                        //TODO: check why EF Core is not loading the ProductionPicture property; interim solution
                        var icon = await _appContext.Files.SingleOrDefaultAsync(e => e.Id == f.ProductionPictureId);
                        if (icon == null)
                        {
                            f.ProductionPicture = new File();
                        }
                        else
                        {
                            f.ProductionPicture = icon;
                            f.ProductionPictureId = icon.Id;
                        }
                    }

                    f.ProductionPicture.Path = dish.ProductionPicture.Path;
                    f.ProductionPicture.FileName = dish.ProductionPicture.FileName ?? System.IO.Path.GetFileName(dish.ProductionPicture.Path);
                    f.ProductionPicture.Type = FileType.Icon.ToString();
                }
            }

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


        public async Task<BaseOperationResponse> DeleteAsync(int dishId)
        {
            var result = new BaseOperationResponse();
            var dish = await GetSingleOrDefaultAsync(r => r.Id == dishId);

            if (dish != null)
                return await Delete(dish);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(Dish dish)
        {
            var result = new BaseOperationResponse();
            SoftDelete(dish);
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
