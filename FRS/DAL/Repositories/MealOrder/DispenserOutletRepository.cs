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
    public class DispenserOutletRepository : Repository<DispenserOutlet>, IDispenserOutletRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;
        public DispenserOutletRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }

        #region Sieved
        public async Task<PagedEntity<DispenserOutlet>> GetDispenserOutletsAsync(BaseFilter filter)
        {
            IQueryable<DispenserOutlet> query = _appContext.DispenserOutlets
                .Include(e => e.Institution)
                .Include(e => e.Trays);

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
        public async Task<DispenserOutlet> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public async Task<BaseOperationResponse> CreateAsync(DispenserOutlet dispenserOutlet)
        {
            var result = new BaseOperationResponse();

            if (await Exists(e => e.OutletId == dispenserOutlet.OutletId && e.DispenserCode == dispenserOutlet.DispenserCode && e.IsActive))
            {
                result.Message = "Dispenser already exists!";
                result.IsSuccess = false;
            }
            else
            {

                var f = await AddAsync(dispenserOutlet);
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

        public async Task<BaseOperationResponse> UpdateAsync(DispenserOutlet dispenserOutlet)
        {
            var result = new BaseOperationResponse();

            if (await Exists(e => e.OutletId == dispenserOutlet.OutletId && e.DispenserCode == dispenserOutlet.DispenserCode && e.IsActive && e.Id != dispenserOutlet.Id))
            {
                result.Message = "Dispenser already exists!";
                result.IsSuccess = false;
            }
            else
            {
                var f = await GetSingleOrDefaultAsync(e => e.Id == dispenserOutlet.Id);

                f.CopyFrom(dispenserOutlet);
                UpdateTrays(f, dispenserOutlet.Trays);
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

        private void UpdateTrays(DispenserOutlet dispenser, ICollection<TrayModel> trays)
        {
            if (trays == null)
                return;

            // Remove deleted trays
            var existingIds = trays.Where(t => t.Id > 0).Select(t => t.Id).ToList();
            var traysToRemove = dispenser.Trays.Where(t => !existingIds.Contains(t.Id)).ToList();
            foreach (var tray in traysToRemove)
                _appContext.Trays.Remove(tray);

            // Update or add
            foreach (var tray in trays)
            {
                var existingTray = dispenser.Trays.FirstOrDefault(t => t.Id == tray.Id);
                if (existingTray != null)
                {
                    existingTray.PLCId = tray.PLCId;
                    existingTray.MotorOutputNumber = tray.MotorOutputNumber;
                    existingTray.LEDOutputNumber = tray.LEDOutputNumber;
                }
                else
                {
                    dispenser.Trays.Add(new TrayModel
                    {
                        PLCId = tray.PLCId,
                        MotorOutputNumber = tray.MotorOutputNumber,
                        LEDOutputNumber = tray.LEDOutputNumber
                    });
                }
            }
        }


        public async Task<BaseOperationResponse> DeleteAsync(int dispenserOutletId)
        {
            var result = new BaseOperationResponse();
            var dispenserOutlet =  await _appContext.DispenserOutlets
                .Include(m => m.Trays).FirstOrDefaultAsync(m => m.Id == dispenserOutletId);

            if (dispenserOutlet != null)
            {
                if (dispenserOutlet.Trays.Any())
                {
                    _appContext.Trays.RemoveRange(dispenserOutlet.Trays);
                }
                return await Delete(dispenserOutlet);
            }

            result.IsSuccess = false;
            result.Message = "Class Level not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(DispenserOutlet dispenserOutlet)
        {
            var result = new BaseOperationResponse();
            SoftDelete(dispenserOutlet);
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
