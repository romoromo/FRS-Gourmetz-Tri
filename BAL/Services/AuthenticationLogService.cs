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

namespace BAL.Services
{
    public class AuthenticationLogService : IAuthenticationLogService
    {
        private ISieveProcessor _sieveProcessor;
        private ApplicationDbContext _appContext;
        private readonly IMapper _mapper;

        public AuthenticationLogService(ApplicationDbContext context, ISieveProcessor sieveProcessor, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._appContext = context;
            _mapper = mapper;
        }

        #region Sieved
        public async Task<PagedEntity<AuthenticationLogDTO>> GetAuthenticationLogsAsync(BaseFilter filter)
        {
            IQueryable<AuthenticationLog> query = _appContext.AuthenticationLogs;

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            int totalCount = query.Count();
            query = _sieveProcessor.Apply(filter, query, applyFiltering: false, applySorting: false);
            var result = new PagedEntity<AuthenticationLogDTO>
            {
                Filter = filter,
                TotalCount = totalCount,
                PagedData =  _mapper.Map<List<AuthenticationLogDTO>>(await query.ToListAsync())
            };

            return result;
        }

        public async Task<byte[]> GenerateAuthenticationLogXls(BaseFilter filter)
        {
            IQueryable<AuthenticationLog> query = _appContext.AuthenticationLogs;

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            var logs = _mapper.Map<List<AuthenticationLogDTO>>(await query.ToListAsync());

            if (logs != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Authentication Logs");
                    var headers = new string[] { "User Name", "Message", "Date" };

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
                        cell.SetCellValue(dt.UserName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(1);
                        cell.SetCellValue(dt.Message);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(2);
                        cell.SetCellValue(dt.CreatedDate.ToString("dd/MM/yyyy hh:mm:ss tt"));
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
        public async Task<AuthenticationLogDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<AuthenticationLogDTO>(await _appContext.AuthenticationLogs.FindAsync(id));
        }

        public async Task<BaseOperationResponse> CreateAsync(AuthenticationLogDTO logDTO)
        {
            var result = new BaseOperationResponse();
            try
            {
                var log = _mapper.Map<AuthenticationLog>(logDTO);

                var f = await _appContext.AuthenticationLogs.AddAsync(log);
                if (await _appContext.SaveChangesAsync() > 0)
                {
                    result.Message = "Successfully saved!";
                    result.IsSuccess = true;
                }
                else
                {
                    result.Message = "Failed to save Authentication Log!";
                    result.IsSuccess = false;
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        
    }
}
