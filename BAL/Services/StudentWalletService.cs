using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Core;
using Sieve.Services;
using DAL;
using BAL.Services.Interfaces;
using AutoMapper;
using BAL.DTO.MealOrder;
using DAL.Filters;

namespace BAL.Services
{
    public class StudentWalletService : IStudentWalletService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public StudentWalletService(ISieveProcessor sieveProcessor, IUnitOfWork uow, IMapper mapper)
        {
            _sieveProcessor = sieveProcessor;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<PagedEntity<StudentWalletTransactionDTO>> GetWalletTransactionsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<StudentWalletTransactionDTO>>(await this._uow.StudentWalletTransactions.GetWalletTransactionsAsync(filter));
            return result;
        }

        public async Task<List<StudentWalletTransactionDTO>> GetWalletTransactionByIdAsync(int id)
        {
            return _mapper.Map<List<StudentWalletTransactionDTO>>((await this._uow.StudentWalletTransactions.FindAsync(e => e.StudentId == id)).ToList());
        }

        public async Task<BaseOperationResponse> StudentWalletTransactionAsync(StudentWalletTransactionDTO dto)
        {
            var result = new BaseOperationResponse();
            var student = await this._uow.Students.GetByIdAsync(dto.StudentId);

            //if (!student.ConcurrencyStamp.SequenceEqual(dto.ConcurrencyStamp))
            //{
            //    result.IsSuccess = false;
            //    result.Message = "Student is not the latest version. Please refresh.";
            //    return result;
            //}
            //else
            //{
                if (string.IsNullOrEmpty(dto.TransactionType) ||
                    (!dto.TransactionType.Equals(RewardTransactionType.CREDIT.ToString(), StringComparison.OrdinalIgnoreCase) &&
                    !dto.TransactionType.Equals(RewardTransactionType.DEBIT.ToString(), StringComparison.OrdinalIgnoreCase)))
                {
                    result.IsSuccess = false;
                    result.Message = "Transaction type is missing or invalid.";
                    return result;
                }

                if (dto.TransactionType.Equals(WalletTransactionType.CREDIT.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    student.WalletBalance += dto.Amount;
                }
                else if (dto.TransactionType.Equals(WalletTransactionType.DEBIT.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    if (student.WalletBalance - dto.Amount < 0)
                    {
                        result.IsSuccess = false;
                        result.Message = "Insufficient balance.";
                        return result;
                    } 
                    else if (student.IsWalletFreeze)
                    {
                        result.IsSuccess = false;
                        result.Message = "Wallet is Freezed";
                        return result;
                    }
                    else
                    {
                    student.WalletBalance -= dto.Amount;
                    }
                }

                result = await this._uow.Students.UpdateAsync(student);

                if (result.IsSuccess)
                {
                    var transaction = new StudentWalletTransaction
                    {
                        Amount = dto.Amount,
                        TransactionType = dto.TransactionType,
                        StudentId = dto.StudentId,
                        Description = dto.Description
                    };

                    await this._uow.StudentWalletTransactions.CreateAsync(transaction);
                }

                var d = _mapper.Map<StudentDTO>(result.Data);
                result.Data = d;
            //}

            return result;
        }

        public async Task<BaseOperationResponse> TopupWalletBalanceByStudentGroupIdAsync(int studentGroupId, double amount, int userId, WalletType walletTypeData)
        {
            return await this._uow.StudentWalletTransactions.TopupWalletBalanceByStudentGroupIdAsync(studentGroupId, amount, userId,walletTypeData);
        }

        public async Task<BaseOperationResponse> TopupWalletBalanceByStudentIdAsync(int studentId, double amount, int userId, WalletType walletType)
        {
            return await this._uow.StudentWalletTransactions.TopupWalletBalanceByStudentIdAsync(studentId, amount, userId,walletType);
        }

        public async Task<BaseOperationResponse> RefundToWalletBalanceAsync(int studentId, double amount, int userId, WalletType walletType)
        {
            return await this._uow.StudentWalletTransactions.RefundToWalletBalanceAsync(studentId, amount, userId, walletType);
        }

        public async Task<BaseOperationResponse> OffBoardingStudent(int studentId, int userId)
        {
            return await this._uow.StudentWalletTransactions.OffBoardingStudent(studentId, userId);
        }
    }
}
