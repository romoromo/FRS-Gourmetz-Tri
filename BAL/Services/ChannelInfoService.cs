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
    public class ChannelInfoService : IChannelInfoService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ChannelInfoService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _mapper = mapper;
        }

        #region Channel Info

        public async Task<PagedEntity<ChannelInfoDTO>> GetChannelInfosAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<ChannelInfoDTO>>(await this._uow.ChannelInfos.GetChannelInfosAsync(filter));
            return result;
        }

        public async Task<ChannelInfoDTO> GetChannelInfoByIdAsync(int id)
        {
            return _mapper.Map<ChannelInfoDTO>(await this._uow.ChannelInfos.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateChannelInfoAsync(ChannelInfoDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ChannelInfos.CreateAsync(_mapper.Map<ChannelInfo>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateChannelInfoAsync(ChannelInfoDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ChannelInfos.UpdateAsync(_mapper.Map<ChannelInfo>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteChannelInfoAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ChannelInfos.DeleteAsync(id);
            return result;
        }

        #endregion
        
    }
}
