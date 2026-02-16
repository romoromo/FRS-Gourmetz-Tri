using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL;
using BAL.Services.Interfaces;
using BAL.DTO;
using AutoMapper;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using DAL.Models.MealOrder;
using Microsoft.Data.SqlClient;
using DAL.Models.StoredProcedures;
using System.Data;
using BAL.DTO.MealOrder;
using DAL.Core.DTO;

namespace BAL.Services
{
    public class AuditLogService : IAuditLogService
    {
        private ISieveProcessor _sieveProcessor;
        private ApplicationDbContext _appContext;
        private readonly IMapper _mapper;

        public AuditLogService(ApplicationDbContext context, ISieveProcessor sieveProcessor, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._appContext = context;
            _mapper = mapper;
        }

        #region Sieved
        public async Task<PagedEntity<AuditLogDTO>> GetDataLogsAsync(BaseFilter filter)
        {
            IQueryable<AuditLog> query = _appContext.AuditLogs;

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<AuditLogDTO>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData =  _mapper.Map<List<AuditLogDTO>>(await query.ToListAsync())
            };

            return result;
        }

        public async Task<PagedEntity<AuditLogDetailDTO>> GetDataLogDetailsAsync(BaseFilter filter)
        {
            IQueryable<AuditLogDetail> query = _appContext.AuditLogDetails;

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<AuditLogDetailDTO>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = _mapper.Map<List<AuditLogDetailDTO>>(await query.ToListAsync())
            };

