using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Core
{
    public class ApiKeyManager : IApiKeyManager
    {
        public async Task<bool> IsAuthorized(string clientId, string apiKey)
        {
            var flag = true;
            return flag;
        }
    }
}
