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
    public class SignagePublicationRepository : Repository<SignagePublication>, ISignagePublicationRepository
    {
        public SignagePublicationRepository(ApplicationDbContext context) : base(context)
        { }


        public async Task<SignagePublication> GetByIdAsync(int id)
        {
            return await GetAsync(id);
        }

        public IEnumerable<SignagePublication> All()
        {
            return GetAll()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<List<SignagePublication>> GetSignagePublicationsLoadRelatedAsync(int page, int pageSize, int? institutionId = null)
        {
            IQueryable<SignagePublication> query = _appContext.SignagePublications
                .Where(e => e.IsActive)
                .OrderBy(r => r.Name);

            if (page != -1)
                query = query.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                query = query.Take(pageSize);

            var roles = await query.ToListAsync();

            return roles;
        }

        public async Task<BaseOperationResponse> CreateAsync(SignagePublication signagePublication)
        {
            var result = new BaseOperationResponse();
            if (Exists(e => e.Name == signagePublication.Name && e.IsActive).Result)
            {
                result.Message = "Signage Publication Name already exists!";
                result.IsSuccess = false;
            }
            else
            {
                var f = await AddAsync(signagePublication);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save signage publication!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(SignagePublication signagePublication)
        {
            var result = new BaseOperationResponse();

            var f = GetSingleOrDefault(e => e.Id == signagePublication.Id);

            if (Exists(e => e.Id != f.Id && e.Name == signagePublication.Name && e.IsActive).Result)
            {
                result.Message = "Signage Publication Name already exists!";
                result.IsSuccess = false;
            }
            else
            {
                var approveChanged = f.Approved != signagePublication.Approved || f.Rejected != signagePublication.Rejected || f.Remark != signagePublication.Remark;


                f.CopyFrom(signagePublication);

                if(approveChanged)
                {
                    if (signagePublication.Approved)
                    {
                        f.ApprovedBy = _appContext.CurrentUserId;
                        f.ApprovedDate = DateTime.Now;
                    }

                    if (signagePublication.Rejected)
                    {
                        f.RejectedBy = _appContext.CurrentUserId;
                        f.RejectedDate = DateTime.Now;
                    }
                }

                Update(f);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    signagePublication.Schedules.ToList().ForEach(cSource =>
                    {
                        if (cSource.IsActive || cSource.Id > 0)
                        {
                            var ct = _appContext.SignageSchedules.FirstOrDefault(c => cSource.Id == c.Id) ?? new SignageSchedule();
                            ct.CopyFrom(cSource);
                            if (f.Id != ct.PublicationId) ct.PublicationId = f.Id;
                            ct.IsActive = cSource.IsActive;
                            _appContext.SignageSchedules.Update(ct);
                            _appContext.SaveChanges();

                            cSource.Compilations.ToList().ForEach(dSource =>
                            {
                                if (dSource.IsActive || dSource.Id > 0)
                                {
                                    var dt = _appContext.SignageScheduleCompilations.FirstOrDefault(c => dSource.Id == c.Id) ?? new SignageScheduleCompilation();
                                    dt.CopyFrom(dSource);
                                    if (ct.Id != dt.ScheduleId) dt.ScheduleId = ct.Id;
                                    dt.CompilationId = dSource.CompilationId;
                                    dt.IsActive = dSource.IsActive;
                                    _appContext.SignageScheduleCompilations.Update(dt);
                                    _appContext.SaveChanges();
                                }
                            });
                        }
                    });

                    if(approveChanged)
                    {
                        if (signagePublication.Approved)
                        {
                            await _appContext.SignagePublicationHistorys.AddAsync(new SignagePublicationHistory()
                            {
                                Approved = f.Approved,
                                Rejected = f.Rejected,
                                Remark = f.Remark,
                                ApprovedBy = f.ApprovedBy,
                                ApprovedDate = f.ApprovedDate,
                                PublicationId = f.Id
                            });
                        }

                        if(signagePublication.Rejected)
                        {
                            await _appContext.SignagePublicationHistorys.AddAsync(new SignagePublicationHistory()
                            {
                                Approved = f.Approved,
                                Rejected = f.Rejected,
                                Remark = f.Remark,
                                RejectedBy = f.RejectedBy,
                                RejectedDate = f.RejectedDate,
                                PublicationId = f.Id
                            });
                        }

                        await _appContext.SaveChangesAsync();
                    }

                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                    result.Data = f;
                }
                else
                {
                    result.Message = "Failed to save signage publication!";
                    result.IsSuccess = false;
                }
            }

            return result;
        }


        public async Task<BaseOperationResponse> DeleteAsync(int signagePublicationId)
        {
            var result = new BaseOperationResponse();
            var signagePublication = await GetSingleOrDefaultAsync(r => r.Id == signagePublicationId);

            if (signagePublication != null)
                return await Delete(signagePublication);

            result.IsSuccess = false;
            result.Message = "Signage Publication not found.";
            return result;
        }

        public async Task<BaseOperationResponse> Delete(SignagePublication signagePublication)
        {
            var result = new BaseOperationResponse();
            SoftDelete(signagePublication);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
            }
            else
            {
                result.Message = "Failed to delete signage publication!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<byte[]> GetHistory(int? publicationId)
        {
            if (publicationId == null) return null;

            using (MemoryStream stream = new MemoryStream())
            {
                var wb = new HSSFWorkbook();
                var sheet = (HSSFSheet)wb.CreateSheet("Sheet1");
                var headers = new List<string>(new string[] { 
                    "Publication Name",
                    "Create By",
                    "Create Date", 
                    "Approved By",
                    "Approved Date",
                    "Rejected By",
                    "Rejected Date",
                    "Status",  
                    "Remark",
                    });

                var history = (await GetByIdAsync(publicationId.Value)).History.ToList();
                history = history.Where(h => h.IsActive).ToList();

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
                history
                    .ForEach(t =>
                    {
                        var tinfo = t;
                        var towid = "";
                        var towCode = "";

                        row = sheet.GetRow(rowCount);
                        if (row == null)
                            row = sheet.CreateRow(rowCount);

                        var i = 0;
                        row.CreateCell(i++).SetCellValue(tinfo.Publication.Name);
                        row.CreateCell(i++).SetCellValue(tinfo.Publication.CreatedByUser.FullName);
                        row.CreateCell(i++).SetCellValue(tinfo.Publication.CreatedDate.ToString("yyyy-MM-dd hh:mm:ss"));
                        row.CreateCell(i++).SetCellValue(tinfo.ApprovedByUser?.FullName);
                        row.CreateCell(i++).SetCellValue(tinfo.ApprovedDate != null ? tinfo.ApprovedDate?.ToString("yyyy-MM-dd hh:mm:ss") : "");
                        row.CreateCell(i++).SetCellValue(tinfo.RejectedByUser?.FullName);
                        row.CreateCell(i++).SetCellValue(tinfo.RejectedDate != null ? tinfo.RejectedDate?.ToString("yyyy-MM-dd hh:mm:ss") : "");
                        row.CreateCell(i++).SetCellValue(tinfo.Approved ? "Approved" : (tinfo.Rejected ? "Rejected" : "Pending"));
                        row.CreateCell(i++).SetCellValue(tinfo.Remark);

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

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
