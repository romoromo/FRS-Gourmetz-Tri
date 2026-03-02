using DAL.Core;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces;
using DAL.Repositories.Interfaces.MealOrder;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.MealOrder
{
    public class OrderPortalContentRepository : Repository<OrderPortalContent>, IOrderPortalContentRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public OrderPortalContentRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<OrderPortalContent>> GetOrderPortalContentsAsync(BaseFilter filter)
        {
            var today = DateTime.Today;
            IQueryable<OrderPortalContent> query = _appContext.OrderPortalContents.Where(x => x.EffectiveStartDate <= today && x.EffectiveEndDate >= today);

            var result = await this._sieveProcessor.GetPagedAsync(query, filter);

            return result;
        }

        #endregion

        public async Task<OrderPortalContent> GetOrderPortalContentFirst(int outletId)
        {
            var records = await FindWithIncludeAsync(e => e.OutletId == outletId);

            return records.FirstOrDefault();
        }

        public async Task<OrderPortalContent> GetByIdAsync(int id)
        {
            var records = await FindWithIncludeAsync(e => e.Id == id);

            return records.FirstOrDefault();
        }

        public async Task<BaseOperationResponse> CreateAsync(OrderPortalContent content)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(content);
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

        public async Task<BaseOperationResponse> UpdateAsync(OrderPortalContent content)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == content.Id);

            //delete old content banners
            var selectedBannerIds = content.Banners.Select(a => a.Id);
            var existingBanners = this._appContext.OrderPortalBanners.Where(e => e.OrderPortalContentId == content.Id);

            var toBeDeleted = existingBanners.Where(e => !selectedBannerIds.Contains(e.Id));
            this._appContext.OrderPortalBanners.RemoveRange(toBeDeleted);

            foreach (var component in content.Banners)
            {
                var existComponent = this._appContext.OrderPortalBanners.FirstOrDefault(e => e.Id == component.Id);
                if (existComponent == null || existComponent.Id == 0)
                {
                    //if (component.BannerImage != null && !string.IsNullOrEmpty(component.BannerImage.Path))
                    //{
                    //    if (component.BannerImage == null)
                    //    {
                    //        //TODO: check why EF Core is not loading the Icon property; interim solution
                    //        var icon = await _appContext.Files.SingleOrDefaultAsync(e => e.Id == existComponent.ImageId);
                    //        if (icon == null)
                    //        {
                    //            component.BannerImage = new File();
                    //        }
                    //        else
                    //        {
                    //            component.BannerImage = icon;
                    //            component.ImageId = icon.Id;
                    //        }
                    //    }

                    //    component.BannerImage.Path = component.BannerImage.Path;
                    //    component.BannerImage.FileName = component.BannerImage.FileName ?? System.IO.Path.GetFileName(component.BannerImage.Path);
                    //    component.BannerImage.Type = FileType.Icon.ToString();
                    //}
                    await this._appContext.OrderPortalBanners.AddAsync(component);
                }
                else
                {
                    existComponent.CopyFrom(component);
                    //if (component.BannerImage != null && !string.IsNullOrEmpty(component.BannerImage.Path))
                    //{
                    //    if (!existComponent.ImageId.HasValue)
                    //    {
                    //        existComponent.BannerImage = component.BannerImage;
                    //    }
                    //    else
                    //    {
                    //        if (existComponent.BannerImage == null)
                    //        {
                    //            //TODO: check why EF Core is not loading the Icon property; interim solution
                    //            var icon = await _appContext.Files.SingleOrDefaultAsync(e => e.Id == existComponent.ImageId);
                    //            if (icon == null)
                    //            {
                    //                existComponent.BannerImage = new File();
                    //            }
                    //            else
                    //            {
                    //                existComponent.BannerImage = icon;
                    //                existComponent.ImageId = icon.Id;
                    //            }
                    //        }

                    //        existComponent.BannerImage.Path = component.BannerImage.Path;
                    //        existComponent.BannerImage.FileName = component.BannerImage.FileName ?? System.IO.Path.GetFileName(component.BannerImage.Path);
                    //        existComponent.BannerImage.Type = FileType.Icon.ToString();
                    //    }
                    //}
                    this._appContext.OrderPortalBanners.Update(existComponent);
                }

            }

            f.CopyFrom(content);

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


        public async Task<BaseOperationResponse> DeleteAsync(int contentId)
        {
            var result = new BaseOperationResponse();
            var content = await GetSingleOrDefaultAsync(r => r.Id == contentId);

            if (content != null)
                return await Delete(content);

            result.IsSuccess = false;
            result.Message = "Record not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(OrderPortalContent content)
        {
            var result = new BaseOperationResponse();
            SoftDelete(content);
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
