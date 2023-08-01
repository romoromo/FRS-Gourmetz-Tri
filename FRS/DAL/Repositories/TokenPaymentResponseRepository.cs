using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Core;
using Sieve.Services;
using DAL.Filters;

namespace DAL.Repositories
{
    public class TokenPaymentResponseRepository : Repository<TokenPaymentResponse>, ITokenPaymentResponseRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;

        public TokenPaymentResponseRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }


        public async Task<BaseOperationResponse> CreateAsync(TokenPaymentResponse paymentResponse)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(paymentResponse);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                long insertedNewRecordID = paymentResponse.PaymentResponseId;

                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                // result.Data = f;  // returns the object paymentRequest
                result.Data = insertedNewRecordID;
 
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<TokenPaymentResponse> GetByIdAsync(string id)
        {
          //  return await this._appContext.TokenPaymentResponse.FindAsync(id);

            return await this._appContext.TokenPaymentResponse.SingleOrDefaultAsync(e => e.PaymentTransactionId == id);
        }
        

        public async Task<BaseOperationResponse> UpdateAsync(string transacionOrderId,string UpdateStatus)
        {
            var result = new BaseOperationResponse();
          
            var f = await this._appContext.TokenPaymentResponse.SingleOrDefaultAsync(e => e.PaymentTransactionId == transacionOrderId);
         
            // We just need to update 1 field.
            f.StatusCode = UpdateStatus;
           //   f.CopyFrom(updatePaymentResponse); // we dont need to updata entire tbl just 1 field.

            Update(f);

            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }


        public async Task<BaseOperationResponse> UpdateAsync(TokenPaymentUpdate updatePaymentResponse)
        {
            var result = new BaseOperationResponse();

            var f = await GetSingleOrDefaultAsync(e => e.PaymentTransactionId == updatePaymentResponse.PaymentTransactionId);

            f.CopyFrom(updatePaymentResponse);

            Update(f);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                result.Message = "Successfully saved!";
                result.IsSuccess = true;
                result.Data = f;
            }
            else
            {
                result.Message = "Failed to save!";
                result.IsSuccess = false;
            }

            return result;
        }

 
        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
