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
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Drawing;
using NPOI.HSSF.Util;
using NPOI.SS.Util;

namespace BAL.Services
{
    public class ConnectionService : IConnectionService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ConnectionService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _mapper = mapper;
        }

        public async Task<ConnectionDTO> GetConnectionStatus(int deviceId)
        {
            var result = _mapper.Map<ConnectionDTO>(await this._uow.Connections.GetFirstOrDefaultAsync(e => e.Identifier == deviceId.ToString()));
            return result;
        }
    }
}
