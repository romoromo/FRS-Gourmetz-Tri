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

namespace BAL.Services
{
    public class ImageReferenceTypeService : IImageReferenceTypeService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;

        public ImageReferenceTypeService(IUnitOfWork uow, ISieveProcessor sieveProcessor)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
        }

        public async Task<PagedEntity<ImageReferenceTypeDTO>> GetImageReferenceTypesAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<ImageReferenceTypeDTO>>(await this._uow.ImageReferenceTypes.GetImageReferenceTypesAsync(filter));
            return result;
        }

        public async Task<ImageReferenceTypeDTO> GetByIdAsync(int id)
        {
            return Mapper.Map<ImageReferenceTypeDTO>(await this._uow.ImageReferenceTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateAsync(ImageReferenceTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ImageReferenceTypes.CreateAsync(Mapper.Map<ImageReferenceType>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(ImageReferenceTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ImageReferenceTypes.UpdateAsync(Mapper.Map<ImageReferenceType>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ImageReferenceTypes.DeleteAsync(id);
            return result;
        }
    }
}
