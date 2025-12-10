using AutoMapper;
using BAL.DTO;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces;
using BAL.Services.Interfaces.MealOrder;
using BAL.Utilities;
using DAL;
using DAL.Core;
using DAL.Core.Helpers;
using DAL.Core.Interfaces;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using DAL.Repositories.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services.MealOrder
{
    public class DeliveryService : IDeliveryService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private IAccountManager _accountManager;
        private ApplicationDbContext _appContext;
        private readonly IMapper _mapper;

        public DeliveryService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IAccountManager accountManager, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _accountManager = accountManager;
            _mapper = mapper;
        }

        #region CatererInfo

        public async Task<PagedEntity<CatererInfoSimpleDTO>> GetCatererInfosSimpleAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<CatererInfoSimpleDTO>>(await this._uow.CatererInfos.GetCatererInfosAsync(filter));
            return result;
        }

        public async Task<PagedEntity<CatererInfoDTO>> GetCatererInfosAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<CatererInfoDTO>>(await this._uow.CatererInfos.GetCatererInfosAsync(filter));
            return result;
        }

        public async Task<CatererInfoDTO> GetCatererInfoByIdAsync(int id)
        {
            return _mapper.Map<CatererInfoDTO>(await this._uow.CatererInfos.GetByIdAsync(id));
        }

        public async Task<CatererInfoSimpleDTO> GetCatererInfoSimpleByIdAsync(int id)
        {
            return _mapper.Map<CatererInfoSimpleDTO>(await this._uow.CatererInfos.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateCatererInfoAsync(CatererInfoSimpleDTO dto)
        {
            return await this._uow.CatererInfos.CreateAsync(_mapper.Map<CatererInfo>(dto));
        }

        public async Task<BaseOperationResponse> UpdateCatererInfoAsync(CatererInfoSimpleDTO dto)
        {
            return await this._uow.CatererInfos.UpdateAsync(_mapper.Map<CatererInfo>(dto));
        }

        public async Task<BaseOperationResponse> DeleteCatererInfoAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.CatererInfos.DeleteAsync(id);
            return result;
        }

        public async Task<BaseOperationResponse> RequestOutlet(int catererId, int outletId, string status, bool isRsp, int? outletProfileId)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.CatererInfos.RequestOutlet(catererId, outletId, status, isRsp, outletProfileId);
            return result;
        }

        #endregion

        #region BentoBoxType

        public async Task<PagedEntity<BentoBoxTypeDTO>> GetBentoBoxTypesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<BentoBoxTypeDTO>>(await this._uow.BentoBoxTypes.GetBentoBoxTypesAsync(filter));
            return result;
        }

        public async Task<BentoBoxTypeDTO> GetBentoBoxTypeByIdAsync(int id)
        {
            return _mapper.Map<BentoBoxTypeDTO>(await this._uow.BentoBoxTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateBentoBoxTypeAsync(BentoBoxTypeDTO dto)
        {
            return await this._uow.BentoBoxTypes.CreateAsync(_mapper.Map<BentoBoxType>(dto));
        }

        public async Task<BaseOperationResponse> UpdateBentoBoxTypeAsync(BentoBoxTypeDetailsDTO dto)
        {
            return await this._uow.BentoBoxTypes.UpdateAsync(_mapper.Map<BentoBoxType>(dto));
        }

        public async Task<BaseOperationResponse> DeleteBentoBoxTypeAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.BentoBoxTypes.DeleteAsync(id);
            return result;
        }

        #endregion


        #region CartonType

        public async Task<PagedEntity<CartonTypeDTO>> GetCartonTypesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<CartonTypeDTO>>(await this._uow.CartonTypes.GetCartonTypesAsync(filter));
            return result;
        }

        public async Task<CartonTypeDTO> GetCartonTypeByIdAsync(int id)
        {
            return _mapper.Map<CartonTypeDTO>(await this._uow.CartonTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateCartonTypeAsync(CartonTypeDTO dto)
        {
            return await this._uow.CartonTypes.CreateAsync(_mapper.Map<CartonType>(dto));
        }

        public async Task<BaseOperationResponse> UpdateCartonTypeAsync(CartonTypeDetailsDTO dto)
        {
            return await this._uow.CartonTypes.UpdateAsync(_mapper.Map<CartonType>(dto));
        }

        public async Task<BaseOperationResponse> DeleteCartonTypeAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.CartonTypes.DeleteAsync(id);
            return result;
        }

        #endregion

        #region DeliveryOrder

        public async Task<PagedEntity<DeliveryOrderDTO>> GetDeliveryOrdersAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<DeliveryOrderDTO>>(await this._uow.DeliveryOrders.GetDeliveryOrdersAsync(filter));
            return result;
        }

        public async Task<DeliveryOrderDTO> GetDeliveryOrderByIdAsync(int id)
        {
            return _mapper.Map<DeliveryOrderDTO>(await this._uow.DeliveryOrders.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateDeliveryOrderAsync(DeliveryOrderDTO dto)
        {
            return await this._uow.DeliveryOrders.CreateAsync(_mapper.Map<DeliveryOrder>(dto));
        }

        public async Task<BaseOperationResponse> UpdateDeliveryOrderAsync(DeliveryOrderDTO dto)
        {
            return await this._uow.DeliveryOrders.UpdateAsync(_mapper.Map<DeliveryOrder>(dto));
        }

        public async Task<BaseOperationResponse> DeleteDeliveryOrderAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.DeliveryOrders.DeleteAsync(id);
            return result;
        }

        #endregion

        #region DeliveryOrderNew

        public async Task<PagedEntity<DeliveryOrderNewDTO>> GetDeliveryOrderNewsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<DeliveryOrderNewDTO>>(await this._uow.DeliveryOrderNews.GetDeliveryOrdersAsync(filter));
            return result;
        }

        public async Task<DeliveryOrderNewDTO> GetDeliveryOrderNewByIdAsync(int id)
        {
            return _mapper.Map<DeliveryOrderNewDTO>(await this._uow.DeliveryOrderNews.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateDeliveryOrderNewAsync(DeliveryOrderNewDTO dto)
        {
            return await this._uow.DeliveryOrderNews.CreateAsync(_mapper.Map<DeliveryOrderNew>(dto));
        }

        public async Task<BaseOperationResponse> UpdateDeliveryOrderNewAsync(DeliveryOrderNewDTO dto)
        {
            return await this._uow.DeliveryOrderNews.UpdateAsync(_mapper.Map<DeliveryOrderNew>(dto));
        }

        public async Task<BaseOperationResponse> LoadDeliveryOrderNewAsync(DeliveryOrderNewDTO dto)
        {
            return await this._uow.DeliveryOrderNews.LoadAsync(_mapper.Map<DeliveryOrderNew>(dto));
        }

        public async Task<BaseOperationResponse> DeleteDeliveryOrderNewAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.DeliveryOrderNews.DeleteAsync(id);
            return result;
        }

        #endregion


        #region Bento count
        public async Task<PagedEntity<BentoUsageCountDTO>> GetBentoUsage(BaseFilter filter)
        {
            var insideFilter = new BaseFilter();
            var results = await GetDeliveryOrderNewsAsync(insideFilter);
            var usages = new List<BentoUsageCountDTO>();
            foreach (var dOrder in results.PagedData)
            {
                foreach (var doDetail in dOrder.DeliveryDetails)
                {
                    foreach (var doBento in doDetail.DeliveryBentos)
                    {
                        if (doBento.BentoAssetId == null) continue;
                        var bentoIndex = usages.FindIndex(x => x.BentoId == doBento.BentoAssetId);
                        if (bentoIndex < 0)
                        {
                            var bentoUse = new BentoUsageCountDTO();
                            bentoUse.BentoId = doBento.BentoAssetId;
                            bentoUse.BentoCode = doBento.BentoAssetCode;
                            bentoUse.UsageCount = 1;
                            usages.Add(bentoUse);

                        }
                        else
                        {
                            usages[bentoIndex].UsageCount += 1;
                        }
                    }
                }
            }

            if (filter.Sorts == "usageCount")
            {
                usages = usages.OrderBy(o => o.UsageCount).ToList();
            }
            else if (filter.Sorts == "-usageCount")
            {
                usages = usages.OrderByDescending(o => o.UsageCount).ToList();
            }

            int startPage = ((filter.Page ?? 1) - 1) * (filter.PageSize ?? 10);
            int endPage = ((filter.Page ?? 1) * (filter.PageSize ?? 10)) - 1;
            if (endPage >= usages.Count())
            {
                endPage = (usages.Count()) % (filter.PageSize ?? 10);
            }
            else
            {
                endPage = (filter.PageSize ?? 10);
            }

            var pagedUsage = usages.GetRange(startPage, endPage);

            int total = usages.Count();
            var result = new PagedEntity<BentoUsageCountDTO>();
            result.Filter = filter;
            result.PagedData = pagedUsage;
            result.CurrentPage = filter.Page ?? 1;
            result.PageSize = filter.PageSize ?? 10;
            result.PageCount = total / result.PageSize;
            result.TotalCount = total;

            return result;
        }

        #endregion

        #region StoreInventory

        public async Task<PagedEntity<StoreInventoryDTO>> GetStoreInventoriesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<StoreInventoryDTO>>(await this._uow.StoreInventories.GetStoreInventoriesAsync(filter));
            return result;
        }

        public async Task<StoreInventoryDTO> GetStoreInventoryByIdAsync(int id)
        {
            return _mapper.Map<StoreInventoryDTO>(await this._uow.StoreInventories.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateStoreInventoryAsync(StoreInventoryDTO dto)
        {
            return await this._uow.StoreInventories.CreateAsync(_mapper.Map<StoreInventory>(dto));
        }

        public async Task<BaseOperationResponse> UpdateStoreInventoryAsync(StoreInventoryDTO dto)
        {
            return await this._uow.StoreInventories.UpdateAsync(_mapper.Map<StoreInventory>(dto));
        }

        public async Task<BaseOperationResponse> DeleteStoreInventoryAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.StoreInventories.DeleteAsync(id);
            return result;
        }

        public async Task<PagedEntity<StoreInventoryDetailDTO>> GetStoreInventoryDetailsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<StoreInventoryDetailDTO>>(await this._uow.StoreInventories.GetStoreInventoryDetailsAsync(filter));
            return result;
        }

        #endregion



        #region TrackingStatus

        public async Task<PagedEntity<TrackingStatusDTO>> GetTrackingStatussAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<TrackingStatusDTO>>(await this._uow.TrackingStatuss.GetTrackingStatussAsync(filter));
            return result;
        }

        public async Task<TrackingStatusDTO> GetTrackingStatusByIdAsync(int id)
        {
            return _mapper.Map<TrackingStatusDTO>(await this._uow.TrackingStatuss.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateTrackingStatusAsync(TrackingStatusDTO dto)
        {
            return await this._uow.TrackingStatuss.CreateAsync(_mapper.Map<TrackingStatus>(dto));
        }

        public async Task<BaseOperationResponse> UpdateTrackingStatusAsync(TrackingStatusDTO dto)
        {
            return await this._uow.TrackingStatuss.UpdateAsync(_mapper.Map<TrackingStatus>(dto));
        }

        public async Task<BaseOperationResponse> DeleteTrackingStatusAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.TrackingStatuss.DeleteAsync(id);
            return result;
        }

        #endregion

        #region StoreInfo

        public async Task<PagedEntity<StoreInfoDTO>> GetStoreInfosAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<StoreInfoDTO>>(await this._uow.StoreInfos.GetStoreInfosAsync(filter));
            return result;
        }

        public async Task<StoreInfoDTO> GetStoreInfoByIdAsync(int id)
        {
            return _mapper.Map<StoreInfoDTO>(await this._uow.StoreInfos.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateStoreInfoAsync(StoreInfoDTO dto)
        {
            return await this._uow.StoreInfos.CreateAsync(_mapper.Map<StoreInfo>(dto));
        }

        public async Task<BaseOperationResponse> UpdateStoreInfoAsync(StoreInfoDTO dto)
        {
            return await this._uow.StoreInfos.UpdateAsync(_mapper.Map<StoreInfo>(dto));
        }

        public async Task<BaseOperationResponse> DeleteStoreInfoAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.StoreInfos.DeleteAsync(id);
            return result;
        }

        #endregion


        #region BentoAsset

        public async Task<PagedEntity<BentoAssetDTO>> GetBentoAssetsAsync(BentoAssetsFilter filter)
        {
            var result = _mapper.Map<PagedEntity<BentoAssetDTO>>(await this._uow.BentoAssets.GetBentoAssetsAsync(filter));
            return result;
        }

        public async Task<PagedEntity<BentoAssetDTO>> GetBentoAssetsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<BentoAssetDTO>>(await this._uow.BentoAssets.GetBentoAssetsAsync(filter));
            return result;
        }

        public async Task<BentoAssetDTO> GetBentoAssetByIdAsync(int id)
        {
            return _mapper.Map<BentoAssetDTO>(await this._uow.BentoAssets.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateBentoAssetAsync(BentoAssetDTO dto)
        {
            return await this._uow.BentoAssets.CreateAsync(_mapper.Map<BentoAsset>(dto));
        }

        public async Task<BaseOperationResponse> UpdateBentoAssetAsync(BentoAssetDTO dto)
        {
            return await this._uow.BentoAssets.UpdateAsync(_mapper.Map<BentoAsset>(dto));
        }

        public async Task<BaseOperationResponse> DeleteBentoAssetAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.BentoAssets.DeleteAsync(id);
            return result;
        }

        public async Task<BaseOperationResponse> ResetBentoAssetAsync()
        {
            var result = new BaseOperationResponse();
            result = await this._uow.BentoAssets.ResetAsync();
            return result;
        }

        #endregion

        #region CartonAsset

        public async Task<PagedEntity<CartonAssetDTO>> GetCartonAssetsAsync(CartonAssetsFilter filter)
        {
            var result = _mapper.Map<PagedEntity<CartonAssetDTO>>(await this._uow.CartonAssets.GetCartonAssetsAsync(filter));
            return result;
        }

        public async Task<CartonAssetDTO> GetCartonAssetByIdAsync(int id)
        {
            return _mapper.Map<CartonAssetDTO>(await this._uow.CartonAssets.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateCartonAssetAsync(CartonAssetDTO dto)
        {
            return await this._uow.CartonAssets.CreateAsync(_mapper.Map<CartonAsset>(dto));
        }

        public async Task<BaseOperationResponse> UpdateCartonAssetAsync(CartonAssetDTO dto)
        {
            return await this._uow.CartonAssets.UpdateAsync(_mapper.Map<CartonAsset>(dto));
        }

        public async Task<BaseOperationResponse> DeleteCartonAssetAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.CartonAssets.DeleteAsync(id);
            return result;
        }

        public async Task<BaseOperationResponse> ResetCartonAssetAsync()
        {
            var result = new BaseOperationResponse();
            result = await this._uow.CartonAssets.ResetAsync();
            return result;
        }

        public async Task<byte[]> GenerateCartonAssetLabel(int id)
        {
            var asset = await this._uow.CartonAssets.GetByIdAsync(id);
            var tagId = asset?.Code ?? string.Empty;

            var catererName = "Gourmetz Catering Pte Lt";
            var catererAddress = "";

            if (asset.CartonType != null && asset.CartonType.CatererInfo != null)
            {
                var catererInfo = await this._uow.CatererInfos.GetByIdAsync(asset.CartonType.CatererInfoId.Value);
                if (catererInfo != null)
                {
                    catererName = catererInfo.Name.Length > 24 ? catererInfo.Name.Substring(0, 24) : catererInfo.Name;
                    var addresses = catererInfo.Address.Split('\n');

                    catererAddress = string.Join("\n", addresses.Select(e => e.Length > 28 ? e.Substring(0, 28) : e));
                }
            }

            using (var stream = new System.IO.MemoryStream())
            {
                var folderName = Path.Combine("Resources", "Font");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var imageFolderName = Path.Combine("Resources", "Images", "Default");
                var logoImagePath = Path.Combine(Directory.GetCurrentDirectory(), imageFolderName);

                var fullPath = Path.Combine(pathToSave, "Aller_Bd.ttf");
                var fullImagePath = Path.Combine(logoImagePath, "company_logo.png");

                BaseFont allerfont = BaseFont.CreateFont(fullPath, BaseFont.WINANSI, BaseFont.EMBEDDED);
                Font aller = new Font(allerfont, 12);

                var pgSize = new iTextSharp.text.Rectangle(227f, 114f); // Page size
                Document document = new Document(pgSize, 5, 5, 5, 5);
                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                // QR Code on the left half
                BarcodeQRCode qrCode = new BarcodeQRCode(tagId, 105, 105, null);
                iTextSharp.text.Image qrImage = qrCode.GetImage();
                //qrImage.ScaleToFit(80f, 80f);
                qrImage.SetAbsolutePosition(0f, 12f);  // Positioned on the left side
                document.Add(qrImage);

                // Caterer name and address on the right half
                Paragraph para1 = new Paragraph(catererName, new Font(allerfont, 10));
                para1.Alignment = Element.ALIGN_RIGHT;
                para1.IndentationRight = 5f; // Align to the right side
                para1.PaddingTop = 0f;
                document.Add(para1);

                Image png = Image.GetInstance(fullImagePath);
                png.ScaleToFit(25f, 25f);
                png.SetAbsolutePosition(100f, 65f);
                document.Add(png);

                Paragraph para2 = new Paragraph(catererAddress, new Font(allerfont, 6));
                para2.Alignment = Element.ALIGN_RIGHT;
                para2.IndentationRight = 5f; // Align to the right side
                document.Add(para2);

                // Add the rectangle around the tagId substring (para3) and make the text red
                string tagIdSubstring = tagId.Substring(12, 4); // Extract substring
                PdfContentByte cb = writer.DirectContent;

                // Set the position and dimensions for the rectangle
                float rectX = 105f;
                float rectY = 30f;
                float rectWidth = 80f;
                float rectHeight = 30f;

                // Draw rectangle
                cb.Rectangle(rectX, rectY, rectWidth, rectHeight);
                cb.Stroke();

                // Add the text inside the rectangle in red
                Font redFont = new Font(allerfont, 26, Font.NORMAL, BaseColor.RED);
                ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, new Phrase(tagIdSubstring, redFont), rectX + rectWidth / 2, rectY + rectHeight / 4, 0);

                // Add the full tagId at the bottom, center-aligned across the entire page
                var phrase = new Phrase();
                phrase.Add(new Chunk(tagId, new Font(allerfont, 10, Font.NORMAL)));

                //Paragraph para7 = new Paragraph(phrase);

                //float paragraphHeight = para7.GetCalculatedHeight();

                //// Set the bottom position
                //float yPosition = document.PageSize.Bottom + 10; // Adjust this to set a margin from the bottom

                //// Calculate the center position
                //float xPosition = (document.PageSize.Width - para7.GetCalculatedWidth()) / 2;

                // Add the paragraph to the document
                ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, new Phrase(phrase), 115f, 10f, 0);

                //para7.Alignment = Element.ALIGN_CENTER;
                //para7.SpacingBefore = 35f; // Spacing before tagId
                //para7.SetLeading(0, 2); // Adjust line spacing
                //para7.IndentationLeft = 15f; // Ensure it spans across the page
                //document.Add(para7);

                document.Close();
                writer.Close();

                return stream.ToArray();
            }
        }


        #endregion

        #region DisposableBox

        public async Task<PagedEntity<CartonDisposableBoxDTO>> GetDisposableBoxesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<CartonDisposableBoxDTO>>(await this._uow.CartonDisposableBoxes.GetDisposableBoxesAsync(filter));
            return result;
        }

        public async Task<CartonDisposableBoxDTO> GetDisposableBoxByIdAsync(int id)
        {
            return _mapper.Map<CartonDisposableBoxDTO>(await this._uow.CartonDisposableBoxes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateDisposableBoxAsync(CartonDisposableBoxDTO dto)
        {
            return await this._uow.CartonDisposableBoxes.CreateAsync(_mapper.Map<CartonDisposableBox>(dto));
        }

        public async Task<BaseOperationResponse> UpdateDisposableAsync(CartonDisposableBoxDTO dto)
        {
            return await this._uow.CartonDisposableBoxes.UpdateAsync(_mapper.Map<CartonDisposableBox>(dto));
        }

        public async Task<BaseOperationResponse> DeleteDisposableBoxAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.CartonDisposableBoxes.DeleteAsync(id);
            return result;
        }

        public async Task<BaseOperationResponse> ResetDisposableBoxAsync()
        {
            var result = new BaseOperationResponse();
            result = await this._uow.CartonDisposableBoxes.ResetAsync();
            return result;
        }

        public async Task<BaseOperationResponse> ResetDishDisposableBoxAsync(CartonDisposableBoxDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.CartonDisposableBoxes.ResetDishAsync(_mapper.Map<CartonDisposableBox>(dto));
            return result;
        }

        #endregion

        #region Outlet

        public async Task<PagedEntity<OutletSimpleDTO>> GetOutletsSimpleAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<OutletSimpleDTO>>(await this._uow.Outlets.GetOutletsAsync(filter));
            return result;
        }

        public async Task<PagedEntity<OutletDTO>> GetOutletsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<OutletDTO>>(await this._uow.Outlets.GetOutletsAsync(filter));
            return result;
        }

        public async Task<OutletSimpleDTO> GetOutletByIdSimpleAsync(int id)
        {
            return _mapper.Map<OutletSimpleDTO>(await this._uow.Outlets.GetByIdAsync(id));
        }


        public async Task<OutletDTO> GetOutletByIdAsync(int id)
        {
            return _mapper.Map<OutletDTO>(await this._uow.Outlets.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateOutletAsync(OutletSimpleDTO dto)
        {
            return await this._uow.Outlets.CreateAsync(_mapper.Map<Outlet>(dto));
        }

        public async Task<BaseOperationResponse> UpdateOutletAsync(OutletSimpleDTO dto)
        {
            return await this._uow.Outlets.UpdateAsync(_mapper.Map<Outlet>(dto));
        }

        public async Task<BaseOperationResponse> DeleteOutletAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Outlets.DeleteAsync(id);
            return result;
        }

        public async Task<BaseOperationResponse> UpdateStoresAsync(OutletDTO dto)
        {
            var outlet = _mapper.Map<Outlet>(dto);
            var stores = _mapper.Map<List<StoreInfo>>(dto.Stores);
            return await this._uow.Outlets.UpdateStoresAsync(outlet, stores);
        }

        #endregion

        #region OutletProfile

        public async Task<PagedEntity<OutletProfileDTO>> GetOutletProfilesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<OutletProfileDTO>>(await this._uow.OutletProfiles.GetOutletProfilesAsync(filter));
            return result;
        }

        public async Task<OutletProfileDTO> GetOutletProfileByIdAsync(int id)
        {
            return _mapper.Map<OutletProfileDTO>(await this._uow.OutletProfiles.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateOutletProfileAsync(OutletProfileDTO dto)
        {
            return await this._uow.OutletProfiles.CreateAsync(_mapper.Map<OutletProfile>(dto));
        }

        public async Task<BaseOperationResponse> UpdateOutletProfileAsync(OutletProfileDTO dto)
        {
            return await this._uow.OutletProfiles.UpdateAsync(_mapper.Map<OutletProfile>(dto));
        }

        public async Task<BaseOperationResponse> DeleteOutletProfileAsync(int id)
        {
            var result = new BaseOperationResponse();

            result = await this._uow.OutletProfiles.DeleteAsync(id);
            return result;
        }

        public async Task<bool> TestDeleteOutletProfileAsync(int id)
        {
            return await this._uow.OutletProfiles.TestDeleteAsync(id);
        }
        #endregion

        #region Driver

        public async Task<PagedEntity<DriverDTO>> GetDriversAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<DriverDTO>>(await this._uow.Drivers.GetDriversAsync(filter));
            return result;
        }

        public async Task<DriverDTO> GetDriverByIdAsync(int id)
        {
            return _mapper.Map<DriverDTO>(await this._uow.Drivers.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateDriverAsync(DriverDTO dto)
        {
            return await this._uow.Drivers.CreateAsync(_mapper.Map<Driver>(dto));
        }

        public async Task<BaseOperationResponse> UpdateDriverAsync(DriverDTO dto)
        {
            return await this._uow.Drivers.UpdateAsync(_mapper.Map<Driver>(dto));
        }

        public async Task<BaseOperationResponse> DeleteDriverAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Drivers.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Route

        public async Task<PagedEntity<RouteDTO>> GetRoutesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<RouteDTO>>(await this._uow.Routes.GetRoutesAsync(filter));
            return result;
        }

        public async Task<RouteDTO> GetRouteByIdAsync(int id)
        {
            return _mapper.Map<RouteDTO>(await this._uow.Routes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateRouteAsync(RouteDTO dto)
        {
            return await this._uow.Routes.CreateAsync(_mapper.Map<Route>(dto));
        }

        public async Task<BaseOperationResponse> UpdateRouteAsync(RouteDTO dto)
        {
            var route = _mapper.Map<Route>(dto);
            var routeNodes = _mapper.Map<List<RouteNode>>(dto.Nodes);
            return await this._uow.Routes.UpdateAsync(route, routeNodes);
        }

        public async Task<BaseOperationResponse> DeleteRouteAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Routes.DeleteAsync(id);
            return result;
        }

        #endregion

        #region printDo

        public PdfPCell getCellBold(String text, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            //cell.Padding = 0 ;
            cell.HorizontalAlignment = alignment;
            //cell.Border = PdfPCell.NO_BORDER;
            return cell;
        }

        public PdfPCell getCellBoldNoBord(String text, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            //cell.Padding = 0 ;
            cell.HorizontalAlignment = alignment;
            cell.Border = Rectangle.NO_BORDER;
            return cell;
        }

        public PdfPCell getCell(String text, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 10)));
            //cell.Padding = 0 ;
            cell.HorizontalAlignment = alignment;
            //cell.Border = PdfPCell.NO_BORDER;
            return cell;
        }

        public PdfPCell getCellNoBord(String text, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 10)));
            //cell.Padding = 0 ;
            cell.HorizontalAlignment = alignment;
            cell.Border = Rectangle.NO_BORDER;
            return cell;
        }

        public void pageNumber(PdfWriter writer, Document document, int totalPage)
        {
            PdfContentByte cb = writer.DirectContent;
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            String text = "Page " + writer.PageNumber + " of " + totalPage;

            cb.BeginText();
            cb.SetFontAndSize(bf, 10);
            cb.SetTextMatrix(document.PageSize.GetRight(60), document.PageSize.GetBottom(30));
            cb.ShowText(text);
            cb.EndText();
        }


        public void printHeader(DeliveryOrderNewDTO dOrder, Document document)
        {
            var imageFolderName = Path.Combine("Resources", "Images");
            var pathToImageSave = Path.Combine(Directory.GetCurrentDirectory(), imageFolderName);

            var fullImagePath = Path.Combine(pathToImageSave, "SFS_DO.jpg");

            Image png = Image.GetInstance(fullImagePath);
            png.ScaleToFit(40f, 40f);

            PdfPTable table2 = new PdfPTable(4);
            float[] widths = new float[] { 3.4f, 7f, 2.2f, 3.4f };
            table2.SetWidths(widths);
            table2.WidthPercentage = 100f;
            table2.DefaultCell.Border = Rectangle.NO_BORDER;

            var qrSize = new iTextSharp.text.Rectangle(50, 50);
            BarcodeQRCode qrcode = new BarcodeQRCode(dOrder.DONumber, 1, 1, null);
            Image qrcodeImage = qrcode.GetImage();
            qrcodeImage.ScaleAbsolute(qrSize);

            PdfPCell cell = new PdfPCell(png);
            cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            cell.Border = Rectangle.NO_BORDER;
            table2.AddCell(cell);

            PdfPCell cell4 = new PdfPCell() { Colspan = 2 };
            cell4.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            cell4.Border = Rectangle.NO_BORDER;
            table2.AddCell(cell4);

            //table2.AddCell(getCell("E-DO QR Code", PdfPCell.ALIGN_CENTER));
            PdfPCell cell3 = new PdfPCell(qrcodeImage);
            cell3.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            cell3.Border = Rectangle.NO_BORDER;
            table2.AddCell(cell3);
            //table2.AddCell(getCell("ayam ", PdfPCell.ALIGN_CENTER));

            PdfPCell cell2 = new PdfPCell(new Phrase("Company:", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD))) { Rowspan = 2 };
            cell2.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            cell2.Border = Rectangle.NO_BORDER;
            table2.AddCell(cell2);

            table2.AddCell(getCellNoBord("Gourmetz PTE LTD", PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellBoldNoBord("DO Number : ", PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellNoBord(dOrder.DONumber, PdfPCell.ALIGN_LEFT));

            //table2.AddCell(getCell("GST Regn: ", PdfPCell.ALIGN_CENTER));
            //table2.AddCell(getCell("M90363671C ", PdfPCell.ALIGN_CENTER));

            table2.AddCell(getCellNoBord("234 Pandan Loop, Singapore 128422", PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellBoldNoBord("Date : ", PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellNoBord(dOrder.CreatedDate.ToString("dddd, MMM dd, yyyy"), PdfPCell.ALIGN_LEFT));

            table2.AddCell(getCellBoldNoBord("UEN NO: ", PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellNoBord("197300678G ", PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellBoldNoBord("Pickup Time : ", PdfPCell.ALIGN_LEFT));
            if (dOrder.PickupTime != null)
            {
                table2.AddCell(getCellNoBord(dOrder.PickupTime.Value.ToString("h:mm tt"), PdfPCell.ALIGN_LEFT));
            }
            else
            {
                table2.AddCell(getCellNoBord(" ", PdfPCell.ALIGN_LEFT));
            }

            table2.AddCell(getCellBoldNoBord("From : ", PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellNoBord(dOrder.FromStoreName, PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellBoldNoBord("Route : ", PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellNoBord(dOrder.RouteName, PdfPCell.ALIGN_LEFT));

            table2.AddCell(getCellBoldNoBord("To Location :   ", PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellNoBord(dOrder.ToStoreName, PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellBoldNoBord("Meal Session :   ", PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellNoBord(dOrder.MealSessionName, PdfPCell.ALIGN_LEFT));

            table2.AddCell(getCellBoldNoBord("Address :   ", PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellNoBord(dOrder.ToStoreAddress, PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellBoldNoBord("", PdfPCell.ALIGN_LEFT));
            table2.AddCell(getCellNoBord("", PdfPCell.ALIGN_LEFT));




            table2.SpacingAfter = 10;
            document.Add(table2);

        }

        public void printTableHeader(PdfPTable table)
        {
            PdfPCell cell_1_h = new PdfPCell(new Phrase("Carton No", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            PdfPCell cell_2_h = new PdfPCell(new Phrase("Item Description", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            PdfPCell cell_3_h = new PdfPCell(new Phrase("Categories", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            PdfPCell cell_4_h = new PdfPCell(new Phrase("Ordered Qty", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            PdfPCell cell_5_h = new PdfPCell(new Phrase("Issued Qty", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));

            cell_1_h.HorizontalAlignment = Element.ALIGN_CENTER;
            cell_1_h.BackgroundColor = BaseColor.LIGHT_GRAY;
            //cell_1_h.BorderWidth = 1f;
            cell_2_h.HorizontalAlignment = Element.ALIGN_CENTER;
            cell_2_h.BackgroundColor = BaseColor.LIGHT_GRAY;
            //cell_2_h.BorderWidth = 1f;
            cell_3_h.HorizontalAlignment = Element.ALIGN_CENTER;
            cell_3_h.BackgroundColor = BaseColor.LIGHT_GRAY;
            //cell_3_h.BorderWidth = 1f;
            cell_4_h.HorizontalAlignment = Element.ALIGN_CENTER;
            cell_4_h.BackgroundColor = BaseColor.LIGHT_GRAY;
            //cell_4_h.BorderWidth = 1f;
            cell_5_h.HorizontalAlignment = Element.ALIGN_CENTER;
            cell_5_h.BackgroundColor = BaseColor.LIGHT_GRAY;
            //cell_5_h.BorderWidth = 1f;


            table.AddCell(cell_1_h);
            table.AddCell(cell_2_h);
            table.AddCell(cell_3_h);
            table.AddCell(cell_4_h);
            table.AddCell(cell_5_h);
        }

        public async Task<byte[]> GeneratePrintDo(int doId)
        {
            var imageFolderName = Path.Combine("Resources", "Images");
            var pathToImageSave = Path.Combine(Directory.GetCurrentDirectory(), imageFolderName);

            var fullImagePath = Path.Combine(pathToImageSave, "SFS_DO.jpg");

            var dOrder = await GetDeliveryOrderNewByIdAsync(doId);

            BaseFilter invFilter = new BaseFilter();
            invFilter.Filters = "(DeliveryOrderNewID)==" + doId;
            var dInvs = await GetStoreInventoriesAsync(invFilter);

            StoreInventoryDTO dInv = new StoreInventoryDTO();

            if (dInvs.PagedData.Count > 0)
            {
                dInv = dInvs.PagedData[0];
            }

            if (dOrder != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    #region pdfheader
                    Document document = new Document(PageSize.A4, 20, 20, 30, 30);
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    //writer.PageEvent = new PageNumberEvent();
                    document.Open();


                    float[] widths3 = new float[] { 3.4f, 7f, 2.2f, 1.8f, 1.6f };


                    printHeader(dOrder, document);


                    PdfPTable table = new PdfPTable(5);
                    table.SetWidths(widths3);
                    table.WidthPercentage = 100f;

                    printTableHeader(table);

                    #endregion

                    #region pdfcontent
                    var totalQty = 0;
                    var totalIssued = 0;
                    var cartonNo = 1;

                    var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);

                    var notCompleted = false;

                    float totalRowCount = 0;
                    dOrder.DeliveryDetails.ForEach(c =>
                    {
                        totalRowCount += 1;
                        List<DoPrintDTO> DOReports = new List<DoPrintDTO>();
                        c.DeliveryBentos.ForEach(b =>
                        {

                            var rep = DOReports.Find(r => r.dishID == b.DishId);
                            if (rep == null)
                            {
                                var repDish = new DoPrintDTO();
                                repDish.dishID = b.DishId.Value;

                                DOReports.Add(repDish);
                                totalRowCount += 1;
                            }

                        });
                    });

                    var totalPage = Math.Ceiling(totalRowCount / 30);

                    var rowCount = 0;

                    dOrder.DeliveryDetails.ForEach(c =>
                    {

                        if (rowCount == 30)
                        {
                            table.SpacingAfter = 20;
                            document.Add(table);
                            pageNumber(writer, document, (int)totalPage);
                            document.NewPage();

                            table = new PdfPTable(5);
                            table.SetWidths(widths3);
                            table.WidthPercentage = 100f;

                            printHeader(dOrder, document);

                            printTableHeader(table);

                            rowCount = 0;
                        }

                        PdfPCell cell_1 = new PdfPCell(new Phrase("Carton No " + cartonNo++, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                        var cartonAsset = new Phrase();
                        cartonAsset.Add(new Chunk(c.CartonAssetCode.Substring(0, c.CartonAssetCode.Length - 6), normalFont));
                        cartonAsset.Add(new Chunk(c.CartonAssetCode.Substring(c.CartonAssetCode.Length - 6), boldFont));
                        PdfPCell cell_2 = new PdfPCell(cartonAsset);
                        PdfPCell cell_3 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                        PdfPCell cell_4 = new PdfPCell(new Phrase(""));
                        PdfPCell cell_5 = new PdfPCell(new Phrase(""));

                        cell_1.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell_1.BackgroundColor = BaseColor.LIGHT_GRAY;
                        cell_1.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        //cell_1.BorderWidth = 1f;
                        cell_2.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell_2.BackgroundColor = BaseColor.LIGHT_GRAY;
                        cell_2.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        //cell_2.BorderWidth = 1f;
                        cell_3.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell_3.BackgroundColor = BaseColor.LIGHT_GRAY;
                        cell_3.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        //cell_3.BorderWidth = 1f;
                        cell_4.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell_4.BackgroundColor = BaseColor.LIGHT_GRAY;
                        cell_4.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        //cell_4.BorderWidth = 1f;
                        cell_5.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell_5.BackgroundColor = BaseColor.LIGHT_GRAY;
                        cell_5.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        //cell_5.BorderWidth = 1f;

                        table.AddCell(cell_1);
                        table.AddCell(cell_2);
                        table.AddCell(cell_3);
                        table.AddCell(cell_4);
                        table.AddCell(cell_5);

                        rowCount += 1;

                        List<DoPrintDTO> DOReports = new List<DoPrintDTO>();

                        c.DeliveryBentos.ForEach(b =>
                        {

                            var rep = DOReports.Find(r => r.dishID == b.DishId);
                            var rec = (StoreInventoryDetailDTO)null;
                            if (dInv.StoreInventoryDetails != null)
                            {
                                rec = dInv.StoreInventoryDetails.Find(r => (r.DishId == b.DishId && r.CartonId == c.CartonAssetId));
                            }
                            if (rep == null)
                            {
                                var repDish = new DoPrintDTO();
                                repDish.dishID = b.DishId.Value;
                                repDish.dishLabel = b.DishLabel;
                                repDish.dishCode = b.DishCode;
                                repDish.categories = b.DishType;
                                repDish.dishQty = b.Qty;
                                repDish.issQty = rec != null ? rec.QtyReceived : 0;
                                totalQty += b.Qty;
                                totalIssued += repDish.issQty;
                                repDish.bentoList = "";
                                if (b.BentoAssetCode != null && b.BentoAssetCode != "")
                                {
                                    repDish.bentoList += b.BentoAssetCode;
                                }

                                DOReports.Add(repDish);
                            }
                            else
                            {
                                rep.dishQty += b.Qty;
                                totalQty += b.Qty;
                                //rep.issQty += (rec != null ? rec.QtyReceived : 0);
                                //totalIssued += (rec != null ? rec.QtyReceived : 0);
                                if (b.BentoAssetCode != null && b.BentoAssetCode != "")
                                {
                                    if (rep.bentoList.Length > 0)
                                    {
                                        rep.bentoList += ", ";
                                    }
                                    rep.bentoList += b.BentoAssetCode;
                                }
                            }

                        });

                        DOReports.ForEach(dd =>
                        {

                            if (rowCount == 30)
                            {
                                table.SpacingAfter = 20;
                                document.Add(table);
                                pageNumber(writer, document, (int)totalPage);
                                document.NewPage();

                                table = new PdfPTable(5);
                                table.SetWidths(widths3);
                                table.WidthPercentage = 100f;

                                printHeader(dOrder, document);

                                printTableHeader(table);

                                rowCount = 0;
                            }

                            PdfPCell i_cell_1 = new PdfPCell(new Phrase(dd.dishCode, new Font(Font.FontFamily.HELVETICA, 10)));
                            PdfPCell i_cell_2 = new PdfPCell(new Phrase(dd.dishLabel, new Font(Font.FontFamily.HELVETICA, 10)));
                            PdfPCell i_cell_4 = new PdfPCell(new Phrase(dd.categories, new Font(Font.FontFamily.HELVETICA, 10)));
                            PdfPCell i_cell_3 = new PdfPCell();
                            if (dd.dishQty == dd.issQty)
                            {
                                i_cell_3 = new PdfPCell(new Phrase(dd.dishQty + "", new Font(Font.FontFamily.HELVETICA, 10)));
                            }
                            else
                            {
                                notCompleted = true;
                                i_cell_3 = new PdfPCell(new Phrase(dd.dishQty + "", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                            }
                            PdfPCell i_cell_5 = new PdfPCell(new Phrase(dd.issQty + "", new Font(Font.FontFamily.HELVETICA, 10)));

                            i_cell_1.HorizontalAlignment = Element.ALIGN_CENTER;
                            i_cell_2.HorizontalAlignment = Element.ALIGN_LEFT;
                            i_cell_4.HorizontalAlignment = Element.ALIGN_CENTER;
                            i_cell_3.HorizontalAlignment = Element.ALIGN_CENTER;
                            i_cell_5.HorizontalAlignment = Element.ALIGN_CENTER;

                            i_cell_1.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                            i_cell_2.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                            i_cell_3.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                            i_cell_4.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                            i_cell_5.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;

                            table.AddCell(i_cell_1);
                            table.AddCell(i_cell_2);
                            table.AddCell(i_cell_4);
                            table.AddCell(i_cell_3);
                            table.AddCell(i_cell_5);

                            rowCount += 1;

                        });

                    });

                    PdfPCell emptyCell = new PdfPCell(new Phrase(""));

                    emptyCell.HorizontalAlignment = Element.ALIGN_LEFT;

                    //table.AddCell(emptyCell);
                    //table.AddCell(emptyCell);
                    //table.AddCell(emptyCell);
                    //table.AddCell(emptyCell);

                    //table.AddCell(emptyCell);
                    //table.AddCell(emptyCell);
                    //table.AddCell(emptyCell);
                    //table.AddCell(emptyCell);

                    //table.AddCell(emptyCell);
                    //table.AddCell(emptyCell);
                    PdfPCell t_cell_1 = new PdfPCell(new Phrase("Total Meals: ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD))) { Colspan = 3 };
                    PdfPCell t_cell_2 = new PdfPCell(new Phrase(totalQty + "", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                    PdfPCell t_cell_3 = new PdfPCell(new Phrase(totalIssued + "", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));

                    t_cell_1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    t_cell_2.HorizontalAlignment = Element.ALIGN_CENTER;
                    t_cell_3.HorizontalAlignment = Element.ALIGN_CENTER;


                    t_cell_1.BackgroundColor = BaseColor.LIGHT_GRAY;
                    t_cell_2.BackgroundColor = BaseColor.LIGHT_GRAY;
                    t_cell_3.BackgroundColor = BaseColor.LIGHT_GRAY;

                    table.AddCell(t_cell_1);
                    table.AddCell(t_cell_2);
                    table.AddCell(t_cell_3);

                    table.SpacingAfter = 20;
                    document.Add(table);

                    PdfPTable table3 = new PdfPTable(2);
                    float[] widths2 = new float[] { 3.4f, 7f };
                    table3.SetWidths(widths2);
                    table3.WidthPercentage = 65f;

                    table3.AddCell(getCellBoldNoBord("Issued By : ", PdfPCell.ALIGN_LEFT));
                    table3.AddCell(getCellNoBord(dOrder.CreatedByName + " - " + dOrder.CreatedDate.ToString("dd MMMM yy, h:mm tt"), PdfPCell.ALIGN_LEFT));

                    PdfPCell cellBlankRow = new PdfPCell(new Phrase(" "));
                    cellBlankRow.Colspan = 2;
                    cellBlankRow.HorizontalAlignment = 1;
                    cellBlankRow.FixedHeight = 10;
                    cellBlankRow.Border = Rectangle.NO_BORDER;
                    table3.AddCell(cellBlankRow);

                    table3.AddCell(getCellBoldNoBord("Delivered By : ", PdfPCell.ALIGN_LEFT));
                    var loadingTime = "";
                    if (dOrder.LoadingTime != DateTime.MinValue)
                    {
                        loadingTime = dOrder.LoadingTime.ToString("dd MMMM yy, h:mm tt");
                    }
                    table3.AddCell(getCellNoBord(dOrder.VehicleNumber + " - " + loadingTime, PdfPCell.ALIGN_LEFT));
                    table3.AddCell(cellBlankRow);
                    table3.AddCell(getCellBoldNoBord("Received By : ", PdfPCell.ALIGN_LEFT));
                    var receivingTime = "";
                    if (dInv.UpdatedDate != DateTime.MinValue)
                    {
                        receivingTime = dInv.UpdatedDate.ToString("dd MMMM yy, h:mm tt");
                    }
                    table3.AddCell(getCellNoBord(dInv.ReceivedBy + " - " + receivingTime, PdfPCell.ALIGN_LEFT));
                    table3.AddCell(cellBlankRow);


                    table3.HorizontalAlignment = Element.ALIGN_LEFT;
                    document.Add(table3);


                    PdfPTable table5 = new PdfPTable(1);
                    table5.WidthPercentage = 100f;

                    PdfPCell remarkRow = new PdfPCell(new Phrase("Remarks : ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                    remarkRow.HorizontalAlignment = Element.ALIGN_LEFT;
                    //remarkRow.FixedHeight = 50;
                    remarkRow.Border = Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    table5.AddCell(remarkRow);

                    var text = "";

                    if (dOrder.VehicleNumber == null || dOrder.VehicleNumber == "" || dOrder.ReceivingTime == null)
                    {
                        text = "";
                    }
                    else if (notCompleted)
                    {
                        text = "Please note that there are item(s) that has not been issued.Please follow-up.";
                    }
                    else
                    {
                        text = "N/A";
                    }

                    PdfPCell remarkContentRow = new PdfPCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 10)));
                    remarkContentRow.HorizontalAlignment = Element.ALIGN_LEFT;
                    remarkContentRow.FixedHeight = 40;
                    remarkContentRow.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;

                    table5.AddCell(remarkContentRow);

                    document.Add(table5);

                    pageNumber(writer, document, (int)totalPage);

                    document.Close();
                    writer.Close();
                    #endregion

                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<byte[]> GeneratePrintCartonLabel(int cartonId)
        {
            //var dOrder = await GetDeliveryOrderNewByIdAsync(doId);

            BaseFilter filterDis = new BaseFilter();

            filterDis.Filters = "(isActive)==True,(CartonAssetId)==" + cartonId;

            var disposables = _mapper.Map<PagedEntity<CartonDisposableBoxDTO>>(await GetDisposableBoxesAsync(filterDis));

            var bentos = _mapper.Map<PagedEntity<BentoAssetDTO>>(await GetBentoAssetsAsync(filterDis));

            List<DoPrintDTO> DOReports = new List<DoPrintDTO>();

            var cartonCode = "";

            var totalQty = 0;

            if (disposables.TotalCount <= 0 && bentos.TotalCount <= 0)
            {
                return null;
            }

            foreach (CartonDisposableBoxDTO d in disposables.PagedData)
            {
                var rep = DOReports.Find(r => r.dishID == d.DishId);
                if (rep == null)
                {
                    var repDish = new DoPrintDTO();
                    repDish.dishID = d.DishId.Value;
                    repDish.dishLabel = d.DishLabel;
                    repDish.dishCode = d.DishCode;
                    //repDish.categories = d.Dish;
                    repDish.dishQty = d.Qty;
                    totalQty += d.Qty;
                    repDish.bentoList = "";
                    cartonCode = d.CartonAssetCode;

                    DOReports.Add(repDish);
                }
                else
                {
                    rep.dishQty += d.Qty;
                    totalQty += d.Qty;
                }
            }

            foreach (BentoAssetDTO d in bentos.PagedData)
            {
                var rep = DOReports.Find(r => r.dishID == d.DishId);
                if (rep == null)
                {
                    var repDish = new DoPrintDTO();
                    repDish.dishID = d.DishId.Value;
                    repDish.dishLabel = d.DishLabel;
                    repDish.dishCode = d.DishCode;
                    //repDish.categories = d.Dish;
                    repDish.dishQty = 1;
                    totalQty += 1;
                    repDish.bentoList = "";
                    cartonCode = d.CartonAssetCode;

                    DOReports.Add(repDish);
                }
                else
                {
                    rep.dishQty += 1;
                    totalQty += 1;
                }
            }


            using (var stream = new System.IO.MemoryStream())
            {
                #region pdfheader
                Document document = new Document(PageSize.A6, 10, 10, 20, 20);
                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100f;

                PdfPCell cell_1 = new PdfPCell(new Phrase("Carton Code: ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                PdfPCell cell_2 = new PdfPCell(new Phrase(cartonCode, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                PdfPCell cell_3 = new PdfPCell(new Phrase(""));

                cell_1.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell_1.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell_1.BorderWidth = 1f;
                cell_2.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell_2.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell_2.BorderWidth = 1f;
                cell_3.HorizontalAlignment = Element.ALIGN_LEFT;
                //cell_3.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell_3.BorderWidth = 1f;

                table.AddCell(cell_1);
                table.AddCell(cell_2);
                table.AddCell(cell_3);

                PdfPCell cell_1_h = new PdfPCell(new Phrase("Dish Code", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                PdfPCell cell_2_h = new PdfPCell(new Phrase("Dish Label", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                PdfPCell cell_3_h = new PdfPCell(new Phrase("Qty", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));

                cell_1_h.HorizontalAlignment = Element.ALIGN_LEFT;
                cell_1_h.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell_1_h.BorderWidth = 1f;
                cell_2_h.HorizontalAlignment = Element.ALIGN_LEFT;
                cell_2_h.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell_2_h.BorderWidth = 1f;
                cell_3_h.HorizontalAlignment = Element.ALIGN_LEFT;
                cell_3_h.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell_3_h.BorderWidth = 1f;

                table.AddCell(cell_1_h);
                table.AddCell(cell_2_h);
                table.AddCell(cell_3_h);

                DOReports.ForEach(dd =>
                {
                    PdfPCell i_cell_1 = new PdfPCell(new Phrase(dd.dishCode, new Font(Font.FontFamily.HELVETICA, 10)));
                    PdfPCell i_cell_2 = new PdfPCell(new Phrase(dd.dishLabel, new Font(Font.FontFamily.HELVETICA, 10)));
                    PdfPCell i_cell_3 = new PdfPCell(new Phrase(dd.dishQty + "", new Font(Font.FontFamily.HELVETICA, 10)));

                    i_cell_1.HorizontalAlignment = Element.ALIGN_LEFT;
                    i_cell_2.HorizontalAlignment = Element.ALIGN_LEFT;
                    i_cell_3.HorizontalAlignment = Element.ALIGN_LEFT;

                    table.AddCell(i_cell_1);
                    table.AddCell(i_cell_2);
                    table.AddCell(i_cell_3);

                });

                PdfPCell t_cell_1 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));
                PdfPCell t_cell_2 = new PdfPCell(new Phrase("Total Qty: ", new Font(Font.FontFamily.HELVETICA, 10)));
                PdfPCell t_cell_3 = new PdfPCell(new Phrase(totalQty + "", new Font(Font.FontFamily.HELVETICA, 10)));

                t_cell_1.HorizontalAlignment = Element.ALIGN_LEFT;
                t_cell_2.HorizontalAlignment = Element.ALIGN_LEFT;
                t_cell_3.HorizontalAlignment = Element.ALIGN_LEFT;

                table.AddCell(t_cell_1);
                table.AddCell(t_cell_2);
                table.AddCell(t_cell_3);


                document.Add(table);

                document.Close();
                writer.Close();
                #endregion

                return stream.ToArray();
            }



            //if (dOrder != null)
            //{
            //    using (var stream = new System.IO.MemoryStream())
            //    {
            //        #region pdfheader
            //        Document document = new Document(PageSize.A6, 10, 10, 20, 20);
            //        PdfWriter writer = PdfWriter.GetInstance(document, stream);
            //        document.Open();

            //        //PdfPTable table2 = new PdfPTable(4);
            //        //float[] widths = new float[] { 1f, 3f, 1.25f, 1.75f };
            //        //table2.SetWidths(widths);
            //        //table2.WidthPercentage = 100f;

            //        //table2.AddCell(getCell("From : ", PdfPCell.ALIGN_LEFT));
            //        //table2.AddCell(getCell(dOrder.FromStoreName, PdfPCell.ALIGN_CENTER));
            //        //table2.AddCell(getCell("DO Number : ", PdfPCell.ALIGN_CENTER));
            //        //table2.AddCell(getCell(dOrder.DONumber, PdfPCell.ALIGN_CENTER));

            //        //table2.AddCell(getCell("To Location :   ", PdfPCell.ALIGN_LEFT));
            //        //table2.AddCell(getCell(dOrder.ToStoreName, PdfPCell.ALIGN_CENTER));
            //        //table2.AddCell(getCell("Date : ", PdfPCell.ALIGN_CENTER));
            //        //table2.AddCell(getCell(dOrder.CreatedDate.ToString("dd MMMM yyyy"), PdfPCell.ALIGN_CENTER));

            //        //table2.AddCell(getCell("Address :   ", PdfPCell.ALIGN_LEFT));
            //        //table2.AddCell(getCell(dOrder.ToStoreAddress, PdfPCell.ALIGN_CENTER));
            //        //table2.AddCell(getCell("Route : ", PdfPCell.ALIGN_CENTER));
            //        //table2.AddCell(getCell("N/A", PdfPCell.ALIGN_CENTER));

            //        //table2.AddCell(getCell("Meal Period :   ", PdfPCell.ALIGN_LEFT));
            //        //table2.AddCell(getCell("N/A", PdfPCell.ALIGN_CENTER));
            //        //table2.AddCell(getCell("Pickup Time : ", PdfPCell.ALIGN_CENTER));
            //        //table2.AddCell(getCell("N/A", PdfPCell.ALIGN_CENTER));


            //        //table2.SpacingAfter = 10;
            //        //document.Add(table2);

            //        #endregion

            //        #region pdfcontent
            //        dOrder.DeliveryDetails.ForEach(c => {

            //            PdfPTable table = new PdfPTable(4);
            //            table.WidthPercentage = 100f;

            //            PdfPCell cell_1_h = new PdfPCell(new Phrase("Carton No", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            //            PdfPCell cell_2_h = new PdfPCell(new Phrase("Item Description", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            //            PdfPCell cell_3_h = new PdfPCell(new Phrase("Categories", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            //            PdfPCell cell_4_h = new PdfPCell(new Phrase("QTY", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));

            //            cell_1_h.HorizontalAlignment = Element.ALIGN_LEFT;
            //            //cell_1_h.BackgroundColor = BaseColor.LIGHT_GRAY;
            //            cell_1_h.BorderWidth = 1f;
            //            cell_2_h.HorizontalAlignment = Element.ALIGN_LEFT;
            //            //cell_2_h.BackgroundColor = BaseColor.LIGHT_GRAY;
            //            cell_2_h.BorderWidth = 1f;
            //            cell_3_h.HorizontalAlignment = Element.ALIGN_LEFT;
            //            //cell_3_h.BackgroundColor = BaseColor.LIGHT_GRAY;
            //            cell_3_h.BorderWidth = 1f;
            //            cell_4_h.HorizontalAlignment = Element.ALIGN_LEFT;
            //            //cell_4_h.BackgroundColor = BaseColor.LIGHT_GRAY;
            //            cell_4_h.BorderWidth = 1f;


            //            table.AddCell(cell_1_h);
            //            table.AddCell(cell_2_h);
            //            table.AddCell(cell_3_h);
            //            table.AddCell(cell_4_h);

            //            PdfPCell cell_1 = new PdfPCell(new Phrase(c.CartonAssetCode, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            //            PdfPCell cell_2 = new PdfPCell(new Phrase(""));
            //            PdfPCell cell_3 = new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            //            PdfPCell cell_4 = new PdfPCell(new Phrase(""));

            //            cell_1.HorizontalAlignment = Element.ALIGN_LEFT;
            //            //cell_1.BackgroundColor = BaseColor.LIGHT_GRAY;
            //            cell_1.BorderWidth = 1f;
            //            cell_2.HorizontalAlignment = Element.ALIGN_LEFT;
            //            //cell_2.BackgroundColor = BaseColor.LIGHT_GRAY;
            //            cell_2.BorderWidth = 1f;
            //            cell_3.HorizontalAlignment = Element.ALIGN_LEFT;
            //            //cell_3.BackgroundColor = BaseColor.LIGHT_GRAY;
            //            cell_3.BorderWidth = 1f;
            //            cell_3.HorizontalAlignment = Element.ALIGN_LEFT;
            //            //cell_3.BackgroundColor = BaseColor.LIGHT_GRAY;
            //            cell_3.BorderWidth = 1f;

            //            table.AddCell(cell_1);
            //            table.AddCell(cell_2);
            //            table.AddCell(cell_3);
            //            table.AddCell(cell_4);


            //            List<DoPrintDTO> DOReports = new List<DoPrintDTO>();

            //            c.DeliveryBentos.ForEach(b => {

            //                var rep = DOReports.Find(r => r.dishID == b.DishId);
            //                if (rep == null)
            //                {
            //                    var repDish = new DoPrintDTO();
            //                    repDish.dishID = b.DishId.Value;
            //                    repDish.dishLabel = b.DishLabel;
            //                    repDish.dishCode = b.DishCode;
            //                    repDish.categories = b.DishType;
            //                    repDish.dishQty = b.Qty;
            //                    repDish.bentoList = "";
            //                    if (b.BentoAssetCode != null && b.BentoAssetCode != "")
            //                    {
            //                        repDish.bentoList += b.BentoAssetCode;
            //                    }

            //                    DOReports.Add(repDish);
            //                }
            //                else
            //                {
            //                    rep.dishQty += b.Qty;
            //                    if (b.BentoAssetCode != null && b.BentoAssetCode != "")
            //                    {
            //                        if (rep.bentoList.Length > 0)
            //                        {
            //                            rep.bentoList += ", ";
            //                        }
            //                        rep.bentoList += b.BentoAssetCode;
            //                    }
            //                }

            //            });

            //            DOReports.ForEach(dd => {
            //                PdfPCell i_cell_1 = new PdfPCell(new Phrase(dd.dishCode, new Font(Font.FontFamily.HELVETICA, 10)));
            //                PdfPCell i_cell_2 = new PdfPCell(new Phrase(dd.dishLabel, new Font(Font.FontFamily.HELVETICA, 10)));
            //                PdfPCell i_cell_4 = new PdfPCell(new Phrase(dd.categories, new Font(Font.FontFamily.HELVETICA, 10)));
            //                PdfPCell i_cell_3 = new PdfPCell(new Phrase(dd.dishQty + "", new Font(Font.FontFamily.HELVETICA, 10)));

            //                i_cell_1.HorizontalAlignment = Element.ALIGN_LEFT;
            //                i_cell_2.HorizontalAlignment = Element.ALIGN_LEFT;
            //                i_cell_4.HorizontalAlignment = Element.ALIGN_LEFT;
            //                i_cell_3.HorizontalAlignment = Element.ALIGN_LEFT;

            //                table.AddCell(i_cell_1);
            //                table.AddCell(i_cell_2);
            //                table.AddCell(i_cell_4);
            //                table.AddCell(i_cell_3);

            //            });

            //            document.Add(table);
            //            document.NewPage();

            //        });

            //        document.Close();
            //        writer.Close();
            //        #endregion

            //        return stream.ToArray();
            //    }
            //}
            //else
            //{
            //    return null;
            //}
        }


        public async Task<byte[]> GeneratePrintCartonLabelV2(int cartonId, int routeId)
        {
            //var dOrder = await GetDeliveryOrderNewByIdAsync(doId);

            BaseFilter filterDis = new BaseFilter();

            filterDis.Filters = "(isActive)==True,(CartonAssetId)==" + cartonId + ",(RouteId)==" + routeId;

            var disposables = _mapper.Map<PagedEntity<CartonDisposableBoxDTO>>(await GetDisposableBoxesAsync(filterDis));

            var bentos = _mapper.Map<PagedEntity<BentoAssetDTO>>(await GetBentoAssetsAsync(filterDis));

            var route = _mapper.Map<RouteDTO>(await GetRouteByIdAsync(routeId));

            List<DoPrintDTO> DOReports = new List<DoPrintDTO>();

            var cartonCode = "";

            var totalQty = 0;

            if (disposables.TotalCount <= 0 && bentos.TotalCount <= 0)
            {
                return null;
            }

            foreach (CartonDisposableBoxDTO d in disposables.PagedData)
            {
                var rep = DOReports.Find(r => r.dishID == d.DishId);
                if (rep == null)
                {
                    var repDish = new DoPrintDTO();
                    repDish.dishID = d.DishId.Value;
                    repDish.dishLabel = d.DishLabel;
                    repDish.dishCode = d.DishCode;
                    //repDish.categories = d.Dish;
                    repDish.dishQty = d.Qty;
                    totalQty += d.Qty;
                    repDish.bentoList = "";
                    cartonCode = d.CartonAssetCode;

                    DOReports.Add(repDish);
                }
                else
                {
                    rep.dishQty += d.Qty;
                    totalQty += d.Qty;
                }
            }

            foreach (BentoAssetDTO d in bentos.PagedData)
            {
                var rep = DOReports.Find(r => r.dishID == d.DishId);
                if (rep == null)
                {
                    var repDish = new DoPrintDTO();
                    repDish.dishID = d.DishId.Value;
                    repDish.dishLabel = d.DishLabel;
                    repDish.dishCode = d.DishCode;
                    //repDish.categories = d.Dish;
                    repDish.dishQty = 1;
                    totalQty += 1;
                    repDish.bentoList = "";
                    cartonCode = d.CartonAssetCode;

                    DOReports.Add(repDish);
                }
                else
                {
                    rep.dishQty += 1;
                    totalQty += 1;
                }
            }


            using (var stream = new System.IO.MemoryStream())
            {
                #region pdfheader
                Document document = new Document(PageSize.A6, 10, 10, 20, 20);
                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100f;

                PdfPCell cell_1 = new PdfPCell(new Phrase("Carton Code: ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                PdfPCell cell_2 = new PdfPCell(new Phrase(cartonCode, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                PdfPCell cell_3 = new PdfPCell(new Phrase(""));

                System.Drawing.Color systemColor = System.Drawing.ColorTranslator.FromHtml(route.Color);

                BaseColor bgColor = new BaseColor(systemColor.R, systemColor.G, systemColor.B);

                cell_1.HorizontalAlignment = Element.ALIGN_LEFT;
                cell_1.BackgroundColor = bgColor;
                cell_1.BorderWidth = 1f;
                cell_2.HorizontalAlignment = Element.ALIGN_LEFT;
                cell_2.BackgroundColor = bgColor;
                cell_2.BorderWidth = 1f;
                cell_3.HorizontalAlignment = Element.ALIGN_LEFT;
                cell_3.BackgroundColor = bgColor;
                cell_3.BorderWidth = 1f;

                table.AddCell(cell_1);
                table.AddCell(cell_2);
                table.AddCell(cell_3);

                PdfPCell cell_1_h = new PdfPCell(new Phrase("Dish Code", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                PdfPCell cell_2_h = new PdfPCell(new Phrase("Dish Label", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                PdfPCell cell_3_h = new PdfPCell(new Phrase("Qty", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));

                cell_1_h.HorizontalAlignment = Element.ALIGN_LEFT;
                cell_1_h.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell_1_h.BorderWidth = 1f;
                cell_2_h.HorizontalAlignment = Element.ALIGN_LEFT;
                cell_2_h.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell_2_h.BorderWidth = 1f;
                cell_3_h.HorizontalAlignment = Element.ALIGN_LEFT;
                cell_3_h.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell_3_h.BorderWidth = 1f;

                table.AddCell(cell_1_h);
                table.AddCell(cell_2_h);
                table.AddCell(cell_3_h);

                DOReports.ForEach(dd =>
                {
                    PdfPCell i_cell_1 = new PdfPCell(new Phrase(dd.dishCode, new Font(Font.FontFamily.HELVETICA, 10)));
                    PdfPCell i_cell_2 = new PdfPCell(new Phrase(dd.dishLabel, new Font(Font.FontFamily.HELVETICA, 10)));
                    PdfPCell i_cell_3 = new PdfPCell(new Phrase(dd.dishQty + "", new Font(Font.FontFamily.HELVETICA, 10)));

                    i_cell_1.HorizontalAlignment = Element.ALIGN_LEFT;
                    i_cell_2.HorizontalAlignment = Element.ALIGN_LEFT;
                    i_cell_3.HorizontalAlignment = Element.ALIGN_LEFT;

                    table.AddCell(i_cell_1);
                    table.AddCell(i_cell_2);
                    table.AddCell(i_cell_3);

                });

                PdfPCell t_cell_1 = new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));
                PdfPCell t_cell_2 = new PdfPCell(new Phrase("Total Qty: ", new Font(Font.FontFamily.HELVETICA, 10)));
                PdfPCell t_cell_3 = new PdfPCell(new Phrase(totalQty + "", new Font(Font.FontFamily.HELVETICA, 10)));

                t_cell_1.HorizontalAlignment = Element.ALIGN_LEFT;
                t_cell_2.HorizontalAlignment = Element.ALIGN_LEFT;
                t_cell_3.HorizontalAlignment = Element.ALIGN_LEFT;

                table.AddCell(t_cell_1);
                table.AddCell(t_cell_2);
                table.AddCell(t_cell_3);


                document.Add(table);

                document.Close();
                writer.Close();
                #endregion

                return stream.ToArray();
            }
        }

        #endregion

        #region Outlet Term

        public async Task<PagedEntity<OutletTermDTO>> GetOutletTermsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<OutletTermDTO>>(await this._uow.OutletTerms.GetOutletTermsAsync(filter));
            return result;
        }

        public async Task<OutletTermDTO> GetOutletTermByIdAsync(int id)
        {
            return _mapper.Map<OutletTermDTO>(await this._uow.OutletTerms.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateOutletTermAsync(OutletTermDTO dto)
        {
            return await this._uow.OutletTerms.CreateAsync(_mapper.Map<OutletTerm>(dto));
        }

        public async Task<BaseOperationResponse> UpdateOutletTermAsync(OutletTermDTO dto)
        {
            return await this._uow.OutletTerms.UpdateAsync(_mapper.Map<OutletTerm>(dto));
        }

        public async Task<BaseOperationResponse> DeleteOutletTermAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.OutletTerms.DeleteAsync(id);
            return result;
        }

        #endregion

        public async Task<PagedEntity<CatererAssetDTO>> GetCatererAssetsAsync(CatererAsserFilter filter)
        {
            var result = _mapper.Map<PagedEntity<CatererAssetDTO>>(await this._uow.CatererAssetRepository.GetAsync(filter));
            return result;
        }

        public async Task<CatererAssetDTO> GetCatererAssetByIdAsync(int id)
        {
            return _mapper.Map<CatererAssetDTO>(await this._uow.CatererAssetRepository.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateCatererAssetAsync(CatererAssetDTO dto)
        {
            return await this._uow.CatererAssetRepository.CreateAsync(_mapper.Map<CatererAsset>(dto));
        }

        public async Task<BaseOperationResponse> UpdateCatererAssetAsync(CatererAssetDTO dto)
        {
            return await this._uow.CatererAssetRepository.UpdateAsync(_mapper.Map<CatererAsset>(dto));
        }

        public async Task<BaseOperationResponse> DeleteCatererAssetAsync(int id)
        {
            return await this._uow.CatererAssetRepository.DeleteAsync(id);
        }

        public async Task<string> GetAssetQRCode(int catererId)
        {
            return await this._uow.CatererAssetType.GetAssetQRCode(catererId);
        }

        public async Task<byte[]> GenerateAssetQRCode(int id, int catererId)
        {
            var asset = await this._uow.CatererAssetRepository.GetByIdAsync(id);
            var tagId = asset?.assetQRCode ?? string.Empty;

            var catererName = "Gourmetz Catering Pte Lt";
            var catererAddress = "";

            var catererInfo = await this._uow.CatererInfos.GetByIdAsync(catererId);
            if (catererInfo != null)
            {
                catererName = catererInfo.Name.Length > 24 ? catererInfo.Name.Substring(0, 24) : catererInfo.Name;
                var addresses = catererInfo.Address.Split('\n');

                catererAddress = string.Join("\n", addresses.Select(e => e.Length > 28 ? e.Substring(0, 28) : e));
            }

            using (var stream = new System.IO.MemoryStream())
            {
                var folderName = Path.Combine("Resources", "Font");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var imageFolderName = Path.Combine("Resources", "Images", "Default");
                var logoImagePath = Path.Combine(Directory.GetCurrentDirectory(), imageFolderName);

                var fullPath = Path.Combine(pathToSave, "Aller_Bd.ttf");
                var fullImagePath = Path.Combine(logoImagePath, "company_logo.png");

                BaseFont allerfont = BaseFont.CreateFont(fullPath, BaseFont.WINANSI, BaseFont.EMBEDDED);
                Font aller = new Font(allerfont, 12);

                var pgSize = new iTextSharp.text.Rectangle(227f, 114f); // Page size
                Document document = new Document(pgSize, 5, 5, 5, 5);
                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                // QR Code on the left half
                BarcodeQRCode qrCode = new BarcodeQRCode(tagId, 105, 105, null);
                iTextSharp.text.Image qrImage = qrCode.GetImage();
                qrImage.SetAbsolutePosition(0f, 12f);  // Positioned on the left side
                document.Add(qrImage);

                // Caterer name and address on the right half
                Paragraph para1 = new Paragraph(catererName, new Font(allerfont, 10));
                para1.Alignment = Element.ALIGN_RIGHT;
                para1.IndentationRight = 5f; // Align to the right side
                para1.PaddingTop = 0f;
                document.Add(para1);

                Image png = Image.GetInstance(fullImagePath);
                png.ScaleToFit(25f, 25f);
                png.SetAbsolutePosition(100f, 65f);
                document.Add(png);

                Paragraph para2 = new Paragraph(catererAddress, new Font(allerfont, 6));
                para2.Alignment = Element.ALIGN_RIGHT;
                para2.IndentationRight = 5f; // Align to the right side
                document.Add(para2);

                //// Add the rectangle around the tagId substring (para3) and make the text red
                //string tagIdSubstring = tagId.Substring(12, 4); // Extract substring
                PdfContentByte cb = writer.DirectContent;

                // Set the position and dimensions for the rectangle
                float rectX = 105f;
                float rectY = 30f;
                float rectWidth = 80f;
                float rectHeight = 30f;

                //// Draw rectangle
                //cb.Rectangle(rectX, rectY, rectWidth, rectHeight);
                //cb.Stroke();

                //// Add the text inside the rectangle in red
                //Font redFont = new Font(allerfont, 26, Font.NORMAL, BaseColor.RED);
                //ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, new Phrase(tagIdSubstring, redFont), rectX + rectWidth / 2, rectY + rectHeight / 4, 0);

                // Add the full tagId at the bottom, center-aligned across the entire page
                var phrase = new Phrase();
                phrase.Add(new Chunk(tagId, new Font(allerfont, 10, Font.NORMAL)));

                ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, new Phrase(phrase), 115f, 10f, 0);

                document.Close();
                writer.Close();

                return stream.ToArray();
            }
        }

        public async Task<PagedEntity<AssetComponentDTO>> GetAssetComponentAsync(AssetComponentFilter filter)
        {
            var result = _mapper.Map<PagedEntity<AssetComponentDTO>>(await this._uow.AssetComponentRepository.GetAsync(filter));
            return result;
        }

        public async Task<AssetComponentDTO> GetAssetComponentByIdAsync(int id)
        {
            return _mapper.Map<AssetComponentDTO>(await this._uow.AssetComponentRepository.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateAssetComponentAsync(AssetComponentDTO dto)
        {
            return await this._uow.AssetComponentRepository.CreateAsync(_mapper.Map<AssetComponent>(dto));
        }

        public async Task<BaseOperationResponse> UpdateAssetComponentAsync(AssetComponentDTO dto)
        {
            return await this._uow.AssetComponentRepository.UpdateAsync(_mapper.Map<AssetComponent>(dto));
        }

        public async Task<BaseOperationResponse> DeleteAssetComponentAsync(int id)
        {
            return await this._uow.AssetComponentRepository.DeleteAsync(id);
        }

        public async Task<PagedEntity<SortingAreaDTO>> GetSortingAreaAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<SortingAreaDTO>>(await this._uow.SortingAreaRepository.GetAsync(filter));
            return result;
        }

        public async Task<SortingAreaDTO> GetSortingAreaByIdAsync(int id)
        {
            return _mapper.Map<SortingAreaDTO>(await this._uow.SortingAreaRepository.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateSortingAreaAsync(SortingAreaDTO dto)
        {
            return await this._uow.SortingAreaRepository.CreateAsync(_mapper.Map<SortingArea>(dto));
        }

        public async Task<BaseOperationResponse> UpdateSortingAreaAsync(SortingAreaDTO dto)
        {
            return await this._uow.SortingAreaRepository.UpdateAsync(_mapper.Map<SortingArea>(dto));
        }

        public async Task<BaseOperationResponse> DeleteSortingAreaAsync(int id)
        {
            return await this._uow.SortingAreaRepository.DeleteAsync(id);
        }

        public async Task<byte[]> GenerateSortingAreaQRCode(int id, int catererId)
        {
            var sortingArea = await _uow.SortingAreaRepository.GetByIdAsync(id);
            string codeQRValue = sortingArea?.Code ?? "";
            string routeName = sortingArea?.Route?.Label ?? "";
            string routeDetails = sortingArea?.Route?.Details ?? "";
            DateTime pickupTimeVal = sortingArea?.Route?.Pickup ?? DateTime.Now;

            BaseColor routeColor = Common.ParseHexColor(sortingArea?.Route?.Color);

            using var stream = new MemoryStream();

            float pageWidth = PageSize.A4.Width;       // 595
            float pageHeight = PageSize.A4.Height;     // 842

            Document document = new Document(new Rectangle(pageWidth, pageHeight), 10, 10, 10, 10);
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            PdfContentByte cb = writer.DirectContent;

            // ---------------------------------------------------------
            // 1. PICKUP TIME SECTION (HEIGHT: 120px)
            // ---------------------------------------------------------
            float topBoxHeight = 120f;

            cb.SetColorFill(routeColor);
            cb.Rectangle(0, pageHeight - topBoxHeight, pageWidth, topBoxHeight);
            cb.Fill();

            // Load custom font
            string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Font", "Aller_Bd.ttf");
            BaseFont pickupFont = BaseFont.CreateFont(fontPath, BaseFont.WINANSI, BaseFont.EMBEDDED);

            Phrase pickupPhrase = new Phrase(
                pickupTimeVal.ToString("HH:mm"),
                new Font(pickupFont, 64, Font.NORMAL, BaseColor.WHITE)
            );

            ColumnText.ShowTextAligned(
                cb,
                Element.ALIGN_CENTER,
                pickupPhrase,
                pageWidth / 2,
                pageHeight - 75,
                0
            );

            // ---------------------------------------------------------
            // 2. QR CODE (TOP HALF)
            // ---------------------------------------------------------
            float qrSize = pageHeight * 0.62f;  // ~520 px, VERY BIG

            BarcodeQRCode qr = new BarcodeQRCode(codeQRValue, (int)qrSize, (int)qrSize, null);
            Image qrImg = qr.GetImage();

            qrImg.ScaleAbsolute(qrSize, qrSize);

            float qrX = (pageWidth - qrSize) / 2;      // Center horizontally
            float qrY = (pageHeight - qrSize) / 2;     // Center vertically BETWEEN top and bottom

            qrImg.SetAbsolutePosition(qrX, qrY);
            document.Add(qrImg);

            // ---------------------------------------------------------
            // 3. ROUTE NAME + DETAILS (BOTTOM STRIP)
            // ---------------------------------------------------------
            float bottomBoxHeight = 120f;

            BaseColor bottomBg = new BaseColor(30, 144, 255); // DodgerBlue

            cb.SetColorFill(bottomBg);
            cb.Rectangle(0, 0, pageWidth, bottomBoxHeight);
            cb.Fill();

            BaseFont helvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);

            // Route Name
            ColumnText.ShowTextAligned(
                cb,
                Element.ALIGN_CENTER,
                new Phrase(routeName, new Font(helvetica, 32, Font.BOLD, BaseColor.WHITE)),
                pageWidth / 2,
                bottomBoxHeight - 40,
                0
            );

            // Route Details
            ColumnText.ShowTextAligned(
                cb,
                Element.ALIGN_CENTER,
                new Phrase(routeDetails, new Font(helvetica, 20, Font.NORMAL, BaseColor.WHITE)),
                pageWidth / 2,
                bottomBoxHeight - 80,
                0
            );

            document.Close();
            return stream.ToArray();
        }
    }
}
