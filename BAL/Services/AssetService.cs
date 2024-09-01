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

namespace BAL.Services
{
    public class AssetService : IAssetService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public AssetService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _mapper = mapper;
        }

        #region Asset Types

        public async Task<PagedEntity<AssetTypeDTO>> GetAssetTypesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<AssetTypeDTO>>(await this._uow.AssetTypes.GetAssetTypesAsync(filter));
            return result;
        }

        public async Task<AssetTypeDTO> GetAssetTypeByIdAsync(int id)
        {
            return _mapper.Map<AssetTypeDTO>(await this._uow.AssetTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateAssetTypeAsync(AssetTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.AssetTypes.CreateAsync(_mapper.Map<AssetType>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateAssetTypeAsync(AssetTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.AssetTypes.UpdateAsync(_mapper.Map<AssetType>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteAssetTypeAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.AssetTypes.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Asset Models

        public async Task<PagedEntity<AssetModelDTO>> GetAssetModelsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<AssetModelDTO>>(await this._uow.AssetModels.GetAssetModelsAsync(filter));
            return result;
        }

        public async Task<AssetModelDTO> GetAssetModelByIdAsync(int id)
        {
            return _mapper.Map<AssetModelDTO>(await this._uow.AssetModels.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateAssetModelAsync(AssetModelDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.AssetModels.CreateAsync(_mapper.Map<AssetModel>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateAssetModelAsync(AssetModelDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.AssetModels.UpdateAsync(_mapper.Map<AssetModel>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteAssetModelAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.AssetModels.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Assets

        public async Task<PagedEntity<AssetDTO>> GetAssetsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<AssetDTO>>(await this._uow.Assets.GetAssetsAsync(filter));
            return result;
        }

        public async Task<AssetDTO> GetAssetByIdAsync(int id)
        {
            return _mapper.Map<AssetDTO>(await this._uow.Assets.GetByIdAsync(id));
        }

        public async Task<AssetDTO> GetAssetByCodeAsync(string serialNumber)
        {
            return _mapper.Map<AssetDTO>(await this._uow.Assets.GetByCodeAsync(serialNumber));
        }

        public async Task<BaseOperationResponse> CreateAssetAsync(AssetDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Assets.CreateAsync(_mapper.Map<Asset>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateAssetAsync(AssetDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Assets.UpdateAsync(_mapper.Map<Asset>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteAssetAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Assets.DeleteAsync(id);
            return result;
        }

        #endregion

        public async Task<BaseOperationResponse> ImportFile(List<List<string>> data)
        {
            var result = new BaseOperationResponse();

            foreach (var row in data.Skip(1).ToList()) // Skip first row
            {
                string serial_number = row[0].ToString().Trim();
                string asset_model = row[1].ToString().Trim();
                string asset_type = row[2].ToString().Trim();
                string purchase_date = row[3].ToString().Trim();
                string warranty_start = row[4].ToString().Trim();
                string warranty_end = row[5].ToString().Trim();
                string location_name = row[6].ToString().Trim();
                string institution_name = row[7].ToString().Trim();
                string po_number = row[8].ToString().Trim();

                int assetModelId = await this._uow.AssetModels.GetOrCreateByCode(new AssetModel()
                {
                    Name = asset_model
                });

                var location = await this._uow.Locations.GetFirstOrDefaultAsync(d => d.Name == location_name && d.IsActive);

                var institution = await this._uow.Institutions.GetFirstOrDefaultAsync(d => d.Name == institution_name && d.IsActive);

                var asset = await GetAssetByCodeAsync(serial_number);

                if (asset == null)
                    asset = new AssetDTO();

                asset.SerialNumber = serial_number;
                if (assetModelId > 0) asset.AssetModelId = assetModelId;
                asset.AssetTypeName = asset_type;
                if(!string.IsNullOrWhiteSpace(purchase_date)) asset.PurchaseDate = DateTime.Parse(purchase_date);
                if (!string.IsNullOrWhiteSpace(warranty_start))  asset.WarrantyStart = DateTime.Parse(warranty_start);
                if (!string.IsNullOrWhiteSpace(warranty_end)) asset.WarrantyEnd = DateTime.Parse(warranty_end);
                asset.LocationId = location?.Id;
                asset.InstitutionId = institution?.Id;
                asset.poNumber = po_number;
                
                
                if (asset.Id > 0) await UpdateAssetAsync(asset);
                else await CreateAssetAsync(asset);
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
                var headers = new List<string>(new string[] { "Serial Number","Asset Model","Asset Type", "Purchase Date","Warranty Start", "Warranty End", "Location", "Institution", "PO Number" });

                var assets = (await GetAssetsAsync(new BaseFilter())).PagedData;

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
                foreach (var t in assets)
                {
                    var tinfo = t;

                        row = sheet.GetRow(rowCount);
                        if (row == null)
                            row = sheet.CreateRow(rowCount);
                        row.CreateCell(0).SetCellValue(tinfo.SerialNumber);
                        row.CreateCell(1).SetCellValue(tinfo.AssetModelName);
                        row.CreateCell(2).SetCellValue(tinfo.AssetTypeName);
                        row.CreateCell(3).SetCellValue(tinfo.PurchaseDate?.ToString("yyyy/MM/dd HH:mm:ss") ?? "");
                        row.CreateCell(4).SetCellValue(tinfo.WarrantyStart?.ToString("yyyy/MM/dd HH:mm:ss") ?? "");
                        row.CreateCell(5).SetCellValue(tinfo.WarrantyEnd?.ToString("yyyy/MM/dd HH:mm:ss") ?? "");
                        row.CreateCell(6).SetCellValue(tinfo.LocationName);
                    row.CreateCell(7).SetCellValue(tinfo.InstitutionName);
                    row.CreateCell(8).SetCellValue(tinfo.poNumber);

                    rowCount++;
                }

                #endregion

                for (int i = 0; i < headers.Count; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                wb.Write(stream);

                return stream.ToArray();
            }
        }
    }
}
