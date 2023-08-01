using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;

namespace DAL.Repositories
{
    public class MediaExtensionRepository : Repository<MediaExtension>, IMediaExtensionRepository
    {
        public MediaExtensionRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<MediaExtension> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<MediaExtension> All()
        {
            return GetAll()
                .OrderBy(c => c.Extension)
                .ToList();
        }

        public async Task<List<MediaExtension>> GetMediaExtensionsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<MediaExtension> query = _appContext.MediaExtension
                .Where(e => e.IsActive)
                .OrderBy(r => r.Extension);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<BaseOperationResponse> CreateAsync(MediaExtension mediaExtension)
        {
            var result = new BaseOperationResponse();

            if (Exists(e => e.Extension == mediaExtension.Extension).Result)
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = await AddAsync(mediaExtension);
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

        public async Task<BaseOperationResponse> UpdateAsync(MediaExtension mediaExtension)
        {
            var result = new BaseOperationResponse();

            if (Exists(e => e.Extension == mediaExtension.Extension && e.Id != mediaExtension.Id).Result)
            {
                result.Message = "Code already exists!";
                result.IsSuccess = false;
                return result;
            }

            var f = await GetSingleOrDefaultAsync(e => e.Id == mediaExtension.Id);

            f.CopyFrom(mediaExtension);
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


        public async Task<BaseOperationResponse> DeleteAsync(int mediaExtensionId)
        {
            var result = new BaseOperationResponse();
            var mediaExtension = await GetSingleOrDefaultAsync(r => r.Id == mediaExtensionId);

            if (mediaExtension != null)
                return await Delete(mediaExtension);

            result.IsSuccess = false;
            result.Message = "Id not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(MediaExtension mediaExtension)
        {
            var result = new BaseOperationResponse();
            SoftDelete(mediaExtension);
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

        public async Task<int> GetOrCreateByCode(MediaExtension data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Extension)) return 0;

            var f = await GetSingleOrDefaultAsync(c => c.Extension == data.Extension && c.IsActive);

            if (f == null)
            {
                await CreateAsync(data);
                f = await GetSingleOrDefaultAsync(c => c.Extension == data.Extension && c.IsActive);
            }

            return f.Id;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
