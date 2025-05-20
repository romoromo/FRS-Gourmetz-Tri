using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Core.DTO;
using DAL.Filters;
using DAL.Models;

namespace BAL.Services.Interfaces.MealOrder
{
    public interface IPaymentService
    {
        Task<BaseOperationResponse> CreatePaymentTypeAsync(PaymentTypeDTO dto);
        Task<BaseOperationResponse> DeletePaymentTypeAsync(int id);
        Task<PaymentTypeDTO> GetPaymentTypeByIdAsync(int id);
        Task<PagedEntity<PaymentTypeDTO>> GetPaymentTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdatePaymentTypeAsync(PaymentTypeDTO dto);

        Task<BaseOperationResponse> CreatePaymentAsync(PaymentDTO dto);
        Task<BaseOperationResponse> DeletePaymentAsync(int id);
        Task<PaymentDTO> GetPaymentByIdAsync(int id);
        Task<PagedEntity<PaymentDTO>> GetPaymentsAsync(BaseFilter filter);
        Task<List<PaymentDTO>> GetCreatedPaymentsAsync(int? studentId = null);
        Task<BaseOperationResponse> UpdatePaymentAsync(PaymentDTO dto);

        Task<BaseOperationResponse> CreateTransactionFeeAsync(TransactionFeeDTO dto);
        Task<BaseOperationResponse> DeleteTransactionFeeAsync(int id);
        Task<TransactionFeeDTO> GetTransactionFeeByIdAsync(int id);
        Task<PagedEntity<TransactionFeeDTO>> GetTransactionFeesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateTransactionFeeAsync(TransactionFeeDTO dto);

        Task<BaseOperationResponse> CreateVoucherTypeAsync(VoucherTypeDTO dto);
        Task<BaseOperationResponse> DeleteVoucherTypeAsync(int id);
        Task<VoucherTypeDTO> GetVoucherTypeByIdAsync(int id);
        Task<PagedEntity<VoucherTypeDTO>> GetVoucherTypesAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateVoucherTypeAsync(VoucherTypeDTO dto);

        Task<BaseOperationResponse> CreateVoucherAsync(VoucherDTO dto);
        Task<BaseOperationResponse> DeleteVoucherAsync(int id);
        Task<VoucherDTO> GetVoucherByIdAsync(int id);
        Task<PagedEntity<VoucherDTO>> GetVouchersAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateVoucherAsync(VoucherDTO dto);
        Task<BaseOperationResponse> ValidateVoucher(int studentId, string code);
        Task<List<VoucherDTO>> GetAllValidVoucher(int studentId);
        Task<PagedEntity<VoucherStudentLiteDTO>> GetVouchersStudentAsync(BaseFilter filter);
        Task<BaseOperationResponse> DeleteStudentVoucherAndUpdateCountVoucher(int id);

        Task<BaseOperationResponse> CreateWaiverAsync(WaiverDTO dto);
        Task<BaseOperationResponse> DeleteWaiverAsync(int id);
        Task<WaiverDTO> GetWaiverByIdAsync(int id);
        Task<PagedEntity<WaiverDTO>> GetWaiversAsync(BaseFilter filter);
        Task<BaseOperationResponse> UpdateWaiverAsync(WaiverDTO dto);
    }
}