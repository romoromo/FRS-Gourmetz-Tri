using iTextSharp.text;
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

        public static BaseColor ParseHexColor(string hex)
        {
            hex = hex.Replace("#", "").Trim();

            if (hex.Length != 6)
                return BaseColor.WHITE;

            try
            {
                int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                int b = Convert.ToInt32(hex.Substring(4, 2), 16);

                return new BaseColor(r, g, b);
            }
            catch
            {
                return BaseColor.WHITE;
            }
        }

        public static string Round(double value)
        {
            if (value == 0) return "0";
            return value.ToString("N2");
        }
    }
}
