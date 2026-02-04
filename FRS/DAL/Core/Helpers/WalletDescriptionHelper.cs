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

        private static readonly Regex RefundPatternRegex =
        new Regex(@"Old\s+(?<label>Fas|Basic|)\s*Balance:\s*\$(?<before>[0-9]+(?:\.[0-9]+)?),\s*New\s*Balance:\s*\$(?<after>[0-9]+(?:\.[0-9]+)?)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex PatternArrow = new Regex(@"\b(?<label>FAS|Normal|Basic)\s*:\s*\$?(?<before>[0-9]+(?:\.[0-9]+)?)\s*->\s*\$?(?<after>[0-9]+(?:\.[0-9]+)?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex PatternOldNew = new Regex(@"Old\s+(?<label>Fas|Basic|Normal|)\s*Balance:\s*\$?(?<before>[0-9]+(?:\.[0-9]+)?),\s*New\s*Balance:\s*\$?(?<after>[0-9]+(?:\.[0-9]+)?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex PatternFromTo = new Regex(@"(?<label>FAS|Normal|Basic)\s+from\s+(?<before>[0-9]+(?:\.[0-9]+)?)\s+to\s+(?<after>[0-9]+(?:\.[0-9]+)?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
        
        private static readonly Regex PatternDirect = new Regex(@"(?:Auto\s+Credit\s+)(?<label>FAS|Normal|Basic)\s+(?<after>[0-9]+(?:\.[0-9]+)?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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

        public static bool IsAutoDebitFAS(string desc)
            => desc?.Contains("Auto Debit FAS", StringComparison.OrdinalIgnoreCase) == true;

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

        public static decimal GetDeductedAmount(string desc, string walletLabel)
        {
            if (string.IsNullOrEmpty(desc)) return 0;

            var matches = BalanceChangeRegex.Matches(desc);
            foreach (Match m in matches)
            {
                if (m.Groups["label"].Value.Equals(walletLabel, StringComparison.OrdinalIgnoreCase))
                {
                    var before = decimal.Parse(m.Groups["before"].Value, CultureInfo.InvariantCulture);
                    var after = decimal.Parse(m.Groups["after"].Value, CultureInfo.InvariantCulture);
                    return before - after; 
                }
            }
            return 0;
        }

        public static decimal GetRefundAmount(string desc, string walletLabel)
        {
            if (string.IsNullOrEmpty(desc)) return 0;

            var matches = RefundPatternRegex.Matches(desc);
            foreach (Match m in matches)
            {
                var label = m.Groups["label"].Value;

                bool isMatch = false;
                if (walletLabel.Equals("FAS", StringComparison.OrdinalIgnoreCase))
                    isMatch = label.Contains("Fas", StringComparison.OrdinalIgnoreCase);
                else
                    isMatch = label.Contains("Basic", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(label);

                if (isMatch)
                {
                    var before = decimal.Parse(m.Groups["before"].Value, CultureInfo.InvariantCulture);
                    var after = decimal.Parse(m.Groups["after"].Value, CultureInfo.InvariantCulture);
                    return after - before; 
                }
            }
            return 0;
        }

        public static decimal? GetAfterBalance(string desc, string walletLabel)
        {
            if (string.IsNullOrEmpty(desc)) return null;

            return TryExtract(PatternArrow, desc, walletLabel)
                   ?? TryExtract(PatternOldNew, desc, walletLabel)
                   ?? TryExtract(PatternFromTo, desc, walletLabel)
                   ?? TryExtract(PatternDirect, desc, walletLabel); // Tambahkan di sini
        }

        private static decimal? TryExtract(Regex regex, string desc, string walletLabel)
        {
            var matches = regex.Matches(desc);
            foreach (Match m in matches)
            {
                var label = m.Groups["label"].Value;
                bool isMatch = false;

                if (walletLabel.Equals("FAS", StringComparison.OrdinalIgnoreCase))
                    isMatch = label.Contains("FAS", StringComparison.OrdinalIgnoreCase);
                else // BASIC / Normal
                    isMatch = label.Contains("Normal", StringComparison.OrdinalIgnoreCase) ||
                              label.Contains("Basic", StringComparison.OrdinalIgnoreCase) ||
                              string.IsNullOrEmpty(label);

                if (isMatch)
                    return decimal.Parse(m.Groups["after"].Value, CultureInfo.InvariantCulture);
            }
            return null;
        }
    }
}
