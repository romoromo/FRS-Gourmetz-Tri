using AutoMapper;
using BAL.DTO.MealOrder;
using BAL.Services.Interfaces.MealOrder;
using BAL.Utilities;
using DAL;
using DAL.Core;
using DAL.Core.Logging;
using DAL.Filters;
using DAL.Models;
using DAL.Models.MealOrder;
using Microsoft.Extensions.Logging;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services.MealOrder
{
    public class OrderPortalService : IOrderPortalService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private ILogger _logger;

        public OrderPortalService(IUnitOfWork uow, ISieveProcessor sieveProcessor)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _logger = Utilities.Logger.CreateLogger<OrderPortalService>();
        }

        public async Task<PagedEntity<OrderPortalContentDTO>> GetOrderPortalContentsAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<OrderPortalContentDTO>>(await this._uow.OrderPortalContents.GetOrderPortalContentsAsync(filter));
            return result;
        }

        public async Task<OrderPortalContentDTO> GetOrderPortalContentByIdAsync(int id)
        {
            return Mapper.Map<OrderPortalContentDTO>(await this._uow.OrderPortalContents.GetByIdAsync(id));
        }

        public async Task<OrderPortalContentDTO> GetOrderPortalContentFirst(int outletId)
        {
            return Mapper.Map<OrderPortalContentDTO>(await this._uow.OrderPortalContents.GetOrderPortalContentFirst(outletId));
        }

        public async Task<BaseOperationResponse> CreateOrderPortalContentAsync(OrderPortalContentDTO dto)
        {
            dto = GetOrderPortalContentWithFile(dto);
            return await this._uow.OrderPortalContents.CreateAsync(Mapper.Map<OrderPortalContent>(dto));
        }

        public async Task<BaseOperationResponse> UpdateOrderPortalContentAsync(OrderPortalContentDTO dto)
        {
            dto = GetOrderPortalContentWithFile(dto);
            return await this._uow.OrderPortalContents.UpdateAsync(Mapper.Map<OrderPortalContent>(dto));
        }

        public async Task<BaseOperationResponse> DeleteOrderPortalContentAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.OrderPortalContents.DeleteAsync(id);
            return result;
        }

        private OrderPortalContentDTO GetOrderPortalContentWithFile(OrderPortalContentDTO dto)
        {
            foreach (var banner in dto.Banners)
            {
                if (!string.IsNullOrEmpty(banner.FilePath))
                {

                    try
                    {
                        string source = Path.Combine(Directory.GetCurrentDirectory(), banner.FilePath);
                        //string ext = Path.GetExtension(source);
                        string fname = Path.GetFileName(source);
                        var relativePath = Path.Combine("Resources", "Images", "OrderPortalContent");
                        string destinationFolder = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
                        if (!Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);

                        var relativeFilePath = Path.Combine(relativePath, fname);
                        var destination = Path.Combine(Directory.GetCurrentDirectory(), relativeFilePath);
                        var sourceFile = new FileInfo(source);
                        sourceFile.MoveTo(destination);
                        //System.IO.File.Copy(source, destination, true);

                        banner.FilePath = relativeFilePath;
                        banner.FileName = fname;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(LoggingEvents.APPLICATION_ERROR, ex, "An error occurred while getting the OrderPortalContent file: " + banner.FilePath);
                    }
                }

            }
            return dto;
        }
    }
}
