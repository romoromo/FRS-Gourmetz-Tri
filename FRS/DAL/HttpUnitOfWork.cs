using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.Extensions.Configuration;
using Sieve.Services;
using Microsoft.Extensions.Logging;
using DAL.Core.Interfaces;

namespace DAL
{
    public class HttpUnitOfWork : UnitOfWork
    {
        public HttpUnitOfWork(IConfiguration configuration, ApplicationDbContext context, IHttpContextAccessor httpAccessor, ISieveProcessor sieveProcessor, ILoggerFactory loggerFactory) : base(configuration, context, sieveProcessor, loggerFactory)
        {
            if (httpAccessor.HttpContext != null && httpAccessor.HttpContext.User != null)
            {
                context.CurrentUserId = Convert.ToInt32(httpAccessor.HttpContext.User.FindFirst(OpenIdConnectConstants.Claims.Subject)?.Value?.Trim());
                CurrentUserId = context.CurrentUserId;
            }
        }
    }
}
