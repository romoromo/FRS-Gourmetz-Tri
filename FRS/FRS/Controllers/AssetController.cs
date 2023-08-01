using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using BAL.DTO;
using BAL.Services.Interfaces;
using DAL;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using FRS.Attributes;
using FRS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OpenIddict.Validation;

namespace FRS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class AssetController : BaseController
    {
        private IAssetService _service;
        readonly ILogger _logger;


        public AssetController(IAssetService service, ILogger<AssetController> logger)
        {
            _service = service;
            _logger = logger;
        }

        #region Asset Types

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("assettypes/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllAssetTypesPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAssetTypes(BaseFilter filter)
        {
            var results = await this._service.GetAssetTypesAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<AssetTypeDTO>>(results));
        }

        #endregion

        [HttpPost("assettypes")]
        //[Authorize(Authorization.Policies.ManageAllAssetTypesPolicy)]
        [ProducesResponseType(201, Type = typeof(AssetTypeDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAssetType([FromBody] AssetTypeDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateAssetTypeAsync(dto);
                if (result.IsSuccess)
                {
                    AssetTypeDTO vm = Mapper.Map<AssetTypeDTO>(result.Data);
                    return CreatedAtAction("GetAssetTypeById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("assettypes/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllAssetTypesPolicy)]
        [ProducesResponseType(200, Type = typeof(AssetTypeDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAssetType(int id)
        {
            var dto = await this._service.GetAssetTypeByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteAssetTypeAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("assettypes/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllAssetTypesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateAssetType(string id, [FromBody] AssetTypeDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetAssetTypeByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateAssetTypeAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Asset Models

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("assetmodels/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllAssetModelsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAssetModels(BaseFilter filter)
        {
            var results = await this._service.GetAssetModelsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<AssetModelDTO>>(results));
        }

        #endregion

        [HttpPost("assetmodels")]
        //[Authorize(Authorization.Policies.ManageAllAssetModelsPolicy)]
        [ProducesResponseType(201, Type = typeof(AssetModelDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAssetModel([FromBody] AssetModelDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateAssetModelAsync(dto);
                if (result.IsSuccess)
                {
                    AssetModelDTO vm = Mapper.Map<AssetModelDTO>(result.Data);
                    return CreatedAtAction("GetAssetModelById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("assetmodels/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllAssetModelsPolicy)]
        [ProducesResponseType(200, Type = typeof(AssetModelDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAssetModel(int id)
        {
            var dto = await this._service.GetAssetModelByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteAssetModelAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("assetmodels/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllAssetModelsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateAssetModel(string id, [FromBody] AssetModelDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetAssetModelByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateAssetModelAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        #region Assets

        #region Sieved
        [ApiKeyAuthorize]
        [HttpGet("assets/sieve/list")]
        //[Authorize(Authorization.Policies.ViewAllAssetsPolicy)]
        //[AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PagedEntityViewModel<>))]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAssets(BaseFilter filter)
        {
            var results = await this._service.GetAssetsAsync(filter);
            return Ok(Mapper.Map<PagedEntityViewModel<AssetDTO>>(results));
        }

        #endregion

        [HttpPost("assets")]
        //[Authorize(Authorization.Policies.ManageAllAssetsPolicy)]
        [ProducesResponseType(201, Type = typeof(AssetDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAsset([FromBody] AssetDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto == null)
                    return BadRequest($"{nameof(dto)} cannot be null");


                var result = await this._service.CreateAssetAsync(dto);
                if (result.IsSuccess)
                {
                    AssetDTO vm = Mapper.Map<AssetDTO>(result.Data);
                    return CreatedAtAction("GetAssetById", new { id = vm.Id }, vm);
                }

                AddErrors(new string[] { result.Message });
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("assets/delete/{id}")]
        //[Authorize(Authorization.Policies.ManageAllAssetsPolicy)]
        [ProducesResponseType(200, Type = typeof(AssetDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            var dto = await this._service.GetAssetByIdAsync(id);
            if (dto == null)
                return NotFound(id);

            var result = await this._service.DeleteAssetAsync(id);
            if (!result.IsSuccess)
                throw new Exception("The following errors occurred while deleting: " + string.Join(", ", result.Message));

            return Ok(dto);
        }

        [HttpPut("assets/update/{id}")]
        //[Authorize(Authorization.Policies.ManageAllAssetsPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateAsset(string id, [FromBody] AssetDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model == null)
                    return BadRequest($"{nameof(model)} cannot be null");

                if (model.Id == 0)
                    return BadRequest("Conflicting type id in parameter and model data");


                var dto = await this._service.GetAssetByIdAsync(model.Id);

                if (dto == null)
                    return NotFound(id);

                var result = await this._service.UpdateAssetAsync(model);
                if (result.IsSuccess)
                    return NoContent();

                AddErrors(new string[] { result.Message });

            }

            return BadRequest(ModelState);
        }

        #endregion

        [HttpGet("assets/export")]
        //[AllowAnonymous]
        //[Authorize(Authorization.Policies.ManageAllDirectoryListingsPolicy)]
        [ProducesResponseType(200, Type = typeof(List<DirectoryListingViewModel>))]
        public async Task<IActionResult> GetDirectoryListings2(int? institutionId = null)
        {
            var bts = await _service.GetTemplate();

            return File(bts,
         "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
         DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_Asset Template.xls");
        }

        [HttpPost("assets/import"), DisableRequestSizeLimit]
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

        async Task<BaseOperationResponse> SaveImport(IFormFile file, string path)
        {
            var headers = new List<string>(new string[] {
                "Serial Number",    //0
                "Asset Model",      //1
                "Asset Type",       //2
                "Purchase Date",    //3
                "Warranty Start",   //4
                "Warranty End",      //5
                "Location",      //6
                "Institution",      //7
                "PO Number",      //8
            });

            var data = xlsParser(path, "Sheet1", 1, headers.Count);
            if (data.Count > 0)
            {
                var model = new ModelImport()
                {
                    name = file.FileName,
                };


                return await _service.ImportFile(data);

            }

            var result = new BaseOperationResponse();
            result.Message = "Failed to save!";
            result.IsSuccess = false;
            return result;
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
    }
}