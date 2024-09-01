using Sieve.Services;
using DAL;
using BAL.Services.Interfaces.MealOrder;
using DAL.Core;
using BAL.DTO.MealOrder;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Models;
using DAL.Filters;
using DAL.Models.MealOrder;
using BAL.Services.Interfaces;

namespace BAL.Services.MealOrder
{
    public class ManagementService : IManagementService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ManagementService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _mapper = mapper;
        }

        #region Faq Subject
        public async Task<PagedEntity<FaqSubjectDTO>> GetFaqSubjectsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<FaqSubjectDTO>>(await this._uow.FaqSubjects.GetFaqSubjectsAsync(filter));
            return result;
        }

        public async Task<FaqSubjectDTO> GetFaqSubjectByIdAsync(int id)
        {
            return _mapper.Map<FaqSubjectDTO>(await this._uow.FaqSubjects.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateFaqSubjectAsync(FaqSubjectDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.FaqSubjects.CreateAsync(_mapper.Map<FaqSubject>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateFaqSubjectAsync(FaqSubjectDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.FaqSubjects.UpdateAsync(_mapper.Map<FaqSubject>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteFaqSubjectAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.FaqSubjects.DeleteAsync(id);
            return result;
        }

        public async Task<BaseOperationResponse> OrderFaqSubjectAsync(int id, bool isAsc)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.FaqSubjects.OrderAsync(id, isAsc);
            return result;
        }

        #endregion

        #region Faq Detail
        public async Task<PagedEntity<FaqDetailDTO>> GetFaqDetailsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<FaqDetailDTO>>(await this._uow.FaqDetails.GetFaqDetailsAsync(filter));
            return result;
        }

        public async Task<FaqDetailDTO> GetFaqDetailByIdAsync(int id)
        {
            return _mapper.Map<FaqDetailDTO>(await this._uow.FaqDetails.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateFaqDetailAsync(FaqDetailDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.FaqDetails.CreateAsync(_mapper.Map<FaqDetail>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateFaqDetailAsync(FaqDetailDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.FaqDetails.UpdateAsync(_mapper.Map<FaqDetail>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteFaqDetailAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.FaqDetails.DeleteAsync(id);
            return result;
        }
        public async Task<BaseOperationResponse> OrderFaqDetailAsync(int id, bool isAsc)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.FaqDetails.OrderAsync(id, isAsc);
            return result;
        }

        #endregion
    }
}
