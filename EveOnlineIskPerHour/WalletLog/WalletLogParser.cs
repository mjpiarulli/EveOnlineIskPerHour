using System;
using System.Collections.Generic;

namespace EveOnlineIskPerHour.WalletLog
{
    public static class WalletLogParser
    {
        public static IEnumerable<WalletLog> Parse(string rawWalletLog)
        {
            var split = rawWalletLog.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var segment in split)
            {
                var result = ParseWalletLogLine(segment.Trim());

                if (result == null)
                    yield break;

                yield return result;
            }
                
        }

        private static WalletLog ParseWalletLogLine(string rawWalletLogLine)
        {
            var split = rawWalletLogLine.Split("\t");
            if (split.Length < 5)
                return null;

            DateTime date;
            var dateSuccess = DateTime.TryParse(split[0].Trim(), out date);
            
            var iskRewardType = split[1].Trim();

            double isk;
            var iskSuccess = double.TryParse(split[2].Trim().Replace(",", string.Empty).Split(" ")[0], out isk);

            double totalIsk;
            var totalIskSuccess = double.TryParse(split[3].Trim().Replace(",", string.Empty).Split(" ")[0], out totalIsk);

            var reason = split[4].Trim();

            if (!dateSuccess || !iskSuccess || !totalIskSuccess)
                return null;

            return new WalletLog(date, iskRewardType, isk, totalIsk, reason);
        }
    }
}
