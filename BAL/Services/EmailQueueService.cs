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
using BAL.DTO.MealOrder;
using DAL.Models.MealOrder;

namespace BAL.Services
{
    public class EmailQueueService : IEmailQueueService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public EmailQueueService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _mapper = mapper;
        }

        public async Task<PagedEntity<EmailQueueDTO>> GetEmailQueuesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<EmailQueueDTO>>(await this._uow.EmailQueues.GetEmailQueuesAsync(filter));
            return result;
        }

        public async Task<EmailQueueDTO> GetEmailQueueByIdAsync(int id)
        {
            return _mapper.Map<EmailQueueDTO>(await this._uow.EmailQueues.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateEmailQueueAsync(EmailQueueDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.EmailQueues.CreateAsync(_mapper.Map<EmailQueue>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateEmailQueueAsync(EmailQueueDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.EmailQueues.UpdateAsync(_mapper.Map<EmailQueue>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteEmailQueueAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.EmailQueues.DeleteAsync(id);
            return result;
        }

        #region Contact Us Subject
        public async Task<PagedEntity<ContactUsSubjectDTO>> GetContactUsSubjectsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<ContactUsSubjectDTO>>(await this._uow.ContactUsSubjects.GetContactUsSubjectsAsync(filter));
            return result;
        }

        public async Task<ContactUsSubjectDTO> GetContactUsSubjectByIdAsync(int id)
        {
            return _mapper.Map<ContactUsSubjectDTO>(await this._uow.ContactUsSubjects.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateContactUsSubjectAsync(ContactUsSubjectDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ContactUsSubjects.CreateAsync(_mapper.Map<ContactUsSubject>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateContactUsSubjectAsync(ContactUsSubjectDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ContactUsSubjects.UpdateAsync(_mapper.Map<ContactUsSubject>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteContactUsSubjectAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ContactUsSubjects.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Contact Us Detail
        public async Task<PagedEntity<ContactUsDetailDTO>> GetContactUsDetailsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<ContactUsDetailDTO>>(await this._uow.ContactUsDetails.GetContactUsDetailsAsync(filter));
            return result;
        }

        public async Task<ContactUsDetailDTO> GetContactUsDetailByIdAsync(int id)
        {
            return _mapper.Map<ContactUsDetailDTO>(await this._uow.ContactUsDetails.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateContactUsDetailAsync(ContactUsDetailDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ContactUsDetails.CreateAsync(_mapper.Map<ContactUsDetail>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateContactUsDetailAsync(ContactUsDetailDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ContactUsDetails.UpdateAsync(_mapper.Map<ContactUsDetail>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteContactUsDetailAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.ContactUsDetails.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Email Template
        public async Task<PagedEntity<EmailTemplateDTO>> GetEmailTemplatesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<EmailTemplateDTO>>(await this._uow.EmailTemplates.GetEmailTemplatesAsync(filter));
            return result;
        }

        public async Task<EmailTemplateDTO> GetEmailTemplateByIdAsync(int id)
        {
            return _mapper.Map<EmailTemplateDTO>(await this._uow.EmailTemplates.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateEmailTemplateAsync(EmailTemplateDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.EmailTemplates.CreateAsync(_mapper.Map<EmailTemplate>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateEmailTemplateAsync(EmailTemplateDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.EmailTemplates.UpdateAsync(_mapper.Map<EmailTemplate>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteEmailTemplateAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.EmailTemplates.DeleteAsync(id);
            return result;
        }

        #endregion
    }
}
