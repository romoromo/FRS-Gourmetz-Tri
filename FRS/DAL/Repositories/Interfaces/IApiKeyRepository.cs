using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IApiKeyManager
    {
        Task<bool> IsAuthorized(string clientId, string apiKey);
    }
}
