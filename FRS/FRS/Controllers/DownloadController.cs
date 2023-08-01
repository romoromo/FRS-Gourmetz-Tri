using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FRS.Controllers
{
    public class DownloadController : Controller
    {
        private readonly IConfiguration _configuration;
        private IUnitOfWork _unitOfWork;
        public DownloadController(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string filePath)
        {
            var baseDownloadPath = _configuration["AppSettings:downloadPath"];
            string absoluteFilePath = Path.Combine(baseDownloadPath, filePath);

            return PhysicalFile(absoluteFilePath, MediaTypeNames.Application.Octet, Path.GetFileName(absoluteFilePath));
        }

        public IActionResult FileByPath(string filePath, string fileType)
        {
            filePath = string.Concat(Directory.GetCurrentDirectory(), filePath);

            return PhysicalFile(filePath, string.IsNullOrEmpty(fileType) ? MediaTypeNames.Application.Octet : fileType, Path.GetFileName(filePath));
        }

        public async Task<IActionResult> GenerateAttendanceXls(int reservationId)
        {
            var xls = await _unitOfWork.Reservations.GenerateAttendanceXlsLayout(reservationId);
            var reportName = DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_AttendanceList.xlsx";
            //var filepath = Path.Combine(Utilities.GetReportPathXls(), reportName);
            //System.IO.File.WriteAllBytes(filepath, xls);
            //var returnPath = Utilities.GetRelativeReportPathPdf(reportName);

            if (xls == null || xls.Length == 0)
            {
                return BadRequest("");
            }

            return File(
                fileContents: xls,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: reportName
            );
        }
    }
}