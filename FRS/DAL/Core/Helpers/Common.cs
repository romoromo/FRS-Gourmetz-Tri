using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Core.Helpers
{
    public static class Common
    {
        public static string GenerateUniqueStringId()
        {
            string uniqueId = $"{DateTime.Now:yyyyMMddHHmmssfff}-{new Random().Next(1000, 9999)}";
            return uniqueId;
        }
    }
}
