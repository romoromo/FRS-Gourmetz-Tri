using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using System.IO;

namespace DAL.Repositories
{
    public class ImageRepository : Repository<ImageFile>, IImageRepository
    {
        public ImageRepository(ApplicationDbContext context) : base(context)
        { }


        public async Task<ImageFile> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<ImageFile> All()
        {
            return GetAll()
                .OrderBy(c => c.Title)
                .ToList();
        }

        public async Task<List<ImageFile>> GetImagesLoadRelatedAsync(int page, int pageSize, int? institutionId = null, string institutionCode = null, int? userId = null)
        {
            IQueryable<ImageFile> query = _appContext.ImageFiles
                .Include(e => e.Institution)
                .Where(e => e.IsActive);

            if (institutionId.HasValue)
            {
                query = query.Where(e => e.InstitutionId == institutionId);
            }

            else if (!string.IsNullOrEmpty(institutionCode))
            {
                query = query.Where(e => e.Institution.Name == institutionCode);
            }

            if (page > 0 || pageSize > 0)
            {
                query = query.OrderBy(e => e.Title);
            }

            if (page > 0)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize > 0)
                query = query.Take(pageSize);

            if(userId.HasValue)
            {
                query = query.Where(e => 
                e.Media != null 
                && e.Media.IsActive
                && e.Media.UserGroups != null 
                && e.Media.UserGroups.Any(ug =>
                ug.IsActive
                && ug.UserGroup != null
                && ug.UserGroup.IsActive
                && ug.UserGroup.Members.Any(u => u.UserId == userId)));
            }

            var roles = await query.ToListAsync();

            return roles
                .OrderByDescending(r => r.CreatedDate).ToList();
        }

        public async Task<List<ImageFile>> GetAllAsync(int? institutionId = null, string institutionCode = null)
        {
            IQueryable<ImageFile> query = _appContext.ImageFiles
                .Include(e => e.Institution)
                .Where(e => e.IsActive);

            if (institutionId.HasValue)
            {
                query = query.Where(e => e.InstitutionId == institutionId);
            }
            else if (!string.IsNullOrEmpty(institutionCode))
            {
                query = query.Where(e => e.Institution.Name == institutionCode);
            }

            var roles = await query.ToListAsync();

            return roles
                .OrderBy(r => r.Title).ToList();
        }

        public async Task<BaseOperationResponse> CreateAsync(ImageFile image, string folder = null)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(image);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save Image!";
                result.IsSuccess = false;
            }

            var media = await _appContext.Medias.FindAsync(f.MediaId);

            if (!string.IsNullOrWhiteSpace(folder) && media != null && !string.IsNullOrWhiteSpace(f.ImageLocation))
            {
                string newPath = Path.Combine(folder, media.Name.ToString());
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), newPath);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filename = Path.GetFileName(f.ImageLocation);
                string filepath = Path.Combine(Directory.GetCurrentDirectory(), f.ImageLocation);
                string destfilepath = Path.Combine(folderPath, filename);

                if (filepath != destfilepath && !System.IO.File.Exists(destfilepath))
                {
                    System.IO.File.Copy(filepath, destfilepath);

                    if (System.IO.File.Exists(destfilepath))
                    {
                        f.ImageLocation = Path.Combine(newPath, filename);

                        Update(f);
                        await _appContext.SaveChangesAsync();

                        System.IO.File.Delete(filepath);
                    }
                }
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(ImageFile image, string folder = null)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == image.Id);

            f.CopyFrom(image);
            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save image!";
                result.IsSuccess = false;
            }

            if (!string.IsNullOrWhiteSpace(folder) && f.Media != null && !string.IsNullOrWhiteSpace(f.ImageLocation))
            {
                string newPath = Path.Combine(folder, f.Media.Name.ToString());
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), newPath);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filename = Path.GetFileName(f.ImageLocation);
                string filepath = Path.Combine(Directory.GetCurrentDirectory(), f.ImageLocation);
                string destfilepath = Path.Combine(folderPath, filename);

                if (filepath != destfilepath && !System.IO.File.Exists(destfilepath))
                {
                    System.IO.File.Copy(filepath, destfilepath);

                    if (System.IO.File.Exists(destfilepath))
                    {
                        f.ImageLocation = Path.Combine(newPath, filename);

                        Update(f);
                        await _appContext.SaveChangesAsync();

                        System.IO.File.Delete(filepath);
                    }
                }
            }

            return result;
        }


        //public async Task<bool> TestCanDeleteAsync(int imageId)
        //{
        //    return !await _appContext.Users.AnyAsync(e => e.IsActive && e.DepartmentId == departmentId) &&
        //        !await _appContext.ContactGroupDepartments.AnyAsync(e => e.IsActive && e.DepartmentId == departmentId);
        //}


        public async Task<BaseOperationResponse> DeleteAsync(int imageId)
        {
            var result = new BaseOperationResponse();
            var image = await GetSingleOrDefaultAsync(r => r.Id == imageId);

            if (image != null)
                return await Delete(image);

            result.IsSuccess = false;
            result.Message = "Image not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(ImageFile image)
        {
            var result = new BaseOperationResponse();
            SoftDelete(image);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete image!";
                result.IsSuccess = false;
            }

            return result;
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
