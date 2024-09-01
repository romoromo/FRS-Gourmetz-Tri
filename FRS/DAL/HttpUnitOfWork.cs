using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Sieve.Services;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using AutoMapper;

namespace DAL
{
    public class HttpUnitOfWork : UnitOfWork
    {
        public HttpUnitOfWork(IConfiguration configuration, ApplicationDbContext context, IHttpContextAccessor httpAccessor, ISieveProcessor sieveProcessor, ILoggerFactory loggerFactory, IMapper mapper) : base(configuration, context, sieveProcessor, loggerFactory, mapper)
        {
            if (httpAccessor.HttpContext != null && httpAccessor.HttpContext.User != null)
            {
                context.CurrentUserId = Convert.ToInt32(httpAccessor.HttpContext.User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value?.Trim());
                CurrentUserId = context.CurrentUserId;
            }
        }
    }
}
