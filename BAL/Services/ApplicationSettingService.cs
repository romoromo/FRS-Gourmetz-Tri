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
    public class ApplicationSettingService : IApplicationSettingService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;

        public ApplicationSettingService(IUnitOfWork uow, ISieveProcessor sieveProcessor)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
        }

        public async Task<ApplicationSettingDTO> GetApplicationSettingByKey(string key)
        {
            var result = Mapper.Map<ApplicationSettingDTO>(await this._uow.ApplicationSettings.GetByKeyAsync(key));
            return result;
        }
    }
}
