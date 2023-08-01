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
using System.IO;
using NPOI.HSSF.UserModel;
using BAL.Services.Interfaces.MealOrder;
using BAL.DTO.MealOrder;
using DAL.Models.MealOrder;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace BAL.Services.MealOrder
{
    public class ClassService : IClassService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;

        public ClassService(IUnitOfWork uow, ISieveProcessor sieveProcessor)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
        }

        #region Class Batches

        public async Task<PagedEntity<ClassBatchDTO>> GetClassBatchesAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<ClassBatchDTO>>(await this._uow.ClassBatches.GetClassBatchesAsync(filter));
            return result;
        }

        public async Task<ClassBatchDTO> GetClassBatchByIdAsync(int id)
        {
            return Mapper.Map<ClassBatchDTO>(await this._uow.ClassBatches.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateClassBatchAsync(ClassBatchDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ClassBatches.CreateAsync(Mapper.Map<ClassBatch>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateClassBatchAsync(ClassBatchDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ClassBatches.UpdateAsync(Mapper.Map<ClassBatch>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteClassBatchAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ClassBatches.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Class Levels

        public async Task<PagedEntity<ClassLevelDTO>> GetClassLevelsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<ClassLevelDTO>>(await this._uow.ClassLevels.GetClassLevelsAsync(filter));
            return result;
        }

        public async Task<ClassLevelDTO> GetClassLevelByIdAsync(int id)
        {
            return Mapper.Map<ClassLevelDTO>(await this._uow.ClassLevels.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateClassLevelAsync(ClassLevelDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ClassLevels.CreateAsync(Mapper.Map<ClassLevel>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateClassLevelAsync(ClassLevelDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ClassLevels.UpdateAsync(Mapper.Map<ClassLevel>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteClassLevelAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ClassLevels.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Classes

        public async Task<PagedEntity<ClassDTO>> GetClassesAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<ClassDTO>>(await this._uow.Classes.GetClassesAsync(filter));
            return result;
        }

        public async Task<ClassDTO> GetClassByIdAsync(int id)
        {
            return Mapper.Map<ClassDTO>(await this._uow.Classes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateClassAsync(ClassDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Classes.CreateAsync(Mapper.Map<Class>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateClassAsync(ClassDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Classes.UpdateAsync(Mapper.Map<Class>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteClassAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Classes.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Class Rostering
        public async Task<PagedEntity<OutletClassRosterDTO>> GetOutletClassRostersAsync(ClassRosterFilter filter)
        {
            var result = Mapper.Map<PagedEntity<OutletClassRosterDTO>>(await this._uow.OutletClassRosters.GetOutletClassRostersAsync(filter));
            return result;
        }

        public async Task<BaseOperationResponse> CreateOutletClassRosterAsync(OutletClassRosterDTO dto)
        {
            return await this._uow.OutletClassRosters.CreateAsync(Mapper.Map<OutletClassRoster>(dto));
        }

        public async Task<List<OutletClassRosterDTO>> GetOutletClassRosterByIdAsync(int outletId, int catererId, int mealSessionId)
        {
            return Mapper.Map<List<OutletClassRosterDTO>>(await this._uow.OutletClassRosters.GetByCatererOutletIdAsync(outletId, catererId, mealSessionId));
        }

        public async Task<BaseOperationResponse> UpdateOutletClassRosterAsync(OutletClassRosterDTO dto)
        {
            return await this._uow.OutletClassRosters.UpdateAsync(Mapper.Map<OutletClassRoster>(dto));
        }

        public async Task<OutletClassRosterDTO> GetOutletClassRosterByIdAsync(int id)
        {
            return Mapper.Map<OutletClassRosterDTO>(await this._uow.OutletClassRosters.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> DeleteOutletClassRosterAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.OutletClassRosters.DeleteAsync(id);
            return result;
        }

        public async Task<MealSessionDetailDTO> GetCurrentOrderMealSessionAsync(int? outletId, DateTime orderDate, int mealSessionId, int classId)
        {
            var result = Mapper.Map<MealSessionDetailDTO>(await this._uow.OutletClassRosters.GetCurrentOrderMealSessionAsync(outletId, orderDate, mealSessionId, classId));
            return result;
        }

        #region Class Rosters
        public async Task<byte[]> GenerateClassRostersXls(ClassRosterFilter filter)
        {
            var classRoster = await _uow.OutletClassRosters.GetByIdAsync(filter.ClassRosterId);
            string dateFormat = "dd/MM/yy";
            string dateTimeFormat = "dd/MM/yy hh:mm tt";
            string decimalFormat = "0.00";

            if (classRoster != null)
            {
                var periods = classRoster.MealSession.Details.Where(e => e.IsActive).ToList().OrderBy(e => e.StartDate.TimeOfDay);
                var roster = Mapper.Map<OutletClassRosterDTO>(classRoster);
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Class Roster Report");
                    var headers = new List<string>();

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.FontName = "Calibri";
                    headerFont.FontHeightInPoints = 11;
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var row = sheet.CreateRow(rowCount++);
                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                    ICell cell;

                    cell = row.CreateCell(0);
                    cell.SetCellValue(roster.Label);
                    cell.CellStyle = borderedHeaderStyle;

                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("Start Date");
                    cell.CellStyle = borderedHeaderStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(roster.StartDate.ToString(dateFormat));
                    cell.CellStyle = borderedHeaderStyle;

                    row = sheet.CreateRow(rowCount++);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("End Date");
                    cell.CellStyle = borderedHeaderStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(roster.EndDate?.ToString(dateFormat));
                    cell.CellStyle = borderedHeaderStyle;

                    #endregion

                    #region Content
                    var contentStyle = wb.CreateCellStyle();
                    var contentFont = wb.CreateFont();
                    contentFont.FontName = "Calibri";
                    contentFont.FontHeightInPoints = 11;
                    contentStyle.SetFont(contentFont);
                    contentStyle.VerticalAlignment = VerticalAlignment.Top;
                    contentStyle.Alignment = HorizontalAlignment.Left;
                    var dataFormatCustom = wb.CreateDataFormat();


                    var createHelper = wb.GetCreationHelper();
                    var format = wb.CreateDataFormat();
                    var dateCellStyle = wb.CreateCellStyle();
                    dateCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(dateFormat);
                    dateCellStyle.Alignment = HorizontalAlignment.Right;
                    dateCellStyle.SetFont(contentFont);

                    var dateTimeCellStyle = wb.CreateCellStyle();
                    dateTimeCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(dateTimeFormat);
                    dateTimeCellStyle.Alignment = HorizontalAlignment.Right;
                    dateTimeCellStyle.SetFont(contentFont);

                    var numericCellStyle = wb.CreateCellStyle();
                    numericCellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat(decimalFormat);
                    numericCellStyle.Alignment = HorizontalAlignment.Right;
                    numericCellStyle.SetFont(contentFont);


                    int col = 1;
                    rowCount = rowCount + 2;
                    row = sheet.CreateRow(rowCount++);

                    foreach (var mealPeriod in periods)
                    {
                        cell = row.CreateCell(col++);
                        cell.SetCellValue(mealPeriod.Name);
                        cell.CellStyle = borderedHeaderStyle;
                    }

                    var schedules = roster.Schedules.OrderBy(e => e.Day).ToList();
                    schedules.ForEach(dt =>
                    {
                        row = sheet.CreateRow(rowCount++);
                        col = 0;
                        cell = row.CreateCell(col++);
                        cell.SetCellValue(string.Format("Day {0}", dt.Day));
                        cell.CellStyle = borderedHeaderStyle;

                        foreach (var mealPeriod in periods)
                        {
                            var classes = dt.Periods.Where(e => e.MealSessionDetailId == mealPeriod.Id).SelectMany(e => e.Classes).Select(e => e.Name).Distinct();
                            cell = row.CreateCell(col++);
                            cell.SetCellValue(string.Join(",", classes));
                            cell.CellStyle = contentStyle;
                        }
                    });

                    #endregion

                    for (var i = 0; i < 30; i++)
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

        #endregion
    }
}