            return result;
        }

        public async Task<byte[]> GenerateDataLogXls(BaseFilter filter)
        {
            IQueryable<AuditLog> query = _appContext.AuditLogs;

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            //var logs = _mapper.Map<List<AuditLogDTO>>(await query.ToListAsync());
            var logs = await query.ToListAsync();

            if (logs != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Audit Logs");

                    var detailsCount = 0;

                    logs.ForEach(dt =>
                    {
                        detailsCount = Math.Max(detailsCount, dt.Details?.Count() ?? 0);
                    });

                    var headers = new List<string> { "User Name", "Type", "Date", "Table", "Record ID"};

                    for(int i = 0; i < detailsCount; i++)
                    {
                        headers.Add("Property Name");
                        headers.Add("Original Value");
                        headers.Add("New Value");
                    }

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                    ICell cell;
                    for (var i = 0; i < headers.Count(); i++)
                    {
                        cell = row.CreateCell(i);
                        cell.SetCellValue(headers[i]);
                        cell.CellStyle = borderedHeaderStyle;
                    }
                    sheet.AutoSizeColumn(0);

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    contentStyle.BorderTop = BorderStyle.Thin;
                    contentStyle.BorderBottom = BorderStyle.Thin;
                    contentStyle.BorderLeft = BorderStyle.Thin;
                    contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    contentStyle.WrapText = false;
                    var dataFormatCustom = wb.CreateDataFormat();
                    logs.ForEach(dt =>
                    {
                        row = sheet.CreateRow(++rowCount);

                        var i = 0;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.UserName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(Enum.GetName(typeof(AuditLogType), dt.LogType));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.EventDateTime.ToString("dd/MM/yyyy hh:mm:ss tt"));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.TableName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(i++);
                        cell.SetCellValue(dt.RecordId);
                        cell.CellStyle = contentStyle;

                        //var propertyName = "";
                        //var originalValue = "";
                        //var newValue = "";

                        dt.Details?.ToList().ForEach(det =>
                        {
                            //propertyName += det.PropertyName + "\n";
                            //originalValue += det.OriginalValue + "\n";
                            //newValue += det.NewValue + "\n";

                            cell = row.CreateCell(i++);
                            cell.SetCellValue(det.PropertyName);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(i++);
                            cell.SetCellValue(det.OriginalValue);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(i++);
                            cell.SetCellValue(det.NewValue);
                            cell.CellStyle = contentStyle;
                        });

                        

                    });

                    #endregion

                    for (var i = 0; i < headers.Count(); i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        #endregion
        public async Task<AuditLogDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<AuditLogDTO>(await _appContext.AuthenticationLogs.FindAsync(id));
        }

        #region external login log
        public async Task<PagedEntity<ExternalAppLoginLogDTO>> GetExternalLoginLogsAsync(BaseFilter filter)
        {
            IQueryable<ExternalAppLoginLog> query = _appContext.ExternalAppLoginLogs;

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<ExternalAppLoginLogDTO>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData = _mapper.Map<List<ExternalAppLoginLogDTO>>(await query.ToListAsync())
            };

            return result;
        }

        public async Task<BaseOperationResponse> CreateExternalLoginLogAsync(ExternalAppLoginLogDTO dto)
        {
            var externalAppLoginLog = _mapper.Map<ExternalAppLoginLog>(dto);

            var result = new BaseOperationResponse();
            var f = await _appContext.ExternalAppLoginLogs.AddAsync(externalAppLoginLog);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save record!";
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<byte[]> GenerateExternalLoginLogXls(BaseFilter filter)
        {
            var results = await GetExternalLoginLogsAsync(filter);

            if (results != null)
            {
                var logs = results.PagedData.ToList();
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Order Portal Login Logs");
                    var headers = new string[] { "Username", "Email", "Message", "Date" };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                    ICell cell;
                    for (var i = 0; i < headers.Length; i++)
                    {
                        cell = row.CreateCell(i);
                        cell.SetCellValue(headers[i]);
                        cell.CellStyle = borderedHeaderStyle;
                    }
                    sheet.AutoSizeColumn(0);

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    contentStyle.BorderTop = BorderStyle.Thin;
                    contentStyle.BorderBottom = BorderStyle.Thin;
                    contentStyle.BorderLeft = BorderStyle.Thin;
                    contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();
                    logs.ForEach(dt =>
                    {
                        row = sheet.CreateRow(++rowCount);

                        cell = row.CreateCell(0);
                        cell.SetCellValue(dt.Username);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(1);
                        cell.SetCellValue(dt.Email);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(2);
                        cell.SetCellValue(dt.Message);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(3);
                        cell.SetCellValue(dt.EventDateTime.ToString("dd/MM/yyyy hh:mm:ss tt"));
                        cell.CellStyle = contentStyle;

                    });

                    #endregion

                    for (var i = 0; i < headers.Length; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region User Activity Log
        public async Task<PagedEntity<UserActivityLogDTO>> GetUserActivityLogs(UserActivityLogReportFilter filter)
        {
            var results = await GetUserActivityLogsByFilter(filter);
            var logs = new List<UserActivityLogDTO>();
            foreach(var header in results.Headers)
            {
                var details = _mapper.Map<List<UserActivityLogDetailDTO>>(results.Details.Where(e => e.GroupId == header.GroupId));
                var log = new UserActivityLogDTO
                {
                    GroupId = header.GroupId,
                    Remarks = header.Remarks,
                    Total = header.Total,
                    Username = header.Username,
                    EventDateTime = details.FirstOrDefault().EventDateTime,
                    Details = details
                };

                logs.Add(log);
            }

            int total = logs != null && logs.Any() ? logs.FirstOrDefault().Total : 0;
            var result = new PagedEntity<UserActivityLogDTO>();
            result.Filter = filter;
            result.PagedData = logs;
            result.CurrentPage = filter.Page ?? 1;
            result.PageSize = filter.PageSize ?? 10;
            result.PageCount = total / result.PageSize;
            result.TotalCount = total;

            return result;
        }

        private async Task<spGetUserActivityLogDTO> GetUserActivityLogsByFilter(UserActivityLogReportFilter filter)
        {
            var result = new spGetUserActivityLogDTO();
            var from = new SqlParameter("@ReportDateFrom", System.Data.SqlDbType.Date);
            var to = new SqlParameter("@ReportDateTo", System.Data.SqlDbType.Date);
            var keywords = new SqlParameter("@UserName", System.Data.SqlDbType.NVarChar);
            var reportType = new SqlParameter("@ActionName", System.Data.SqlDbType.NVarChar);

            from.Value = filter.ReportDateFrom;
            to.Value = filter.ReportDateTo;
            keywords.Value = (object)filter.Keyword ?? DBNull.Value;
            reportType.Value = (object)filter.ReportType ?? DBNull.Value;

            using (var command = _appContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "spGetUserActivityLog";
                command.Parameters.AddRange(new[] { from, to, keywords, reportType });

                _appContext.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var header = _mapper.Map<IDataRecord, spGetUserActivityLogHeader>((IDataRecord)reader);
                        result.Headers.Add(header);
                    }

                    //MyDTO dto = _mapper.Map<IDataRecord, MyDTO>((IDataRecord)reader);

                    //// Read the first result set (header) into a list of UserActivityLogHeader
                    //var headers = _appContext.Set<spGetUserActivityLogHeader>().FromSql("exec spGetUserActivityLog @ReportDateFrom, @ReportDateTo, @UserName, @ActionName", from, to, keywords, reportType).ToList();
                    //result.Headers = headers;
                    // Move to the next result set (detail)
                    reader.NextResult();

                    while (reader.Read())
                    {
                        var detail = _mapper.Map<IDataRecord, spGetUserActivityLogDetail>((IDataRecord)reader);
                        result.Details.Add(detail);
                    }

                    // Read the second result set (detail) into a list of UserActivityLogDetail
                    //var details = _appContext.Set<spGetUserActivityLogDetail>().FromSql("exec spGetUserActivityLog @ReportDateFrom, @ReportDateTo, @UserName, @ActionName", from, to, keywords, reportType).ToList();
                    //result.Details = details;
                    // Process the header and detail as needed
                }
            }

            //var logs = await _appContext.spGetUserActivityLog
            //                .FromSql($"exec spGetUserActivityLog @ReportDateFrom, @ReportDateTo, @UserName, @ActionName",
            //                        from, to, keywords, reportType).ToListAsync();

            return result;
        }

        public async Task<byte[]> GenerateUserActivityLogsXls(UserActivityLogReportFilter filter)
        {
            var results = await GetUserActivityLogsByFilter(filter);
            var logs = new List<UserActivityLogDTO>();
            foreach (var header in results.Headers)
            {
                var details = _mapper.Map<List<UserActivityLogDetailDTO>>(results.Details.Where(e => e.GroupId == header.GroupId));
                var log = new UserActivityLogDTO
                {
                    GroupId = header.GroupId,
                    Remarks = header.Remarks,
                    Total = header.Total,
                    Username = header.Username,
                    EventDateTime = details.FirstOrDefault().EventDateTime,
                    Details = details
                };

                logs.Add(log);
            }


            if (results != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("User Activity Logs");
                    var headers = new string[] { "Transaction", "Username", "Event Date/Time", "Remarks", "Log Type", "Details" };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                    ICell cell;
                    for (var i = 0; i < headers.Length; i++)
                    {
                        cell = row.CreateCell(i);
                        cell.SetCellValue(headers[i]);
                        cell.CellStyle = borderedHeaderStyle;
                    }
                    sheet.AutoSizeColumn(0);

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    contentStyle.BorderTop = BorderStyle.Thin;
                    contentStyle.BorderBottom = BorderStyle.Thin;
                    contentStyle.BorderLeft = BorderStyle.Thin;
                    contentStyle.BorderRight = BorderStyle.Thin;
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    contentStyle.WrapText = true;
                    var dataFormatCustom = wb.CreateDataFormat();
                    logs.ForEach(dt =>
                    {
                        foreach (var d in dt.Details)
                        {
                            row = sheet.CreateRow(++rowCount);

                            int i = 0;
                            cell = row.CreateCell(i++);
                            cell.SetCellValue(dt.GroupId);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(i++);
                            cell.SetCellValue(dt.Username);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(i++);
                            cell.SetCellValue(dt.EventDateTime.ToString("dd/MM/yyyy hh:mm:ss tt"));
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(i++);
                            cell.SetCellValue(dt.Remarks);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(i++);
                            cell.SetCellValue(d.LogType);
                            cell.CellStyle = contentStyle;

                            cell = row.CreateCell(i++);
                            cell.SetCellValue(d.DetailRemarks);
                            cell.CellStyle = contentStyle;
                        }

                    });

                    #endregion

                    for (var i = 0; i < headers.Length; i++)
                    {
                        sheet.AutoSizeColumn(i, true);
                    }

                    wb.Write(stream);

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
