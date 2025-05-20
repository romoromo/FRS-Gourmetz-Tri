using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;
using DAL;
using AutoMapper;
using BAL.Services.Interfaces.MealOrder;
using BAL.DTO.MealOrder;
using DAL.Models.MealOrder;
using DAL.Core.DTO;

namespace BAL.Services.MealOrder
{
    public class PaymentService : IPaymentService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork uow, ISieveProcessor sieveProcessor, IMapper mapper)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
            _mapper = mapper;
        }

        #region Payment Types

        public async Task<PagedEntity<PaymentTypeDTO>> GetPaymentTypesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<PaymentTypeDTO>>(await this._uow.PaymentTypes.GetPaymentTypesAsync(filter));
            return result;
        }

        public async Task<PaymentTypeDTO> GetPaymentTypeByIdAsync(int id)
        {
            return _mapper.Map<PaymentTypeDTO>(await this._uow.PaymentTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreatePaymentTypeAsync(PaymentTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.PaymentTypes.CreateAsync(_mapper.Map<PaymentType>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdatePaymentTypeAsync(PaymentTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.PaymentTypes.UpdateAsync(_mapper.Map<PaymentType>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeletePaymentTypeAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.PaymentTypes.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Payment

        public async Task<PagedEntity<PaymentDTO>> GetPaymentsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<PaymentDTO>>(await this._uow.Payments.GetPaymentsAsync(filter));
            return result;
        }

        public async Task<List<PaymentDTO>> GetCreatedPaymentsAsync(int? studentId = null)
        {
            var result = _mapper.Map<List<PaymentDTO>>(await this._uow.Payments.GetCreatedPaymentsAsync(studentId));
            return result;
        }

        public async Task<PaymentDTO> GetPaymentByIdAsync(int id)
        {
            return _mapper.Map<PaymentDTO>(await this._uow.Payments.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreatePaymentAsync(PaymentDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Payments.CreateAsync(_mapper.Map<Payment>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdatePaymentAsync(PaymentDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Payments.UpdateAsync(_mapper.Map<Payment>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeletePaymentAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Payments.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Transaction Fees

        public async Task<PagedEntity<TransactionFeeDTO>> GetTransactionFeesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<TransactionFeeDTO>>(await this._uow.TransactionFees.GetTransactionFeesAsync(filter));
            return result;
        }

        public async Task<TransactionFeeDTO> GetTransactionFeeByIdAsync(int id)
        {
            return _mapper.Map<TransactionFeeDTO>(await this._uow.TransactionFees.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateTransactionFeeAsync(TransactionFeeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.TransactionFees.CreateAsync(_mapper.Map<TransactionFee>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateTransactionFeeAsync(TransactionFeeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.TransactionFees.UpdateAsync(_mapper.Map<TransactionFee>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteTransactionFeeAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.TransactionFees.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Voucher Types

        public async Task<PagedEntity<VoucherTypeDTO>> GetVoucherTypesAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<VoucherTypeDTO>>(await this._uow.VoucherTypes.GetVoucherTypesAsync(filter));
            return result;
        }

        public async Task<VoucherTypeDTO> GetVoucherTypeByIdAsync(int id)
        {
            return _mapper.Map<VoucherTypeDTO>(await this._uow.VoucherTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateVoucherTypeAsync(VoucherTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.VoucherTypes.CreateAsync(_mapper.Map<VoucherType>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateVoucherTypeAsync(VoucherTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.VoucherTypes.UpdateAsync(_mapper.Map<VoucherType>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteVoucherTypeAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.VoucherTypes.DeleteAsync(id);
            return result;
        }

        #endregion

        #region Vouchers

        public async Task<PagedEntity<VoucherDTO>> GetVouchersAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<VoucherDTO>>(await this._uow.Vouchers.GetVouchersAsync(filter));
            return result;
        }

        public async Task<VoucherDTO> GetVoucherByIdAsync(int id)
        {
            return _mapper.Map<VoucherDTO>(await this._uow.Vouchers.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateVoucherAsync(VoucherDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Vouchers.CreateAsync(_mapper.Map<Voucher>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateVoucherAsync(VoucherDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Vouchers.UpdateAsync(_mapper.Map<Voucher>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteVoucherAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Vouchers.DeleteAsync(id);
            return result;
        }

        public async Task<BaseOperationResponse> ValidateVoucher(int studentId, string code)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Vouchers.ValidateVoucher(studentId, code);
            return result;
        }

        public async Task<List<VoucherDTO>> GetAllValidVoucher(int studentId)
        {
            var result = _mapper.Map<List<VoucherDTO>>(await this._uow.Vouchers.GetAllValidVoucher(studentId));
            return result;
        }

        public async Task<PagedEntity<VoucherStudentLiteDTO>> GetVouchersStudentAsync(BaseFilter filter)
        {
            var result = await _uow.Vouchers.GetVouchersStudentAsync(filter);
            return result;
        }

        public async Task<BaseOperationResponse> DeleteStudentVoucherAndUpdateCountVoucher(int id)
        {
            var result = await _uow.Vouchers.DeleteStudentVoucherAndUpdateCountVoucher(id);
            return result;
        }

        #endregion

        #region Waivers

        public async Task<PagedEntity<WaiverDTO>> GetWaiversAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<WaiverDTO>>(await this._uow.Waivers.GetWaiversAsync(filter));
            return result;
        }

        public async Task<WaiverDTO> GetWaiverByIdAsync(int id)
        {
            return _mapper.Map<WaiverDTO>(await this._uow.Waivers.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateWaiverAsync(WaiverDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Waivers.CreateAsync(_mapper.Map<Waiver>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateWaiverAsync(WaiverDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Waivers.UpdateAsync(_mapper.Map<Waiver>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> DeleteWaiverAsync(int id)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Waivers.DeleteAsync(id);
            return result;
        }

        #endregion
    }
}
