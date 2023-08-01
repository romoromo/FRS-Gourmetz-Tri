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
    public class TokenPaymentRepository : Repository<TokenPaymentRequest>, ITokenPaymentRepository
    {
        private ISieveProcessor _sieveProcessor;
        private int? _currentUserId;
        private int? _currentInstitutionId;

        public TokenPaymentRepository(ApplicationDbContext context, ISieveProcessor sieveProcessor, int? currentUserId, int? currentInstitutionId) : base(context)
        {
            this._sieveProcessor = sieveProcessor;
            this._currentInstitutionId = currentInstitutionId;
            this._currentUserId = currentInstitutionId;
        }


        public async Task<BaseOperationResponse> CreateAsync(TokenPaymentRequest paymentRequest)
        {
            var result = new BaseOperationResponse();

            var f = await AddAsync(paymentRequest);
            if (await _appContext.SaveChangesAsync() > 0)
            {
                long insertedNewRecordID = paymentRequest.PaymentRequestId;

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

 

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
