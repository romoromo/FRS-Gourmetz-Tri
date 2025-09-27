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
using BAL.DTO.MealOrder;

namespace BAL.Services
{
    public class StudentPointService : IStudentPointService
    {
        private ISieveProcessor _sieveProcessor;
        private IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public async Task<PagedEntity<StudentPointTransactionDTO>> GetPointTransactionsAsync(BaseFilter filter)
        {
            var result = _mapper.Map<PagedEntity<StudentPointTransactionDTO>>(await this._uow.StudentPointTransactions.GetPointTransactionsAsync(filter));
            return result;
        }

        public async Task<List<StudentPointTransactionDTO>> GetPointTransactionByIdAsync(int id)
        {
            return _mapper.Map<List<StudentPointTransactionDTO>>((await this._uow.StudentPointTransactions.FindAsync(e => e.StudentId == id)).ToList());
        }

        public async Task<BaseOperationResponse> StudentPointTransactionAsync(StudentPointTransactionDTO dto)
        {
            var result = new BaseOperationResponse();
            var student = await this._uow.Students.GetByIdAsync(dto.StudentId);

            if (!student.ConcurrencyStamp.SequenceEqual(dto.ConcurrencyStamp))
            {
                result.IsSuccess = false;
                result.Message = "Student is not the latest version. Please refresh.";
                return result;
            }
            else
            {
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
                    student.PointBalance += dto.Amount;
                }
                else if (dto.TransactionType.Equals(WalletTransactionType.DEBIT.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    if (student.PointBalance - dto.Amount < 0)
                    {
                        result.IsSuccess = false;
                        result.Message = "Insufficient balance.";
                        return result;
                    }
                    else
                    {
                        student.PointBalance -= dto.Amount;
                    }
                }

                result = await this._uow.Students.UpdateAsync(student);

                if (result.IsSuccess)
                {
                    var transaction = new StudentPointTransaction
                    {
                        Amount = dto.Amount,
                        TransactionType = dto.TransactionType,
                        StudentId = dto.StudentId,
                        Description = dto.Description
                    };

                    await this._uow.StudentPointTransactions.CreateAsync(transaction);
                }

                var d = _mapper.Map<StudentDTO>(result.Data);
                result.Data = d;
            }

            return result;
        }
    }
}
