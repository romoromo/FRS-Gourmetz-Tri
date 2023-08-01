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
using NPOI.HSSF.UserModel;

namespace DAL.Repositories
{
    public class DirectoryListingRepository : Repository<DirectoryListing>, IDirectoryListingRepository
    {
        public DirectoryListingRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<DirectoryListing> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<DirectoryListing> All()
        {
            return GetAll()
                .OrderBy(c => c.Code)
                .ToList();
        }

        public async Task<List<DirectoryListing>> GetDirectoryListingsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<DirectoryListing> query = _appContext.DirectoryListing
                .Where(e => e.IsActive)
                .OrderBy(r => r.Code);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<List<DirectoryListing>> GetDirectoryListingsExcludeInternal(int? institutionId = null)
        {
            IQueryable<DirectoryListing> query = _appContext.DirectoryListing
                .Where(e => e.IsActive && !e.IsInternal)
                .OrderBy(r => r.Code);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<BaseOperationResponse> CreateAsync(DirectoryListing directoryListing)
        {
            var result = new BaseOperationResponse();
            var f = await AddAsync(directoryListing);
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

        public async Task<BaseOperationResponse> UpdateAsync(DirectoryListing directoryListing)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.Id == directoryListing.Id);

            f.CopyFrom(directoryListing);
            f.DirectoryListingCategoryId = directoryListing.DirectoryListingCategoryId;
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

        public async Task<BaseOperationResponse> ImportFile(List<List<string>> data)
        {
            var result = new BaseOperationResponse();

            foreach (var row in data.Skip(1).ToList()) // Skip first row
            {
                string dir_code = row[0].ToString().Trim();
                string category_id = row[1].ToString().Trim();
                string category_label = row[2].ToString().Trim();
                string building_label = row[3].ToString().Trim();
                string disciplines = row[4].ToString().Trim();
                string aliases = row[5].ToString().Trim();
                string level = row[6].ToString().Trim();
                string building_code = row[7].ToString().Trim();
                string description = row[8].ToString().Trim();
                string opening_hours = row[9].ToString().Trim();
                string contact = row[10].ToString().Trim();
                string towid = row[11].ToString().Trim();
                string location_id = row[13].ToString().Trim();
                string floor_code = row[15].ToString().Trim();
                string floor_label = row[16].ToString().Trim();

                int? categoryId = await new DirectoryListingCategoryRepository(_appContext).GetOrCreateByCode(new DirectoryListingCategory()
                {
                    Code = category_label,
                    Label = category_label
                });

                int? floorId = await new FloorRepository(_appContext).GetOrCreateByCode(new Floor()
                {
                    Code = floor_code,
                    Label = floor_code
                });

                var building = _appContext.DirectoryListing.Where(e =>
                                e.Code == dir_code &&
                                e.IsActive).FirstOrDefault();
                if (building == null)
                    building = new DirectoryListing();

                building.Label = building_label;
                building.UnitNumber = building_code;
                //building.level = level;


                building.Description = description;
                building.Aliases = aliases;
                building.Disciplines = disciplines;
                building.OpeningHours = opening_hours;
                building.Contact = contact;
                building.Code = dir_code;
                building.MapDisplayName = building_label;

                if (categoryId != null && categoryId > 0)
                {
                    building.DirectoryListingCategoryId = categoryId;
                }
                if (location_id != "" && location_id != "0")
                {
                    //building.location_id = Convert.ToInt64(location_id);
                }
                if (floorId != null && floorId > 0)
                {
                    building.FloorId = floorId;
                }

                building.IsActive = true;

                if (building.Id > 0) await UpdateAsync(building);
                else await CreateAsync(building);

                //var towerList = GetTower(building.building_id);

                //towerList.ToList()
                //    .ForEach(t =>
                //    {
                //        Debug.WriteLine("row is " + t, t);
                //        var towerEntity = DB.sgn_building_tower.Find(t.building_tower_id);

                //        towerEntity.active_data = false;
                //        towerEntity.deleted_data = true;
                //    });

                //if (towid.Length > 0)
                //{
                //    Debug.WriteLine("tower ids : " + towid);
                //    string[] towsplit = towid.Split(new Char[] { ',' });
                //    foreach (string t in towsplit)
                //    {
                //        Debug.WriteLine("tower id aja : " + t);
                //        long long_tower = Convert.ToInt64(t);
                //        var entity = DB.sgn_building_tower.FirstOrDefault(s => s.building_id == building.building_id && s.tower_id == long_tower) ?? new sgn_building_tower();


                //        entity.tower_id = long_tower;
                //        entity.building_id = building.building_id;
                //        entity.active_data = true;
                //        entity.deleted_data = false;

                //        if (entity.building_tower_id > 0)
                //        {
                //            DB.Entry(entity).State = EntityState.Modified;
                //        }
                //        else
                //        {
                //            DB.sgn_building_tower.Add(entity);
                //        }
                //    };
                //}

                //DB.SaveChanges();
            }

            result.Message = "Successfully saved!";
            result.IsSuccess = true;
            return result;
        }

        public async Task<byte[]> GetTemplate()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var wb = new HSSFWorkbook();
                var sheet = (HSSFSheet)wb.CreateSheet("Sheet1");
                var headers = new List<string>(new string[] { "Directory Code*","Category ID","Category Label", "Display Name*","Disciplines","Aliases", "Level", "Unit Number",
                    "Info", "Opening Hours", "Contact", "Building ID", "Building Code", "Cluster ID*", "Cluster Label", "Floor Code", "Floor Label"});

                var directory = await GetDirectoryListingsLoadRelatedAsync(-1, -1);

                var row = sheet.CreateRow(0);
                for (int i = 0; i < headers.Count; i++)
                {
                    row.CreateCell(i).SetCellValue(headers[i]);
                }

                var style = wb.CreateCellStyle();
                var font = wb.CreateFont();
                //font.Color = NPOI.HSSF.Util.HSSFColor.Blue.;
                style.SetFont(font);

                #region Directory Table

                var rowCount = 1;
                directory
                    .ForEach(t =>
                    {
                        var tinfo = t;
                        var towid = "";
                        var towCode = "";
                        
                        row = sheet.GetRow(rowCount);
                        if (row == null)
                            row = sheet.CreateRow(rowCount);
                        row.CreateCell(0).SetCellValue(tinfo.Code);
                        row.CreateCell(1).SetCellValue(tinfo.DirectoryListingCategory != null ? tinfo.DirectoryListingCategory.Code : "");
                        row.CreateCell(2).SetCellValue(tinfo.DirectoryListingCategory != null ? tinfo.DirectoryListingCategory.Code : "");
                        row.CreateCell(3).SetCellValue(tinfo.Label);
                        row.CreateCell(4).SetCellValue(tinfo.Disciplines);
                        row.CreateCell(5).SetCellValue(tinfo.Aliases);
                        row.CreateCell(6).SetCellValue("");
                        row.CreateCell(7).SetCellValue(tinfo.UnitNumber);
                        row.CreateCell(8).SetCellValue(tinfo.Description);
                        row.CreateCell(9).SetCellValue(tinfo.OpeningHours);
                        row.CreateCell(10).SetCellValue(tinfo.Contact);
                        //row.CreateCell(11).SetCellValue(towid);
                        //row.CreateCell(12).SetCellValue(towCode);
                        //row.CreateCell(13).SetCellValue(tinfo.location_id != null ? (long)tinfo.location_id : 0);
                        //row.CreateCell(14).SetCellValue(tinfo.location_label);
                        row.CreateCell(15).SetCellValue(tinfo.Floor != null ? tinfo.Floor.Code : "");
                        row.CreateCell(16).SetCellValue(tinfo.Floor != null ? tinfo.Floor.Code : "");

                        rowCount++;
                    });

                #endregion

                for (int i = 0; i < headers.Count; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                wb.Write(stream);

                return stream.ToArray();
            }
        }


        public async Task<BaseOperationResponse> DeleteAsync(int directoryListingId)
        {
            var result = new BaseOperationResponse();
            var directoryListing = await GetSingleOrDefaultAsync(r => r.Id == directoryListingId);

            if (directoryListing != null)
                return await Delete(directoryListing);

            result.IsSuccess = false;
            result.Message = "Id not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(DirectoryListing directoryListing)
        {
            var result = new BaseOperationResponse();
            SoftDelete(directoryListing);
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
