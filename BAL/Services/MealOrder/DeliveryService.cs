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
using DAL.Core.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using BAL.Utilities;

namespace BAL.Services.MealOrder
{
    public class DeliveryService : IDeliveryService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private IAccountManager _accountManager;
        private ApplicationDbContext _appContext;

        public DeliveryService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IAccountManager accountManager)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _accountManager = accountManager;
        }

        #region CatererInfo

        public async Task<PagedEntity<CatererInfoDTO>> GetCatererInfosAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<CatererInfoDTO>>(await this._uow.CatererInfos.GetCatererInfosAsync(filter));
            return result;
        }

        public async Task<CatererInfoDTO> GetCatererInfoByIdAsync(int id)
        {
            return Mapper.Map<CatererInfoDTO>(await this._uow.CatererInfos.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateCatererInfoAsync(CatererInfoDTO dto)
        {
            return await this._uow.CatererInfos.CreateAsync(Mapper.Map<CatererInfo>(dto));
        }

        public async Task<BaseOperationResponse> UpdateCatererInfoAsync(CatererInfoDTO dto)
        {
            return await this._uow.CatererInfos.UpdateAsync(Mapper.Map<CatererInfo>(dto));
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
            var result = Mapper.Map<PagedEntity<BentoBoxTypeDTO>>(await this._uow.BentoBoxTypes.GetBentoBoxTypesAsync(filter));
            return result;
        }

        public async Task<BentoBoxTypeDTO> GetBentoBoxTypeByIdAsync(int id)
        {
            return Mapper.Map<BentoBoxTypeDTO>(await this._uow.BentoBoxTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateBentoBoxTypeAsync(BentoBoxTypeDTO dto)
        {
            return await this._uow.BentoBoxTypes.CreateAsync(Mapper.Map<BentoBoxType>(dto));
        }

        public async Task<BaseOperationResponse> UpdateBentoBoxTypeAsync(BentoBoxTypeDetailsDTO dto)
        {
            return await this._uow.BentoBoxTypes.UpdateAsync(Mapper.Map<BentoBoxType>(dto));
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
            var result = Mapper.Map<PagedEntity<CartonTypeDTO>>(await this._uow.CartonTypes.GetCartonTypesAsync(filter));
            return result;
        }

        public async Task<CartonTypeDTO> GetCartonTypeByIdAsync(int id)
        {
            return Mapper.Map<CartonTypeDTO>(await this._uow.CartonTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateCartonTypeAsync(CartonTypeDTO dto)
        {
            return await this._uow.CartonTypes.CreateAsync(Mapper.Map<CartonType>(dto));
        }

        public async Task<BaseOperationResponse> UpdateCartonTypeAsync(CartonTypeDetailsDTO dto)
        {
            return await this._uow.CartonTypes.UpdateAsync(Mapper.Map<CartonType>(dto));
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
            var result = Mapper.Map<PagedEntity<DeliveryOrderDTO>>(await this._uow.DeliveryOrders.GetDeliveryOrdersAsync(filter));
            return result;
        }

        public async Task<DeliveryOrderDTO> GetDeliveryOrderByIdAsync(int id)
        {
            return Mapper.Map<DeliveryOrderDTO>(await this._uow.DeliveryOrders.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateDeliveryOrderAsync(DeliveryOrderDTO dto)
        {
            return await this._uow.DeliveryOrders.CreateAsync(Mapper.Map<DeliveryOrder>(dto));
        }

        public async Task<BaseOperationResponse> UpdateDeliveryOrderAsync(DeliveryOrderDTO dto)
        {
            return await this._uow.DeliveryOrders.UpdateAsync(Mapper.Map<DeliveryOrder>(dto));
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
            var result = Mapper.Map<PagedEntity<DeliveryOrderNewDTO>>(await this._uow.DeliveryOrderNews.GetDeliveryOrdersAsync(filter));
            return result;
        }

        public async Task<DeliveryOrderNewDTO> GetDeliveryOrderNewByIdAsync(int id)
        {
            return Mapper.Map<DeliveryOrderNewDTO>(await this._uow.DeliveryOrderNews.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateDeliveryOrderNewAsync(DeliveryOrderNewDTO dto)
        {
            return await this._uow.DeliveryOrderNews.CreateAsync(Mapper.Map<DeliveryOrderNew>(dto));
        }

        public async Task<BaseOperationResponse> UpdateDeliveryOrderNewAsync(DeliveryOrderNewDTO dto)
        {
            return await this._uow.DeliveryOrderNews.UpdateAsync(Mapper.Map<DeliveryOrderNew>(dto));
        }

        public async Task<BaseOperationResponse> LoadDeliveryOrderNewAsync(DeliveryOrderNewDTO dto)
        {
            return await this._uow.DeliveryOrderNews.LoadAsync(Mapper.Map<DeliveryOrderNew>(dto));
        }

        public async Task<BaseOperationResponse> DeleteDeliveryOrderNewAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.DeliveryOrderNews.DeleteAsync(id);
            return result;
        }

        #endregion


        #region StoreInventory

        public async Task<PagedEntity<StoreInventoryDTO>> GetStoreInventoriesAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<StoreInventoryDTO>>(await this._uow.StoreInventories.GetStoreInventoriesAsync(filter));
            return result;
        }

        public async Task<StoreInventoryDTO> GetStoreInventoryByIdAsync(int id)
        {
            return Mapper.Map<StoreInventoryDTO>(await this._uow.StoreInventories.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateStoreInventoryAsync(StoreInventoryDTO dto)
        {
            return await this._uow.StoreInventories.CreateAsync(Mapper.Map<StoreInventory>(dto));
        }

        public async Task<BaseOperationResponse> UpdateStoreInventoryAsync(StoreInventoryDTO dto)
        {
            return await this._uow.StoreInventories.UpdateAsync(Mapper.Map<StoreInventory>(dto));
        }

        public async Task<BaseOperationResponse> DeleteStoreInventoryAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.StoreInventories.DeleteAsync(id);
            return result;
        }

        public async Task<PagedEntity<StoreInventoryDetailDTO>> GetStoreInventoryDetailsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<StoreInventoryDetailDTO>>(await this._uow.StoreInventories.GetStoreInventoryDetailsAsync(filter));
            return result;
        }

        #endregion



        #region TrackingStatus

        public async Task<PagedEntity<TrackingStatusDTO>> GetTrackingStatussAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<TrackingStatusDTO>>(await this._uow.TrackingStatuss.GetTrackingStatussAsync(filter));
            return result;
        }

        public async Task<TrackingStatusDTO> GetTrackingStatusByIdAsync(int id)
        {
            return Mapper.Map<TrackingStatusDTO>(await this._uow.TrackingStatuss.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateTrackingStatusAsync(TrackingStatusDTO dto)
        {
            return await this._uow.TrackingStatuss.CreateAsync(Mapper.Map<TrackingStatus>(dto));
        }

        public async Task<BaseOperationResponse> UpdateTrackingStatusAsync(TrackingStatusDTO dto)
        {
            return await this._uow.TrackingStatuss.UpdateAsync(Mapper.Map<TrackingStatus>(dto));
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
            var result = Mapper.Map<PagedEntity<StoreInfoDTO>>(await this._uow.StoreInfos.GetStoreInfosAsync(filter));
            return result;
        }

        public async Task<StoreInfoDTO> GetStoreInfoByIdAsync(int id)
        {
            return Mapper.Map<StoreInfoDTO>(await this._uow.StoreInfos.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateStoreInfoAsync(StoreInfoDTO dto)
        {
            return await this._uow.StoreInfos.CreateAsync(Mapper.Map<StoreInfo>(dto));
        }

        public async Task<BaseOperationResponse> UpdateStoreInfoAsync(StoreInfoDTO dto)
        {
            return await this._uow.StoreInfos.UpdateAsync(Mapper.Map<StoreInfo>(dto));
        }

        public async Task<BaseOperationResponse> DeleteStoreInfoAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.StoreInfos.DeleteAsync(id);
            return result;
        }

        #endregion


        #region BentoAsset

        public async Task<PagedEntity<BentoAssetDTO>> GetBentoAssetsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<BentoAssetDTO>>(await this._uow.BentoAssets.GetBentoAssetsAsync(filter));
            return result;
        }

        public async Task<BentoAssetDTO> GetBentoAssetByIdAsync(int id)
        {
            return Mapper.Map<BentoAssetDTO>(await this._uow.BentoAssets.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateBentoAssetAsync(BentoAssetDTO dto)
        {
            return await this._uow.BentoAssets.CreateAsync(Mapper.Map<BentoAsset>(dto));
        }

        public async Task<BaseOperationResponse> UpdateBentoAssetAsync(BentoAssetDTO dto)
        {
            return await this._uow.BentoAssets.UpdateAsync(Mapper.Map<BentoAsset>(dto));
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

        public async Task<PagedEntity<CartonAssetDTO>> GetCartonAssetsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<CartonAssetDTO>>(await this._uow.CartonAssets.GetCartonAssetsAsync(filter));
            return result;
        }

        public async Task<CartonAssetDTO> GetCartonAssetByIdAsync(int id)
        {
            return Mapper.Map<CartonAssetDTO>(await this._uow.CartonAssets.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateCartonAssetAsync(CartonAssetDTO dto)
        {
            return await this._uow.CartonAssets.CreateAsync(Mapper.Map<CartonAsset>(dto));
        }

        public async Task<BaseOperationResponse> UpdateCartonAssetAsync(CartonAssetDTO dto)
        {
            return await this._uow.CartonAssets.UpdateAsync(Mapper.Map<CartonAsset>(dto));
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

        #endregion

        #region DisposableBox

        public async Task<PagedEntity<CartonDisposableBoxDTO>> GetDisposableBoxesAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<CartonDisposableBoxDTO>>(await this._uow.CartonDisposableBoxes.GetDisposableBoxesAsync(filter));
            return result;
        }

        public async Task<CartonDisposableBoxDTO> GetDisposableBoxByIdAsync(int id)
        {
            return Mapper.Map<CartonDisposableBoxDTO>(await this._uow.CartonDisposableBoxes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateDisposableBoxAsync(CartonDisposableBoxDTO dto)
        {
            return await this._uow.CartonDisposableBoxes.CreateAsync(Mapper.Map<CartonDisposableBox>(dto));
        }

        public async Task<BaseOperationResponse> UpdateDisposableAsync(CartonDisposableBoxDTO dto)
        {
            return await this._uow.CartonDisposableBoxes.UpdateAsync(Mapper.Map<CartonDisposableBox>(dto));
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
            result = await this._uow.CartonDisposableBoxes.ResetDishAsync(Mapper.Map<CartonDisposableBox>(dto));
            return result;
        }

        #endregion

        #region Outlet

        public async Task<PagedEntity<OutletDTO>> GetOutletsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<OutletDTO>>(await this._uow.Outlets.GetOutletsAsync(filter));
            return result;
        }

        public async Task<OutletDTO> GetOutletByIdAsync(int id)
        {
            return Mapper.Map<OutletDTO>(await this._uow.Outlets.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateOutletAsync(OutletDTO dto)
        {
            return await this._uow.Outlets.CreateAsync(Mapper.Map<Outlet>(dto));
        }

        public async Task<BaseOperationResponse> UpdateOutletAsync(OutletDTO dto)
        {
            return await this._uow.Outlets.UpdateAsync(Mapper.Map<Outlet>(dto));
        }

        public async Task<BaseOperationResponse> DeleteOutletAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Outlets.DeleteAsync(id);
            return result;
        }

        public async Task<BaseOperationResponse> UpdateStoresAsync(OutletDTO dto)
        {
            var outlet = Mapper.Map<Outlet>(dto);
            var stores = Mapper.Map<List<StoreInfo>>(dto.Stores);
            return await this._uow.Outlets.UpdateStoresAsync(outlet, stores);
        }

        #endregion

        #region OutletProfile

        public async Task<PagedEntity<OutletProfileDTO>> GetOutletProfilesAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<OutletProfileDTO>>(await this._uow.OutletProfiles.GetOutletProfilesAsync(filter));
            return result;
        }

        public async Task<OutletProfileDTO> GetOutletProfileByIdAsync(int id)
        {
            return Mapper.Map<OutletProfileDTO>(await this._uow.OutletProfiles.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateOutletProfileAsync(OutletProfileDTO dto)
        {
            return await this._uow.OutletProfiles.CreateAsync(Mapper.Map<OutletProfile>(dto));
        }

        public async Task<BaseOperationResponse> UpdateOutletProfileAsync(OutletProfileDTO dto)
        {
            return await this._uow.OutletProfiles.UpdateAsync(Mapper.Map<OutletProfile>(dto));
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
            var result = Mapper.Map<PagedEntity<DriverDTO>>(await this._uow.Drivers.GetDriversAsync(filter));
            return result;
        }

        public async Task<DriverDTO> GetDriverByIdAsync(int id)
        {
            return Mapper.Map<DriverDTO>(await this._uow.Drivers.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateDriverAsync(DriverDTO dto)
        {
            return await this._uow.Drivers.CreateAsync(Mapper.Map<Driver>(dto));
        }

        public async Task<BaseOperationResponse> UpdateDriverAsync(DriverDTO dto)
        {
            return await this._uow.Drivers.UpdateAsync(Mapper.Map<Driver>(dto));
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
            var result = Mapper.Map<PagedEntity<RouteDTO>>(await this._uow.Routes.GetRoutesAsync(filter));
            return result;
        }

        public async Task<RouteDTO> GetRouteByIdAsync(int id)
        {
            return Mapper.Map<RouteDTO>(await this._uow.Routes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateRouteAsync(RouteDTO dto)
        {
            return await this._uow.Routes.CreateAsync(Mapper.Map<Route>(dto));
        }

        public async Task<BaseOperationResponse> UpdateRouteAsync(RouteDTO dto)
        {
            var route = Mapper.Map<Route>(dto);
            var routeNodes = Mapper.Map<List<RouteNode>>(dto.Nodes);
            return await this._uow.Routes.UpdateAsync(route,routeNodes);
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
            PdfPCell cell = new PdfPCell(new Phrase(text, new Font(Font.FontFamily.HELVETICA, 10,Font.BOLD)));
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


        public void printHeader(DeliveryOrderNewDTO dOrder,Document document)
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

            table2.AddCell(getCellNoBord("SATS FOOD SERVICE PTE LTD", PdfPCell.ALIGN_LEFT));
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

            if(dInvs.PagedData.Count > 0)
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


                    printHeader(dOrder,document);
                    

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
                        c.DeliveryBentos.ForEach(b => {

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

                    dOrder.DeliveryDetails.ForEach(c => {

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
                        cartonAsset.Add(new Chunk(c.CartonAssetCode.Substring(0,c.CartonAssetCode.Length-6), normalFont));
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

                        c.DeliveryBentos.ForEach(b => {

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
                            } else
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

                        DOReports.ForEach(dd => {

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
                            if(dd.dishQty == dd.issQty)
                            {
                                i_cell_3 = new PdfPCell(new Phrase(dd.dishQty + "", new Font(Font.FontFamily.HELVETICA, 10)));
                            } else
                            {
                                notCompleted = true;
                                i_cell_3 = new PdfPCell(new Phrase(dd.dishQty + "", new Font(Font.FontFamily.HELVETICA, 10,Font.BOLD)));
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
                    float[] widths2 = new float[] { 3.4f, 7f};
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

                    if(dOrder.VehicleNumber == null || dOrder.VehicleNumber == "" || dOrder.ReceivingTime == null)
                    {
                        text = "";
                    } else if (notCompleted)
                    {
                        text = "Please note that there are item(s) that has not been issued.Please follow-up.";
                    } else
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

            var disposables = Mapper.Map<PagedEntity<CartonDisposableBoxDTO>>(await GetDisposableBoxesAsync(filterDis));

            var bentos = Mapper.Map<PagedEntity<BentoAssetDTO>>(await GetBentoAssetsAsync(filterDis));

            List<DoPrintDTO> DOReports = new List<DoPrintDTO>();

            var cartonCode = "";

            var totalQty = 0;

            if(disposables.TotalCount <= 0 && bentos.TotalCount <= 0)
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

                DOReports.ForEach(dd => {
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

                PdfPCell t_cell_1= new PdfPCell(new Phrase(" ", new Font(Font.FontFamily.HELVETICA, 10)));
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

        #endregion

        #region Outlet Term

        public async Task<PagedEntity<OutletTermDTO>> GetOutletTermsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<OutletTermDTO>>(await this._uow.OutletTerms.GetOutletTermsAsync(filter));
            return result;
        }

        public async Task<OutletTermDTO> GetOutletTermByIdAsync(int id)
        {
            return Mapper.Map<OutletTermDTO>(await this._uow.OutletTerms.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateOutletTermAsync(OutletTermDTO dto)
        {
            return await this._uow.OutletTerms.CreateAsync(Mapper.Map<OutletTerm>(dto));
        }

        public async Task<BaseOperationResponse> UpdateOutletTermAsync(OutletTermDTO dto)
        {
            return await this._uow.OutletTerms.UpdateAsync(Mapper.Map<OutletTerm>(dto));
        }

        public async Task<BaseOperationResponse> DeleteOutletTermAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.OutletTerms.DeleteAsync(id);
            return result;
        }

        #endregion
    }
}
