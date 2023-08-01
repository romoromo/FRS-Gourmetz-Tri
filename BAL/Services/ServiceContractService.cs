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
using DAL.Repositories.Interfaces;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Drawing;
using NPOI.HSSF.Util;
using NPOI.SS.Util;

namespace BAL.Services
{
    public class ServiceContractService : IServiceContractService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;

        public ServiceContractService(IUnitOfWork uow, ISieveProcessor sieveProcessor)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
        }

        #region Service Contracts

        public async Task<PagedEntity<ServiceContractDTO>> GetServiceContractsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<ServiceContractDTO>>(await this._uow.ServiceContracts.GetServiceContractsAsync(filter));
            return result;
        }

        public async Task<ServiceContractDTO> GetServiceContractByIdAsync(int id)
        {
            return Mapper.Map<ServiceContractDTO>(await this._uow.ServiceContracts.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateServiceContractAsync(ServiceContractDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ServiceContracts.CreateAsync(Mapper.Map<ServiceContract>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateServiceContractsync(ServiceContractDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ServiceContracts.UpdateAsync(Mapper.Map<ServiceContract>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteServiceContractAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ServiceContracts.DeleteAsync(id);
            return result;
        }

        public async Task<byte[]> GenerateReportXls(ServiceContractFilter filter)
        {
            var query = await this.GetServiceContractsAsync(filter.Filter);
            var results = query.PagedData.ToList();

            if (results != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Service Contracts");
                    var headers = new string[] { "#", "Contract No", "Reference", "Coverage", "Start Date", "End Date", "Details", "Renewal Alert (in Days)" };

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
                    borderedHeaderStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Grey50Percent.Index;
                    borderedHeaderStyle.FillForegroundColor = HSSFColor.Grey50Percent.Index;
                    borderedHeaderStyle.FillPattern = FillPattern.SolidForeground;

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
                    int rowNo = 1;
                    results.ForEach(dt =>
                    {

                        #region information
                        row = sheet.CreateRow(++rowCount);

                        int cellIndx = 0;
                        cell = row.CreateCell(cellIndx++);
                        cell.SetCellValue(rowNo++);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(cellIndx++);
                        cell.SetCellValue(dt.Identifier);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(cellIndx++);
                        cell.SetCellValue(dt.Reference);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(cellIndx++);
                        cell.SetCellValue(dt.Coverage);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(cellIndx++);
                        cell.SetCellValue(dt.StartDate.HasValue ? dt.StartDate.Value.ToString("dd/MM/yyyy") : string.Empty);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(cellIndx++);
                        cell.SetCellValue(dt.EndDate.HasValue ? dt.EndDate.Value.ToString("dd/MM/yyyy") : string.Empty);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(cellIndx++);
                        cell.SetCellValue(dt.Details);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(cellIndx++);
                        cell.SetCellValue(dt.RenewalAlert.GetValueOrDefault());
                        cell.CellStyle = contentStyle;

                        #endregion

                        #region assets

                        if (filter.IncludeAssets)
                        {
                            rowCount = this.CreateAssetXlsTable(dt.ServiceContractAssets, wb, sheet, ++rowCount, headerStyle, headerFont);
                            ++rowCount;
                        }
                        #endregion

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

        private int CreateAssetXlsTable(List<ServiceContractAssetDTO> assets, XSSFWorkbook wb, XSSFSheet sheet, int rowCount, ICellStyle headerStyle, IFont headerFont)
        {
            var headers = new string[] { "#", "Serial Number", "Model", "Asset Type", "Purchase Date", "Warranty Start", "Warranty End" };
            #region Headers

            headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
            headerStyle.SetFont(headerFont);
            headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            var row = sheet.CreateRow(++rowCount);

            var listOfAssetcell = row.CreateCell(1);
            listOfAssetcell.SetCellValue("List of Assets");
            listOfAssetcell.CellStyle = headerStyle;
            sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum, 1, 7));

            row = sheet.CreateRow(++rowCount);
            var borderedHeaderStyle = wb.CreateCellStyle();
            borderedHeaderStyle.SetFont(headerFont);
            borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            borderedHeaderStyle.BorderTop = BorderStyle.Thin;
            borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
            borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
            borderedHeaderStyle.BorderRight = BorderStyle.Thin;
            borderedHeaderStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Grey50Percent.Index;
            borderedHeaderStyle.FillForegroundColor = HSSFColor.Grey50Percent.Index;
            borderedHeaderStyle.FillPattern = FillPattern.SolidForeground;

            ICell cell;
            int startCol = 1, rowNo = 1;
            
            for (var i = startCol; i < headers.Length + 1; i++)
            {
                cell = row.CreateCell(i);
                cell.SetCellValue(headers[i - 1]);
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
            assets.ForEach(dt =>
            {

                #region information
                row = sheet.CreateRow(++rowCount);
                startCol = 1;
                cell = row.CreateCell(startCol++);
                cell.SetCellValue(rowNo++);
                cell.CellStyle = contentStyle;

                cell = row.CreateCell(startCol++);
                cell.SetCellValue(dt.SerialNumber);
                cell.CellStyle = contentStyle;

                cell = row.CreateCell(startCol++);
                cell.SetCellValue(dt.AssetModelName);
                cell.CellStyle = contentStyle;

                cell = row.CreateCell(startCol++);
                cell.SetCellValue(dt.AssetTypeName);
                cell.CellStyle = contentStyle;

                cell = row.CreateCell(startCol++);
                cell.SetCellValue(dt.PurchaseDate.HasValue ? dt.PurchaseDate.Value.ToString("dd/MM/yyyy") : string.Empty);
                cell.CellStyle = contentStyle;

                cell = row.CreateCell(startCol++);
                cell.SetCellValue(dt.WarrantyStart.HasValue ? dt.WarrantyStart.Value.ToString("dd/MM/yyyy") : string.Empty);
                cell.CellStyle = contentStyle;

                cell = row.CreateCell(startCol++);
                cell.SetCellValue(dt.WarrantyEnd.HasValue ? dt.WarrantyEnd.Value.ToString("dd/MM/yyyy") : string.Empty);
                cell.CellStyle = contentStyle;

                #endregion

            });

            #endregion

            return rowCount;
        }

        #endregion
    }
}
