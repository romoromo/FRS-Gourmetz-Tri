using DAL.Core;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces.MealOrder;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                .AsNoTracking()
                .AsSplitQuery();

            query = _sieveProcessor.Apply(filter, query, applyPagination: false);
            var projectedQuery = query.Select(x => new DishLiteDTO
            {
                Id = x.Id,
                CatererId = x.CatererId,
                CatererName = x.Caterer.Name,
                Code = x.Code,
                Label = x.Label,
                ProductionDescription = x.ProductionDescription,
                RRPrice = x.RRPrice,
                Cost = x.Cost,
                IsEnabled = x.IsEnabled,
                DishTypeName = x.DishType.Name,
                CuisineName = x.Cuisine.Name,
                BentoBoxTypeCode = x.BentoBoxType.Code,
                ExtraNote = x.ExtraNotes,
                KicthenName = x.StoreInfo.Code,
                SapCode = x.SapCode,
                Protein = x.Protein,
                Sugar = x.Sugar,
                TotalFat = x.TotalFat,
                TotalCarb = x.TotalCarb,
                Calories = x.Calories,
                Restrictions = x.Restrictions.Select(x => x.Restriction.Label).ToList()

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

        public async Task<List<int?>> GetDishesIdsByCycleIdAsync(int dishCycleId)
        {
            var cycleSetsData = await _appContext.DishCycleScheduleSets
                .AsNoTracking()
                .Where(x => x.DishCycleId == dishCycleId && x.IsActive)
                .Select(x => new { x.CycleTypeId, x.CycleTypeSequence, x.DishCycle.NumOfDays })
                .ToListAsync();

            var numOfDays = cycleSetsData.Select(x => x.NumOfDays).FirstOrDefault();

            var queryDish = await _appContext.DishCycles
                .AsSplitQuery()
                .AsNoTracking()
                .Where(x => cycleSetsData.Select(x => x.CycleTypeId).Contains(x.Id))
                .SelectMany(cycle =>
                    cycle.Schedules.Where(y => y.Day <= numOfDays)
                        .SelectMany(y => y.Details
                            .SelectMany(z => z.Menus
                                .Select(x => new { x.DishId, DishCycleId = cycle.Id, z.Sequence }))))
                .ToListAsync();

            var filteredIds = queryDish
                .Where(d => cycleSetsData
                    .Any(cs => cs.CycleTypeId == d.DishCycleId && cs.CycleTypeSequence == d.Sequence))
                .Select(d => d.DishId)
                .Distinct()
                .ToList();

            return filteredIds;
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

            if (caterer != null)
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

        public async Task<Dish> GetByCodeAsync(string code)
        {
            var records = await FindWithIncludeAsync(e => e.Code == code);

            return records.FirstOrDefault();
        }

        public async Task<BaseOperationResponse> CreateAsync(Dish dish)
        {
            var result = new BaseOperationResponse();

            var existingDishCode = _appContext.Dishes.AsNoTracking().FirstOrDefault(e => e.Code == dish.Code && e.IsActive);
            if (existingDishCode != null)
            {
                result.Message = "Dish code already exists!";
                result.IsSuccess = false;
                return result;
            }

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

            var existingDishCode = _appContext.Dishes.AsNoTracking().FirstOrDefault(e => e.Code == dish.Code && e.IsActive && e.Id != dish.Id);
            if (existingDishCode != null)
            {
                result.Message = "Dish code already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = await GetSingleOrDefaultAsync(e => e.Id == dish.Id);

            //delete old dish periods
            var selectedPeriodIds = dish.DishPeriods.Select(a => a.PeriodId);
            var periods = this._appContext.DishPeriods.Where(e => e.DishId == dish.Id);

            var toBeDeleted = periods.Where(e => !selectedPeriodIds.Contains(e.PeriodId));
            this._appContext.DishPeriods.RemoveRange(toBeDeleted);

            var toBeAdded = selectedPeriodIds.Except(periods.Select(e => e.PeriodId));

            toBeAdded.ToList().ForEach(e =>
            {
                this._appContext.DishPeriods.AddAsync(new DishPeriod { DishId = dish.Id, PeriodId = e });
            });

            //delete old dish restrictions
            var selectedRestrictionIds = dish.Restrictions.Select(a => a.RestrictionId);
            var restrictions = this._appContext.DishRestrictions.Where(e => e.DishId == dish.Id);

            var rtoBeDeleted = restrictions.Where(e => !selectedRestrictionIds.Contains(e.RestrictionId));
            this._appContext.DishRestrictions.RemoveRange(rtoBeDeleted);

            var rtoBeAdded = selectedRestrictionIds.Except(restrictions.Select(e => e.RestrictionId));

            rtoBeAdded.ToList().ForEach(e =>
            {
                this._appContext.DishRestrictions.AddAsync(new DishRestriction { DishId = dish.Id, RestrictionId = e });
            });

            //delete old subdishes
            var selectedSubDishIds = dish.SubDishes.Select(a => a.DishId);
            var currentSubdishes = this._appContext.DishDetails.Where(e => e.ParentDishId == dish.Id);

            var sdtoBeDeleted = currentSubdishes.Where(e => !selectedSubDishIds.Contains(e.DishId));
            this._appContext.DishDetails.RemoveRange(sdtoBeDeleted);

            var sdtoBeAdded = selectedSubDishIds.Except(currentSubdishes.Select(e => e.DishId));

            sdtoBeAdded.ToList().ForEach(e =>
            {
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
            f.SerialNumber = Guid.NewGuid().ToString();
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

            var dishes = await _appContext.Dishes
                .AsNoTracking()
                .Where(x => x.IsActive && x.CatererId == catererInfoId)
                .Select(x => new { x.Code, x.Id })
                .GroupBy(x => x.Code)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.First().Id,
                    StringComparer.OrdinalIgnoreCase
                );


            var (validatedDtos, validationResult) = ValidateDishImport(dtos, dishTypeMap, bentoBoxTypeMap, cuisineMap, storeInfo, restrictions, dishes);

            if (!validationResult.IsSuccess)
                return validationResult;

            var caterer = await _appContext.CatererInfos.FirstOrDefaultAsync(e => e.Id == catererInfoId);
            int dishCount = await _appContext.Dishes.AsNoTracking().CountAsync(e => e.CatererId == catererInfoId && e.IsActive);

            var dishItems = new List<Dish>();
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmm");
            int index = 0;

            var validatedDtosForInsert = validatedDtos.Where(x => (x.DishId == null || x.DishId <= 0));
            foreach (var x in validatedDtosForInsert)
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

            var updateDishIds = validatedDtos.Where(x => x.DishId > 0).Select(x => x.DishId).ToList();

            var dishesToUpdate = await _appContext.Dishes
                .Include(x => x.Restrictions)
                .Where(x => updateDishIds.Contains(x.Id))
                .ToListAsync();

            foreach (var dish in dishesToUpdate)
            {
                var updateDto = validatedDtos.First(x => x.DishId == dish.Id);

                dish.Label = updateDto.OriginalData.Label;
                dish.DishTypeId = updateDto.DishTypeId!.Value;
                dish.CuisineId = updateDto.CuisineId;
                dish.RRPrice = updateDto.OriginalData.RPP;
                dish.Cost = updateDto.OriginalData.Cost;
                dish.IsEnabled = updateDto.OriginalData.IsEnabled;
                dish.BentoBoxTypeId = updateDto.BentoBoxTypeId!.Value;
                dish.Protein = updateDto.OriginalData.Protein;
                dish.Sugar = updateDto.OriginalData.Sugar;
                dish.StoreInfoId = updateDto.StoreId;
                dish.TotalFat = updateDto.OriginalData.TotalFat;
                dish.TotalCarb = updateDto.OriginalData.TotalCarb;
                dish.Calories = updateDto.OriginalData.Calories;
                dish.SapCode = updateDto.OriginalData.SapCode;
                dish.ProductionDescription = updateDto.OriginalData.ProductionDescription;
                dish.ExtraNotes = updateDto.OriginalData.ExtraNote;
                dish.UpdatedBy = userId;

                dish.Restrictions.Clear();
                foreach (var r in updateDto.RestrictionsIds)
                {
                    dish.Restrictions.Add(new DishRestriction
                    {
                        RestrictionId = r,
                        CreatedBy = userId,
                        UpdatedBy = userId
                    });
                }
            }


            await _appContext.SaveChangesAsync();

            return resultData;
        }


        private (List<ValidatedDishInputDTO> ValidatedDtos, DishImportDTO Result) ValidateDishImport(
            List<DishImportInputDTO> dtos,
            Dictionary<string, int> dishTypeMap,
            Dictionary<string, int> bentoBoxTypeMap,
            Dictionary<string, int> cuisineMap,
            Dictionary<string, int> storeInfoMap,
            Dictionary<string, int> restrictions,
            Dictionary<string, int> dishes
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

                int? dishId = null;
                if (!string.IsNullOrEmpty(dto.Code))
                {
                    dishes.TryGetValue(dto.Code ?? "", out int dishIdParam);
                    dishId = dishIdParam;
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
                if (!string.IsNullOrEmpty(dto.Code) && dishId == 0)
                    messages.Add($"Dish: {dto.Code} not found");

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
                        DishId = dishId,
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

        public async Task<List<DishWithMenuDTO>> GetCurrentMenu()
        {
            var result = from dc in _appContext.DishCycles.AsNoTracking()
                         where dc.IsActive 

                         join dcs in _appContext.DishCycleSchedules on dc.Id equals dcs.DishCycleId into joinSchedules
                         from dcs in joinSchedules.DefaultIfEmpty()

                         join dcsd in _appContext.DishCycleScheduleDetails on (dcs == null ? -1 : dcs.Id) equals dcsd.DishCycleScheduleId into joinDetails
                         from dcsd in joinDetails.DefaultIfEmpty()

                         join dcsdm in _appContext.DishCycleScheduleDetailMenus on (dcsd == null ? -1 : dcsd.Id) equals dcsdm.DishCycleScheduleDetailId into joinMenus
                         from dcsdm in joinMenus.DefaultIfEmpty()

                         join d in _appContext.Dishes.Where(x => x.IsActive) on (dcsdm == null ? -1 : dcsdm.DishId) equals d.Id into joinDish
                         from d in joinDish.DefaultIfEmpty()

                         group d by new { dc.Id, dc.Label, dc.UpdatedDate } into grouped
                         select new DishWithMenuDTO
                         {
                             MenuID = grouped.Key.Id,
                             Label = grouped.Key.Label,
                             LastUpdated = grouped.Key.UpdatedDate,
                             Dishes = grouped
                                 .Where(x => x != null) 
                                 .Select(x => new DishWithMenuDetailDTO
                                 {
                                     ID = x.Id,
                                     Code = x.Code,
                                     Label = x.Label,
                                     SerialNumber = x.SerialNumber,
                                     LastUpdated = x.UpdatedDate
                                 })
                                 .Distinct() 
                                 .ToList()
                         };

            return await result.ToListAsync();
        }

        public async Task<List<Dish>> GetDishForDownload(List<string> serialNumbers)
        {
            return await _appContext.Dishes.AsNoTracking()
                .Where(m => serialNumbers.Contains(m.SerialNumber)).ToListAsync();
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
