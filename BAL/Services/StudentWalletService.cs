using AutoMapper;
using BAL.DTO;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces;
using DAL;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Core.Helpers;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class StudentWalletService : IStudentWalletService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private ApplicationDbContext _appContext;

        public StudentWalletService(ISieveProcessor sieveProcessor, IUnitOfWork uow, IMapper mapper, ApplicationDbContext appContext)
        {
            _sieveProcessor = sieveProcessor;
            _uow = uow;
            _mapper = mapper;
            _appContext = appContext;
        }

        public async Task<PagedEntity<StudentWalletTransactionDTO>> GetWalletTransactionsAsync(EWalletTransactionFilter filter)
        {
            var result = _mapper.Map<PagedEntity<StudentWalletTransactionDTO>>(await this._uow.StudentWalletTransactions.GetWalletTransactionsAsync(filter));
            return result;
        }

        public async Task<PagedEntity<StudentWalletTransactionSimpleDTO>> GetWalletTransactionsSimpleAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<StudentWalletTransactionSimpleDTO>>(await this._uow.StudentWalletTransactions.GetWalletTransactionsSimpleAsync(filter));
            return result;
        }

        public async Task<List<StudentWalletTransactionDTO>> GetWalletTransactionByIdAsync(int id)
        {
            return _mapper.Map<List<StudentWalletTransactionDTO>>((await this._uow.StudentWalletTransactions.FindAsync(e => e.StudentId == id)).ToList());
        }

        public async Task<BaseOperationResponse> StudentWalletTransactionAsync(StudentWalletTransactionDTO dto)
        {
            var result = new BaseOperationResponse();
            var student = await this._uow.Students.GetByIdAsync(dto.StudentId);

            //if (!student.ConcurrencyStamp.SequenceEqual(dto.ConcurrencyStamp))
            //{
            //    result.IsSuccess = false;
            //    result.Message = "Student is not the latest version. Please refresh.";
            //    return result;
            //}
            //else
            //{
            if (string.IsNullOrEmpty(dto.TransactionType) ||
                (!dto.TransactionType.Equals(RewardTransactionType.CREDIT.ToString(), StringComparison.OrdinalIgnoreCase) &&
                !dto.TransactionType.Equals(RewardTransactionType.DEBIT.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                result.IsSuccess = false;
                result.Message = "Transaction type is missing or invalid.";
                return result;
            }

            if (dto.TransactionType.Equals(WalletTransactionType.CREDIT.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                student.WalletBalance += dto.Amount;
            }
            else if (dto.TransactionType.Equals(WalletTransactionType.DEBIT.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                if (student.WalletBalance - dto.Amount < 0)
                {
                    result.IsSuccess = false;
                    result.Message = "Insufficient balance.";
                    return result;
                }
                else if (student.IsWalletFreeze)
                {
                    result.IsSuccess = false;
                    result.Message = "Wallet is Freezed";
                    return result;
                }
                else
                {
                    student.WalletBalance -= dto.Amount;
                }
            }

            result = await this._uow.Students.UpdateAsync(student);

            if (result.IsSuccess)
            {
                var transaction = new StudentWalletTransaction
                {
                    Amount = dto.Amount,
                    TransactionType = dto.TransactionType,
                    StudentId = dto.StudentId,
                    Description = dto.Description
                };

                await this._uow.StudentWalletTransactions.CreateAsync(transaction);
            }

            var d = _mapper.Map<StudentDTO>(result.Data);
            result.Data = d;
            //}

            return result;
        }

        public async Task<BaseOperationResponse> TopupWalletBalanceByStudentGroupIdAsync(int studentGroupId, double amount, int userId, WalletType walletTypeData)
        {
            return await this._uow.StudentWalletTransactions.TopupWalletBalanceByStudentGroupIdAsync(studentGroupId, amount, userId, walletTypeData);
        }

        public async Task<BaseOperationResponse> TopupWalletBalanceByStudentIdAsync(int studentId, double amount, int userId, WalletType walletType, int? paymentId)
        {
            return await this._uow.StudentWalletTransactions.TopupWalletBalanceByStudentIdAsync(studentId, amount, userId, walletType, paymentId);
        }

        public async Task<BaseOperationResponse> RefundToWalletBalanceAsync(int studentId, double amount, int userId, WalletType walletType, int? tokenOrderId)
        {
            return await this._uow.StudentWalletTransactions.RefundToWalletBalanceAsync(studentId, amount, userId, walletType, tokenOrderId);
        }

        public async Task<BaseOperationResponse> OffBoardingStudent(int studentId, int userId)
        {
            return await this._uow.StudentWalletTransactions.OffBoardingStudent(studentId, userId);
        }

        public async Task<byte[]> GenerateXls(EWalletTransactionFilter filter)
        {
            //IQueryable<StudentWalletTransaction> query = _appContext.StudentWalletTransactions.Where(m => m.student.IsActive);
            var cutOffDate = new DateTime(2025, 10, 1);
            IQueryable<StudentWalletTransaction> query = _appContext.StudentWalletTransactions
                .Where(m => m.student != null);
            //    .Where(m => m.student.IsActive ||
            //                (filter.IncludeOffboarded && !m.student.IsActive && m.CreatedDate >= cutOffDate));			

            // Filter by StudentType: All / Active / Offboarded
            if (!string.IsNullOrEmpty(filter.StudentType) && filter.StudentType != "All")
            {
                if (filter.StudentType == "Active")
                    query = query.Where(m => m.student.IsActive);
                else if (filter.StudentType == "Offboarded")
                    query = query.Where(m => !m.student.IsActive && m.student.UpdatedDate >= cutOffDate);
            }
            else
            {
                // StudentType == "All": active + offboarded since Oct 1, 2025
                query = query.Where(m => m.student.IsActive ||
                            (!m.student.IsActive && m.student.UpdatedDate >= cutOffDate));
            }

            if (!string.IsNullOrEmpty(filter.PosInvoiceId))
            {
                query = query.Where(m => m.Payment.PosInvoiceId.Contains(filter.PosInvoiceId));
            }

            if (!string.IsNullOrEmpty(filter.Source) && filter.Source != "ALL")
                query = query.Where(m => m.Source == filter.Source);

            if (filter.OutletId != null && filter.OutletId.Count != 0)
            {
                query = query.Where(m => filter.OutletId.Contains(m.student.OutletId.Value));
            }

            if (filter.IsFAS != null)
            {
                query = query.Where(m => m.student.IsFAS == filter.IsFAS);
            }

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            var logs = query.Select(m =>

            new StudentWalletTransactionDTO
            {
                StudentId = m.StudentId,
                StudentName = m.student.Name,
                TransactionDateTime = m.CreatedDate,
                Source = m.Source,
                Amount = m.Amount,
                TransactionType = m.TransactionType,
                Description = m.Description,
                StripeId = m.WalletPayment.fomoid,
                PosInvoiceId = string.IsNullOrEmpty(m.Payment.PosInvoiceId) ? m.Payment.POSSales.no_invoice : m.Payment.PosInvoiceId,
                InvoiceNumber = m.Payment.version == "SUCCESS" ? "" : m.Payment.InvoiceNumber,
                DishLabels = m.TokenOrder.Tokens.SelectMany(t => t.SelectedDishes.Select(d => d.Dish.Label)).ToList(),
                NormalTopup = m.NormalTopup,
                NormalExpensed = m.NormalExpensed,
                FasTopup = m.FasTopup,
                FasExpensed = m.FasExpensed,
                ClassLevel = m.student.ClassLevel.Name,
                Remarks = m.Remarks,
                UserName = m.CreatedByUser.UserName,
                IsStudentActive = m.student.IsActive
            }).AsAsyncEnumerable();

            if (logs != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Wallet Transactions");
                    var headers = new string[] {
                        "Student",
                        "Date",
                        "Source",
                        "Amount",
                        "Type",
                        "Description",
                        "REF_ID",
                        "POS Invoice",
                        "Invoice",
                        "Dish Name",
                        "Normal Topup",
                        "Normal Expensed",
                        "FAS Topup",
                        "FAS Expensed",
                        "Class Level",
                        "Remarks",
                        "Processed By",
                        "Student Status"};

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
                    await foreach (var dt in logs)
                    {
                        row = sheet.CreateRow(++rowCount);

                        cell = row.CreateCell(0);
                        cell.SetCellValue(dt.StudentName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(1);
                        cell.SetCellValue(dt.TransactionDateTime?.ToString("dd/MM/yyyy hh:mm:ss tt"));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(2);
                        cell.SetCellValue(dt.Source);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(3);
                        cell.SetCellValue(dt.Amount);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(4);
                        cell.SetCellValue(dt.TransactionType);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(5);
                        cell.SetCellValue(dt.Description);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(6);
                        cell.SetCellValue(dt.StripeId);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(7);
                        cell.SetCellValue(dt.PosInvoiceId);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(8);
                        cell.SetCellValue(dt.InvoiceNumber);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(9);
                        cell.SetCellValue(string.Join(", ", dt.DishLabels));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(10);
                        cell.SetCellValue(dt.NormalTopup);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(11);
                        cell.SetCellValue(dt.NormalExpensed);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(12);
                        cell.SetCellValue(dt.FasTopup);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(13);
                        cell.SetCellValue(dt.FasExpensed);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(14);
                        cell.SetCellValue(dt.ClassLevel);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(15);
                        cell.SetCellValue(dt.Remarks);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(16);
                        cell.SetCellValue(dt.UserName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(17);
                        cell.SetCellValue(dt.StudentStatus);
                        cell.CellStyle = contentStyle;

                    };

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

        public async Task<byte[]> GenerateWalletTransactionByStudent(BaseFilter filter)
        {
            IQueryable<StudentWalletTransaction> query = _appContext.StudentWalletTransactions
                .Where(m => m.student.IsActive)
                .Select(m => new StudentWalletTransaction
                {
                    Id = m.Id,
                    CreatedDate = m.CreatedDate,
                    StudentId = m.StudentId,
                    Amount = m.Amount,
                    TransactionType = m.TransactionType,
                    Description = m.Description,
                    Remarks = m.Remarks,
                    student = new Student
                    {
                        Name = m.student.Name
                    },
                    WalletPayment = new WalletPayment
                    {
                        fomoid = m.WalletPayment.fomoid
                    },
                    CreatedByUser = new ApplicationUser
                    {
                        UserName = m.CreatedByUser.UserName
                    },
                    Payment = new Payment
                    {
                        PosInvoiceId = m.Payment == null ? string.Empty : m.Payment.PosInvoiceId
                    }
                });

            query = this._sieveProcessor.Apply(filter, query, applyPagination: false);
            var logs = _mapper.Map<List<StudentWalletTransactionDTO>>(await query.ToListAsync());

            if (logs != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Wallet Transactions By Student");
                    var headers = new string[] { "Date", "Amount", "Type", "Description", "Remarks", "Processed By" };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                    var borderedTitleStyle = wb.CreateCellStyle();
                    borderedTitleStyle.SetFont(headerFont);

                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyle.BorderRight = BorderStyle.Thin;
                    var row = sheet.CreateRow(rowCount);
                    ICell cell;

                    cell = row.CreateCell(0);
                    cell.SetCellValue($"Wallet Transaction For {logs.FirstOrDefault()?.StudentName}");
                    cell.CellStyle = borderedTitleStyle;
                    rowCount++;
                    rowCount++;

                    row = sheet.CreateRow(rowCount);
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
                        cell.SetCellValue(dt.TransactionDateTime?.ToString("dd/MM/yyyy hh:mm:ss tt"));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(1);
                        cell.SetCellValue(Common.Round(dt.Amount));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(2);
                        cell.SetCellValue(dt.TransactionType);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(3);
                        cell.SetCellValue(dt.Description);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(4);
                        cell.SetCellValue(dt.Remarks);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(5);
                        cell.SetCellValue(dt.UserName);
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

        public async Task<BaseOperationResponse> WalletTransfer(int studentIdFrom, int studentIdTo, double amount, int userId)
        {
            return await this._uow.StudentWalletTransactions.WalletTransfer(studentIdFrom, studentIdTo, amount, userId);
        }

        public async Task<BaseOperationResponse> UpdateStudentWalletTransaction(StudentWalletTransactionDTO dto)
        {
            return await this._uow.StudentWalletTransactions.UpdateStudentWalletTransaction(_mapper.Map<StudentWalletTransaction>(dto));
        }

        public async Task<PagedEntity<FASMonthlyBillingReportDTO>> GetFASMonthlyBillingReport(FASMonthlyBillingFilter filter)
        {
            return await this._uow.StudentWalletTransactions.GetFASMonthlyBillingReport(filter);
        }

        public async Task<byte[]> GenerateFASMonthlyBillingReport(FASMonthlyBillingFilter filter)
        {
            filter.PageSize = null;
            var datas = await this._uow.StudentWalletTransactions.GetFASMonthlyBillingReport(filter);

            if (datas.PagedData != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Sheet1");
                    var headers = new string[] { "Outlet", "Student ID", "Name", "Class Level", "Class", "FAS Student", "Delivery Date", "Collection Time", "Meal Type", "Meal Name", "QTY", "Dish Price", "Invoice Number", "POS Invoice", "Total Amount Spent", "Student Status" };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                    var borderedTitleStyle = wb.CreateCellStyle();
                    borderedTitleStyle.SetFont(headerFont);

                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyle.BorderRight = BorderStyle.Thin;

                    ICell cell;

                    var outlets = string.Join(",", datas.PagedData.Select(m => m.OutletName).Distinct());
                    if (filter.OutletId == null) outlets = "ALL";
                    var row = sheet.CreateRow(rowCount);
                    cell = row.CreateCell(0);
                    cell.SetCellValue($"Downloaded by : {outlets}");
                    cell.CellStyle = borderedTitleStyle;
                    rowCount++;

                    var classLevels = string.Join(",", datas.PagedData.Select(m => m.ClassLevel).Distinct());
                    if (filter.ClassLevelIds == null) classLevels = "ALL";
                    row = sheet.CreateRow(rowCount);
                    cell = row.CreateCell(0);
                    cell.SetCellValue($"Class Level : {classLevels}");
                    cell.CellStyle = borderedTitleStyle;
                    rowCount++;

                    row = sheet.CreateRow(rowCount);
                    cell = row.CreateCell(0);
                    cell.SetCellValue($"Month : {filter.StartDate:dd/MM/yyyy} - {filter.EndDate:dd/MM/yyyy}");
                    cell.CellStyle = borderedTitleStyle;
                    rowCount++;


                    rowCount++;

                    row = sheet.CreateRow(rowCount);
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
                    datas.PagedData.ToList().ForEach(dt =>
                    {
                        row = sheet.CreateRow(++rowCount);

                        cell = row.CreateCell(0);
                        cell.SetCellValue(dt.OutletName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(1);
                        cell.SetCellValue(dt.StudentID);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(2);
                        cell.SetCellValue(dt.StudentName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(3);
                        cell.SetCellValue(dt.ClassLevel);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(4);
                        cell.SetCellValue(dt.Class);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(5);
                        cell.SetCellValue(dt.FASStudent ? "Y" : "N");
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(6);
                        cell.SetCellValue(dt.DeliveryDate.ToString("dd/MM/yyyy HH:mm:ss"));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(7);
                        cell.SetCellValue(dt.CollectionTime?.ToString("dd/MM/yyyy HH:mm:ss"));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(8);
                        cell.SetCellValue(dt.MealType);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(9);
                        cell.SetCellValue(dt.MealName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(10);
                        cell.SetCellValue(dt.QtyNumber);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(11);
                        cell.SetCellValue(dt.Price);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(12);
                        cell.SetCellValue(dt.InvoiceNumber);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(13);
                        cell.SetCellValue(dt.POSInvoiceNumber);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(14);
                        cell.SetCellValue(Common.Round(dt.Amount));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(15);
                        cell.SetCellValue(dt.StudentStatus);
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

        public async Task<object> GetInvoiceDetail(int id)
        {
            return this._uow.StudentWalletTransactions.GetInvoiceDetail(id);
        }

        public async Task<PagedEntity<DetailedBasicWalletTopUpReport>> GetDetailedBasicWalletTopUpReport(DetailedBasicWalletTopUpFilter filter)
        {
            return await this._uow.StudentWalletTransactions.GetDetailedBasicWalletTopUpReport(filter);
        }

        public async Task<byte[]> GenerateGetDetailedBasicWalletTopUpReport(DetailedBasicWalletTopUpFilter filter)
        {
            filter.PageSize = null;
            var datas = await this._uow.StudentWalletTransactions.GetDetailedBasicWalletTopUpReport(filter);

            if (datas.PagedData != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    var wb = new XSSFWorkbook();
                    var rowCount = 0;
                    var sheet = (XSSFSheet)wb.CreateSheet("Sheet1");
                    var headers = new string[] { "Transaction Date", "Student ID", "Student Name", "Outlet", "Class Level", "Top up Amount", "Source", "REF_ID", "Processed By", "Student Status" };

                    #region Headers

                    var headerStyle = wb.CreateCellStyle();
                    var headerFont = wb.CreateFont();
                    headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    headerStyle.SetFont(headerFont);
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                    var borderedTitleStyle = wb.CreateCellStyle();
                    borderedTitleStyle.SetFont(headerFont);

                    var borderedHeaderStyle = wb.CreateCellStyle();
                    borderedHeaderStyle.SetFont(headerFont);
                    borderedHeaderStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    borderedHeaderStyle.BorderTop = BorderStyle.Thin;
                    borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
                    borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
                    borderedHeaderStyle.BorderRight = BorderStyle.Thin;

                    ICell cell;

                    var row = sheet.CreateRow(rowCount);
                    cell = row.CreateCell(0);
                    cell.SetCellValue($"Report Name : Detailed Basic Wallet Top Up Transaction");
                    cell.CellStyle = borderedTitleStyle;
                    rowCount++;

                    var outlets = string.Join(",", datas.PagedData.Select(m => m.Outlet).Distinct());
                    if (filter.OutletId == null) outlets = "ALL";
                    row = sheet.CreateRow(rowCount);
                    cell = row.CreateCell(0);
                    cell.SetCellValue($"Outlet : {outlets}");
                    cell.CellStyle = borderedTitleStyle;
                    rowCount++;

                    var classLevels = string.Join(",", datas.PagedData.Select(m => m.ClassLevel).Distinct());
                    if (filter.ClassLevelIds == null) classLevels = "ALL";
                    row = sheet.CreateRow(rowCount);
                    cell = row.CreateCell(0);
                    cell.SetCellValue($"Class Level : {classLevels}");
                    cell.CellStyle = borderedTitleStyle;
                    rowCount++;

                    row = sheet.CreateRow(rowCount);
                    cell = row.CreateCell(0);
                    cell.SetCellValue($"Month : {filter.StartDate:dd/MM/yyyy} - {filter.EndDate:dd/MM/yyyy}");
                    cell.CellStyle = borderedTitleStyle;
                    rowCount++;


                    rowCount++;

                    row = sheet.CreateRow(rowCount);
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
                    datas.PagedData.ToList().ForEach(dt =>
                    {
                        row = sheet.CreateRow(++rowCount);

                        cell = row.CreateCell(0);
                        cell.SetCellValue(dt.TransactionDate.ToString("dd/MM/yyyy HH:mm:ss"));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(1);
                        cell.SetCellValue(dt.StudentId);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(2);
                        cell.SetCellValue(dt.StudentName);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(3);
                        cell.SetCellValue(dt.Outlet);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(4);
                        cell.SetCellValue(dt.ClassLevel);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(5);
                        cell.SetCellValue(Common.Round(dt.Amount));
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(6);
                        cell.SetCellValue(dt.Source);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(7);
                        cell.SetCellValue(dt.RefId);
                        cell.CellStyle = contentStyle;

                        cell = row.CreateCell(8);
                        cell.SetCellValue(dt.ProcessedBy);
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
    }
}
