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

namespace BAL.Services.MealOrder
{
    public class PaymentService : IPaymentService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;

        public PaymentService(IUnitOfWork uow, ISieveProcessor sieveProcessor)
        {
            this._sieveProcessor = sieveProcessor;
            this._uow = uow;
        }

        #region Payment Types

        public async Task<PagedEntity<PaymentTypeDTO>> GetPaymentTypesAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<PaymentTypeDTO>>(await this._uow.PaymentTypes.GetPaymentTypesAsync(filter));
            return result;
        }

        public async Task<PaymentTypeDTO> GetPaymentTypeByIdAsync(int id)
        {
            return Mapper.Map<PaymentTypeDTO>(await this._uow.PaymentTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreatePaymentTypeAsync(PaymentTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.PaymentTypes.CreateAsync(Mapper.Map<PaymentType>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdatePaymentTypeAsync(PaymentTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.PaymentTypes.UpdateAsync(Mapper.Map<PaymentType>(dto));
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
            var result = Mapper.Map<PagedEntity<PaymentDTO>>(await this._uow.Payments.GetPaymentsAsync(filter));
            return result;
        }

        public async Task<List<PaymentDTO>> GetCreatedPaymentsAsync(int? studentId = null)
        {
            var result = Mapper.Map<List<PaymentDTO>>(await this._uow.Payments.GetCreatedPaymentsAsync(studentId));
            return result;
        }

        public async Task<PaymentDTO> GetPaymentByIdAsync(int id)
        {
            return Mapper.Map<PaymentDTO>(await this._uow.Payments.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreatePaymentAsync(PaymentDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Payments.CreateAsync(Mapper.Map<Payment>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdatePaymentAsync(PaymentDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Payments.UpdateAsync(Mapper.Map<Payment>(dto));
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
            var result = Mapper.Map<PagedEntity<TransactionFeeDTO>>(await this._uow.TransactionFees.GetTransactionFeesAsync(filter));
            return result;
        }

        public async Task<TransactionFeeDTO> GetTransactionFeeByIdAsync(int id)
        {
            return Mapper.Map<TransactionFeeDTO>(await this._uow.TransactionFees.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateTransactionFeeAsync(TransactionFeeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.TransactionFees.CreateAsync(Mapper.Map<TransactionFee>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateTransactionFeeAsync(TransactionFeeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.TransactionFees.UpdateAsync(Mapper.Map<TransactionFee>(dto));
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
            var result = Mapper.Map<PagedEntity<VoucherTypeDTO>>(await this._uow.VoucherTypes.GetVoucherTypesAsync(filter));
            return result;
        }

        public async Task<VoucherTypeDTO> GetVoucherTypeByIdAsync(int id)
        {
            return Mapper.Map<VoucherTypeDTO>(await this._uow.VoucherTypes.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateVoucherTypeAsync(VoucherTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.VoucherTypes.CreateAsync(Mapper.Map<VoucherType>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateVoucherTypeAsync(VoucherTypeDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.VoucherTypes.UpdateAsync(Mapper.Map<VoucherType>(dto));
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
            var result = Mapper.Map<PagedEntity<VoucherDTO>>(await this._uow.Vouchers.GetVouchersAsync(filter));
            return result;
        }

        public async Task<VoucherDTO> GetVoucherByIdAsync(int id)
        {
            return Mapper.Map<VoucherDTO>(await this._uow.Vouchers.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateVoucherAsync(VoucherDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Vouchers.CreateAsync(Mapper.Map<Voucher>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateVoucherAsync(VoucherDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Vouchers.UpdateAsync(Mapper.Map<Voucher>(dto));
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
            var result = Mapper.Map<List<VoucherDTO>>(await this._uow.Vouchers.GetAllValidVoucher(studentId));
            return result;
        }

        #endregion

        #region Waivers

        public async Task<PagedEntity<WaiverDTO>> GetWaiversAsync(BaseFilter filter)
        {
            var result = Mapper.Map<PagedEntity<WaiverDTO>>(await this._uow.Waivers.GetWaiversAsync(filter));
            return result;
        }

        public async Task<WaiverDTO> GetWaiverByIdAsync(int id)
        {
            return Mapper.Map<WaiverDTO>(await this._uow.Waivers.GetByIdAsync(id));
        }

        public async Task<BaseOperationResponse> CreateWaiverAsync(WaiverDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Waivers.CreateAsync(Mapper.Map<Waiver>(dto));
            return result;
        }

        public async Task<BaseOperationResponse> UpdateWaiverAsync(WaiverDTO dto)
        {
            var result = new BaseOperationResponse();
            result = await this._uow.Waivers.UpdateAsync(Mapper.Map<Waiver>(dto));
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
