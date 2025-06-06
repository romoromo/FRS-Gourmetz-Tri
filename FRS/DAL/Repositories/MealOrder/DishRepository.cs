using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces.MealOrder;
using DAL.Core.DTO;
using Sieve.Models;
using NPOI.SS.Formula.Functions;
using System.IO;

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

        public async Task<PagedEntity<DishLiteDTO>> GetDishesLiteAsync(BaseFilter filter)
        {
            IQueryable<Dish> query = _appContext.Dishes
                .Include(x => x.DishType)
                .Include(x => x.Cuisine)
                .Include(x => x.BentoBoxType)
                .AsNoTracking()
                .AsSplitQuery();

            query = _sieveProcessor.Apply(filter, query, applyPagination: false);
            var projectedQuery = query.Select(x => new DishLiteDTO
            {
                Id = x.Id,
                CatererId = x.CatererId,
                Code = x.Code,
                Label = x.Label,
                ProductionDescription = x.ProductionDescription,
                RRPrice = x.RRPrice,
                Cost = x.Cost,
                IsEnabled = x.IsEnabled,
                DishTypeName = x.DishType.Name,
                CuisineName = x.Cuisine.Name,
                BentoBoxTypeCode = x.BentoBoxType.Code,
            });
            int totalCount = await query.Select(x => x.Id).CountAsync();
            List<DishLiteDTO> pagedData = new List<DishLiteDTO>();
            if (filter.PageSize == null || filter.PageSize < 1)
            {
                pagedData = await projectedQuery.ToListAsync();
            }
            else
            {
                pagedData = await projectedQuery
                    .Skip((filter.Page.Value - 1) * filter.PageSize.Value)
                    .Take(filter.PageSize.Value)
                    .ToListAsync();
            }

            return new PagedEntity<DishLiteDTO>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = pagedData
            };
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
                            f.Icon = new Models.File();
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
                            f.ProductionPicture = new Models.File();
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

        public async Task<DishImportDTO> DishImport(int catererInfoId, List<DishImportInputDTO> dtos, int userId)
        {
            var resultData = new DishImportDTO
            {
                IsSuccess = true,
                Messages = new List<DishImportMessageDTO>()
            };

            if (catererInfoId <= 0 || dtos.Count == 0)
                return resultData;

            var dishTypeMap = (await _appContext.DishTypes.AsNoTracking()
                .Where(x => x.IsActive && x.CatererId == catererInfoId)
                .ToDictionaryAsync(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase));

            var bentoBoxTypeMap = (await _appContext.BentoBoxTypes.AsNoTracking()
                .Where(x => x.IsActive && x.CatererInfoId == catererInfoId)
                .ToDictionaryAsync(x => x.Code, x => x.Id, StringComparer.OrdinalIgnoreCase));

            var cuisineMap = (await _appContext.Cuisines.AsNoTracking()
                .Where(x => x.IsActive)
                .ToDictionaryAsync(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase));

            var storeInfo = (await _appContext.StoreInfos.AsNoTracking()
                .Where(x => x.IsActive && x.StoreType == "Kitchen")
                .ToDictionaryAsync(x => x.Code, x => x.Id, StringComparer.OrdinalIgnoreCase));

            var restrictions = (await _appContext.Restrictions.AsNoTracking()
                .Where(x => x.IsActive)
                .ToDictionaryAsync(x => x.Label, x => x.Id, StringComparer.OrdinalIgnoreCase));


            var (validatedDtos, validationResult) = ValidateDishImport(dtos, dishTypeMap, bentoBoxTypeMap, cuisineMap,storeInfo,restrictions);

            if (!validationResult.IsSuccess)
                return validationResult;

            var caterer = await _appContext.CatererInfos.FirstOrDefaultAsync(e => e.Id == catererInfoId);
            int dishCount = await _appContext.Dishes.AsNoTracking().CountAsync(e => e.CatererId == catererInfoId && e.IsActive);

            var dishItems = new List<Dish>();
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmm");
            int index = 0;

            foreach (var x in validatedDtos)
            {
                var currentCount = dishCount + (index + 1);
                var code = $"{caterer.Code}{timestamp}{currentCount}";
                var fileName = $"{code}.jpg";
                var pullPathName = Path.Combine("Resources", "Images", "Dish", fileName);
                var fileNameProduction = $"{code}_production.jpg";
                var pullPathNameProduction = Path.Combine("Resources", "Images", "Dish", fileNameProduction);

                var datasRestriction = x.RestrictionsIds.Select(x => new DishRestriction
                {
                    RestrictionId = x,
                    CreatedBy = userId,
                    UpdatedBy = userId
                });

                var dish = new Dish
                {
                    Code = code,
                    Label = x.OriginalData.Label,
                    DishTypeId = x.DishTypeId!.Value,
                    CuisineId = x.CuisineId,
                    RRPrice = x.OriginalData.RPP,
                    Cost = x.OriginalData.Cost,
                    IsEnabled = x.OriginalData.IsEnabled,
                    BentoBoxTypeId = x.BentoBoxTypeId!.Value,
                    Protein = x.OriginalData.Protein,
                    Sugar = x.OriginalData.Sugar,
                    StoreInfoId = x.StoreId,
                    TotalFat = x.OriginalData.TotalFat,
                    TotalCarb = x.OriginalData.TotalCarb,
                    Calories = x.OriginalData.Calories,
                    SapCode = x.OriginalData.SapCode,
                    ProductionDescription = x.OriginalData.ProductionDescription,
                    ExtraNotes = x.OriginalData.ExtraNote,
                    CatererId = catererInfoId,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    ProductionPicture = new Models.File
                    {
                        FileName = fileNameProduction,
                        Path = pullPathNameProduction,
                        Type = FileType.Icon.ToString(),
                        CreatedBy = userId,
                        UpdatedBy = userId
                    },
                    Icon = new Models.File
                    {
                        FileName = fileName,
                        Path = pullPathName,
                        Type = FileType.Icon.ToString(),
                        CreatedBy = userId,
                        UpdatedBy = userId
                    },
                    Restrictions = datasRestriction.ToList()
                };

                dishItems.Add(dish);
                index++;
            }

            AddRange(dishItems);
            await _appContext.SaveChangesAsync();

            return resultData;
        }


    private (List<ValidatedDishInputDTO> ValidatedDtos, DishImportDTO Result) ValidateDishImport(
        List<DishImportInputDTO> dtos,
        Dictionary<string, int> dishTypeMap,
        Dictionary<string, int> bentoBoxTypeMap,
        Dictionary<string, int> cuisineMap,
        Dictionary<string, int> storeInfoMap,
        Dictionary<string, int> restrictions
        )
        {
            var result = new DishImportDTO
            {
                IsSuccess = true,
                Messages = new List<DishImportMessageDTO>()
            };

            var validatedList = new List<ValidatedDishInputDTO>();
            int row = 1;

            foreach (var dto in dtos)
            {
                var messages = new List<string>();

                dishTypeMap.TryGetValue(dto.DishTypeName ?? "", out var dishTypeId);
                bentoBoxTypeMap.TryGetValue(dto.BentoBoxTypeName ?? "", out var bentoBoxTypeId);
                int? cuisineId = null;
                if (!string.IsNullOrEmpty(dto.CuisineName))
                {
                    cuisineMap.TryGetValue(dto.CuisineName ?? "", out int cuisineIdParam);
                    cuisineId = cuisineIdParam;
                }

                    storeInfoMap.TryGetValue(dto.KicthenName ?? "", out var storeInfoId);

                if (dishTypeId == 0)
                    messages.Add($"Dish Type: {dto.DishTypeName} not found");
                if (bentoBoxTypeId == 0)
                    messages.Add($"Bento Box Type: {dto.BentoBoxTypeName} not found");
                if (!string.IsNullOrEmpty(dto.CuisineName) && cuisineId == 0)
                    messages.Add($"Cuisine: {dto.CuisineName} not found");
                if (!string.IsNullOrEmpty(dto.KicthenName) && storeInfoId == 0)
                    messages.Add($"Kitchen: {dto.KicthenName} not found");

                List<int> validRestriction = new List<int>();
                foreach (var item in dto.Restrictions)
                {
                    restrictions.TryGetValue(item ?? "", out var restrictionId);
                    if (restrictionId == 0)
                        messages.Add($"Restriction: {item} not found");
                    else
                        validRestriction.Add(restrictionId);
                }

                if (messages.Any())
                {
                    result.IsSuccess = false;
                    result.Messages.Add(new DishImportMessageDTO
                    {
                        RowNumber = row,
                        Message = string.Join(", ", messages)
                    });
                }
                else
                {
                    validatedList.Add(new ValidatedDishInputDTO
                    {
                        OriginalData = dto,
                        DishTypeId = dishTypeId,
                        BentoBoxTypeId = bentoBoxTypeId,
                        CuisineId = cuisineId,
                        StoreId = storeInfoId,
                        RestrictionsIds = validRestriction
                    });
                }

                row++;
            }

            return (validatedList, result);
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
