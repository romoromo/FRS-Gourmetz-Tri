using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DAL.Core.Helpers
{
    public static class WalletDescriptionHelper
    {
        private static readonly Regex BalanceChangeRegex =
        new Regex(@"\b(?<label>FAS|Normal)\s*:\s*\$(?<before>[0-9]+(?:\.[0-9]+)?)\s*->\s*\$(?<after>[0-9]+(?:\.[0-9]+)?)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsTopUpBasic(string desc)
            => desc?.Contains("Top-up BASIC wallet", StringComparison.OrdinalIgnoreCase) == true;

        public static bool IsTopUpFAS(string desc)
            => (desc?.Contains("Top-up FAS wallet", StringComparison.OrdinalIgnoreCase) == true
               || desc?.Contains("Auto Credit FAS", StringComparison.OrdinalIgnoreCase) == true && (!desc?.Contains("at 2026-01-17 17:00") == true));

        public static bool IsRefundBasic(string desc)
            => (desc?.Contains("Refund to BASIC", StringComparison.OrdinalIgnoreCase) == true
               && desc.Contains("Old Balance", StringComparison.OrdinalIgnoreCase)) ||
            desc?.Contains("Refund to Wallet", StringComparison.OrdinalIgnoreCase) == true
               && desc.Contains("Old Basic Balance", StringComparison.OrdinalIgnoreCase);

        public static bool IsRefundFAS(string desc)
            => desc?.Contains("Refund to wallet", StringComparison.OrdinalIgnoreCase) == true
               && desc.Contains("Old Fas Balance", StringComparison.OrdinalIgnoreCase);

        public static bool IsPaymentForOrder(string desc)
            => desc?.Contains("Payment for Order", StringComparison.OrdinalIgnoreCase) == true;

        public static bool IsFASPayment(string desc)
        {
            if (!IsPaymentForOrder(desc)) return false;
            return WalletChanged(desc, "FAS");
        }

        public static bool IsBasicPayment(string desc)
        {
            if (!IsPaymentForOrder(desc)) return false;
            return WalletChanged(desc, "Normal");
        }

        private static bool WalletChanged(string desc, string wallet)
        {
            var match = BalanceChangeRegex.Matches(desc ?? "");
            foreach (Match m in match)
            {
                if (!m.Groups["label"].Value.Equals(wallet, StringComparison.OrdinalIgnoreCase))
                    continue;

                var before = decimal.Parse(m.Groups["before"].Value, CultureInfo.InvariantCulture);
                var after = decimal.Parse(m.Groups["after"].Value, CultureInfo.InvariantCulture);

                if (before != after)
                    return true;
            }
            return false;
        }
    }
}
