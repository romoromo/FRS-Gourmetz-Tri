using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Models;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OpenIddict.Validation.AspNetCore;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class DirectoryListingController : BaseController
    {
        private IUnitOfWork _unitOfWork;
        readonly ILogger _logger;
        private readonly IMapper _mapper;


        public DirectoryListingController(IUnitOfWork unitOfWork, ILogger<DirectoryListingController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet("list")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<DirectoryListingViewModel>))]
        public async Task<IActionResult> GetDirectoryListings(int? institutionId = null)
        {
            return await GetDirectoryListings(-1, -1, institutionId);
        }

        [HttpGet("listexcludeinternal")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<DirectoryListingViewModel>))]
        public async Task<IActionResult> GetDirectoryListingsExcludeInternal(int? institutionId = null)
        {
            var result = await _unitOfWork.DirectoryListings.GetDirectoryListingsExcludeInternal(institutionId);
            return Ok(_mapper.Map<List<DirectoryListingViewModel>>(result));
        }

        [HttpGet("export")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<DirectoryListingViewModel>))]
        public async Task<IActionResult> GetDirectoryListings2(int? institutionId = null)
        {
            var bts = await _unitOfWork.DirectoryListings.GetTemplate();

            return File(bts,
         "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
         DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_Directory Template.xls");
        }


        [HttpGet("list/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<DirectoryListingViewModel>))]
        public async Task<IActionResult> GetDirectoryListings(int pageNumber, int pageSize, int? institutionId = null)
        {
            var result = await _unitOfWork.DirectoryListings.GetDirectoryListingsLoadRelatedAsync(pageNumber, pageSize, institutionId);
            return Ok(_mapper.Map<List<DirectoryListingViewModel>>(result));
        }

        [HttpPost("")]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(201, Type = typeof(DirectoryListingViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateDirectoryListing([FromBody] DirectoryListingViewModel directoryListing)
        {
            if (ModelState.IsValid)
            {
                if (directoryListing == null)
                    return BadRequest($"{nameof(directoryListing)} cannot be null");


                var type = _mapper.Map<DirectoryListing>(directoryListing);

                var result = await _unitOfWork.DirectoryListings.CreateAsync(type);
                if (result.IsSuccess)
                {
                    DirectoryListingViewModel directoryListingVM = _mapper.Map<DirectoryListingViewModel>(result.Data);
                    return CreatedAtAction("GetDirectoryListingById", new { id = directoryListingVM.Id }, directoryListingVM);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200, Type = typeof(DirectoryListingViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDirectoryListing(int id)
        {
            var directoryListing = await this._unitOfWork.DirectoryListings.GetByIdAsync(id);

            DirectoryListingViewModel directoryListingVM = _mapper.Map<DirectoryListingViewModel>(directoryListing);
            if (directoryListingVM == null)
                return NotFound(id);

            var result = await _unitOfWork.DirectoryListings.DeleteAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting application setting: " + string.Join(", ", result.Message));


            return Ok(directoryListingVM);
        }

        [HttpPut("update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateDirectoryListing(string id, [FromBody] DirectoryListingViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");



                var directoryListing = await this._unitOfWork.DirectoryListings.GetByIdAsync(model.Id);

                DirectoryListingViewModel directoryListingVM = _mapper.Map<DirectoryListingViewModel>(directoryListing);
                if (directoryListingVM == null)
                    return NotFound(id);

                var updatedModel = _mapper.Map<DirectoryListing>(model);
                var result = await _unitOfWork.DirectoryListings.UpdateAsync(updatedModel);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        [HttpPost("import"), DisableRequestSizeLimit]
        //[AllowAnonymous]
        public async Task<IActionResult> Import()
        {

            var file = Request.Form.Files[0];
            var folderName = Path.Combine("Resources", "Excel");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            Directory.CreateDirectory(pathToSave);

            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                fileName = Guid.NewGuid().ToString() + fileName;
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                await SaveImport(file, fullPath);

                return Ok(new { dbPath, fileName });
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpGet("template")]
        //[AllowAnonymous]
        public async Task<IActionResult> GetTemplate()
        {
            var bts = await _unitOfWork.DirectoryListings.GetTemplate();

            return File(bts,
         "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
         DateTime.Now.ToString("ddMMyyyy_hhmmss") + "_Directory Template.xls");

        }

        List<List<string>> xlsParser(String filePath, String sheet = "Sheet1",
            int startColumn = 1, int? endColumn = null, int startRow = 0, int? endRow = null)
        {
            var ret = new List<List<string>>();
            try
            {
                IWorkbook wb = new HSSFWorkbook(new FileStream(filePath, FileMode.Open));
                var sht = (HSSFSheet)wb.GetSheet(sheet);
                int rowCount = endRow ?? sht.PhysicalNumberOfRows;
                int colCount = sht.GetRow(startRow).PhysicalNumberOfCells;
                for (int i = 0; i < rowCount; i++)
                {
                    var row = new List<string>();
                    for (int j = (startColumn - 1); j < (endColumn ?? colCount); j++)
                    {
                        var cellValue = sht.GetRow(i).GetCell(j);
                        row.Add(cellValue != null ? cellValue.ToString() : "");
                    }
                    ret.Add(row);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ret;
        }


        async Task<BaseOperationResponse> SaveImport(IFormFile file, string path)
        {
            var headers = new List<string>(new string[] {
                "Directory Code",   //0
                "Category ID",      //1
                "Category Label",   //2
                "Display Name",     //3
                "Disciplines",      //4
                "Aliases",          //5
                "Level",            //6
                "Unit Number",      //7
                "Info",             //8
                "Opening Hours",    //9
                "Contact",          //10
                "Building ID",      //11
                "Building Code",    //12
                "Cluster ID",       //13
                "Cluster Code",     //14
                "Floor Code",       //15
                "Floor Label"       //16
            });

            var data = xlsParser(path, "Sheet1", 1, headers.Count);
            if (data.Count > 0)
            {
                var model = new ModelImport()
                {
                    name = file.FileName,
                };


                return await _unitOfWork.DirectoryListings.ImportFile(data);

            }

            var result = new BaseOperationResponse();
            result.Message = "Failed to save!";
            result.IsSuccess = false;
            return result;
        }
    }

    internal class ModelImport
    {
        public ModelImport()
        {
        }

        public object name { get; set; }
        public object size { get; set; }
    }
}