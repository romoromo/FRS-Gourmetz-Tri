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
using NPOI.SS.Formula.Functions;

namespace BAL.Services
{
    public class ImageReferenceColorService : IImageReferenceColorService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ImageReferenceColorService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _mapper = mapper;
        }

        public async Task<PagedEntity<ImageReferenceColorDTO>> GetImageReferenceColorsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<ImageReferenceColorDTO>>(await this._uow.ImageReferenceColors.GetImageReferenceColorsAsync(filter));
            return result;
        }

        public async Task<ImageReferenceColorDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<ImageReferenceColorDTO>(await this._uow.ImageReferenceColors.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateAsync(ImageReferenceColorDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ImageReferenceColors.CreateAsync(_mapper.Map<ImageReferenceColor>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateAsync(ImageReferenceColorDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ImageReferenceColors.UpdateAsync(_mapper.Map<ImageReferenceColor>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ImageReferenceColors.DeleteAsync(id);
            return result;
        }
    }
}
