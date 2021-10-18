using System;
using System.Collections.Generic;
using System.Linq;

namespace EveOnlineIskPerHour.WalletLog
{
    public static class WalletLogCalculator
    {
        public static WalletLogCalculationResult CalculateStats(IEnumerable<WalletLog> walletLogs)
        {
            var iskPerHour = CalculateIskPerHour(walletLogs);
            var totalIncome = CalculateTotalIncome(walletLogs);
            var totalTime = CalculateTotalTime(walletLogs);
            var fastestSiteTime = CalculateFastestSiteTime(walletLogs);
            var averageSiteTime = CalculateAverageSiteTime(walletLogs);
            var worstSiteTime = CalculateWorstSiteTime(walletLogs);
            var measuredSites = CalculateMeasuredSites(walletLogs);

            return new WalletLogCalculationResult(iskPerHour, totalIncome, totalTime, fastestSiteTime, averageSiteTime, worstSiteTime, measuredSites);
        }

        public static int CalculateIskPerHour(IEnumerable<WalletLog> walletLogs)
        {            
            DateTime startDate = DateTime.MaxValue;
            DateTime endDate = DateTime.MinValue;

            int totalIsk = 0;
            var firstPayment = 0;

            foreach (var walletLog in walletLogs) 
            {
                if (walletLog.Isk < 0)
                    continue;

                if(endDate > walletLog.Date)
                    startDate = walletLog.Date;

                if (endDate < walletLog.Date)
                    endDate = walletLog.Date;

                totalIsk += walletLog.Isk;
                firstPayment = walletLog.Isk;
            }

            if (startDate == endDate)
                return 0;

            totalIsk -= firstPayment;
            var tickDifferenceSeconds = TimeSpan.FromTicks(endDate.Ticks - startDate.Ticks).TotalSeconds;
            var iskPerHour = Convert.ToInt32(totalIsk / (tickDifferenceSeconds / 60 / 60));

            return iskPerHour;                
        }

        public static int CalculateTotalIncome(IEnumerable<WalletLog> walletLogs)
        {
            var total = walletLogs
                .Where(wl => wl.Isk > 0 && wl.Date != walletLogs.Min(wl2 => wl2.Date))
                .Sum(wl => wl.Isk);

            return total;
        }

        public static TimeSpan CalculateTotalTime(IEnumerable<WalletLog> walletLogs)
        {
            var durations = new List<double>();
            var lastDateTime = DateTime.MinValue;            

            foreach (var walletLog in walletLogs)
            {
                var currentDateTime = walletLog.Date;

                if (walletLog.Isk < 0)
                    continue;

                if (lastDateTime == DateTime.MinValue)
                {
                    lastDateTime = currentDateTime;
                    continue;
                }

                var tickDifference = lastDateTime.Ticks - currentDateTime.Ticks;
                var currentTime = TimeSpan.FromTicks(tickDifference).TotalSeconds;
                durations.Add(currentTime);                

                lastDateTime = currentDateTime;
            }

            var totalTime = durations.Sum();

            return TimeSpan.FromSeconds(totalTime);
        }

        public static TimeSpan CalculateFastestSiteTime(IEnumerable<WalletLog> walletLogs)
        {
            var fastestTime = double.MaxValue;
            var lastDateTime = DateTime.MinValue;

            foreach(var walletLog in walletLogs)
            {
                var currentDateTime = walletLog.Date;

                if (walletLog.Isk < 0)
                    continue;

                if (lastDateTime == DateTime.MinValue)
                {
                    lastDateTime = currentDateTime;
                    continue;
                }

                var tickDifference = lastDateTime.Ticks - currentDateTime.Ticks;
                var currentTime = TimeSpan.FromTicks(tickDifference).TotalSeconds;
                if (currentTime < fastestTime)
                    fastestTime = currentTime;

                lastDateTime = currentDateTime;
            }

            return TimeSpan.FromSeconds(fastestTime);
        }

        public static TimeSpan CalculateAverageSiteTime(IEnumerable<WalletLog> walletLogs)
        {
            var durations = new List<double>();
            var lastDateTime = DateTime.MinValue;
            var count = 0;

            foreach (var walletLog in walletLogs)
            {
                var currentDateTime = walletLog.Date;

                if (walletLog.Isk < 0)
                    continue;

                if (lastDateTime == DateTime.MinValue)
                {
                    lastDateTime = currentDateTime;
                    continue;
                }

                var tickDifference = lastDateTime.Ticks - currentDateTime.Ticks;
                var currentTime = TimeSpan.FromTicks(tickDifference).TotalSeconds;
                durations.Add(currentTime);
                count++;

                lastDateTime = currentDateTime;
            }

            var average = durations.Sum() / count;

            return TimeSpan.FromSeconds(average);
        }

        public static TimeSpan CalculateWorstSiteTime(IEnumerable<WalletLog> walletLogs)
        {
            var worstTime = double.MinValue;
            var lastDateTime = DateTime.MinValue;

            foreach (var walletLog in walletLogs)
            {
                var currentDateTime = walletLog.Date;

                if (walletLog.Isk < 0)
                    continue;

                if (lastDateTime == DateTime.MinValue)
                {
                    lastDateTime = currentDateTime;
                    continue;
                }

                var tickDifference = lastDateTime.Ticks - currentDateTime.Ticks;
                var currentTime = TimeSpan.FromTicks(tickDifference).TotalSeconds;
                if (currentTime > worstTime)
                    worstTime = currentTime;

                lastDateTime = currentDateTime;
            }

            return TimeSpan.FromSeconds(worstTime);
        }

        public static int CalculateMeasuredSites(IEnumerable<WalletLog> walletLogs)
        {
            var counter = 0;

            foreach(var walletLog in walletLogs)
            {
                if (walletLog.Isk < 0)
                    continue;

                counter++;
            }

            counter--;

            return counter;
        }

        public static List<double> GetSiteTimeDurationsInMinutes(IEnumerable<WalletLog> walletLogs)
        {
            var durations = new List<double>();
            var lastDateTime = DateTime.MinValue;            

            foreach (var walletLog in walletLogs)
            {
                var currentDateTime = walletLog.Date;

                if (walletLog.Isk < 0)
                    continue;

                if (lastDateTime == DateTime.MinValue)
                {
                    lastDateTime = currentDateTime;
                    continue;
                }

                var tickDifference = lastDateTime.Ticks - currentDateTime.Ticks;
                var currentTime = TimeSpan.FromTicks(tickDifference).TotalMinutes;
                durations.Add(currentTime);                

                lastDateTime = currentDateTime;
            }

            durations.Reverse();

            return durations;
        }

        public static string[] GetSiteTimeStamps(IEnumerable<WalletLog> walletLogs)
        {
            var siteTimeStamps = new List<string>();
            var lastDateTime = DateTime.MinValue;

            foreach (var walletLog in walletLogs)
            {
                var currentDateTime = walletLog.Date;

                if (walletLog.Isk < 0)
                    continue;

                if (lastDateTime == DateTime.MinValue)
                {
                    lastDateTime = currentDateTime;
                    siteTimeStamps.Add(currentDateTime.ToString("HH:mm"));
                    continue;
                }

                var tickDifference = lastDateTime.Ticks - currentDateTime.Ticks;
                var currentTime = TimeSpan.FromTicks(tickDifference).TotalMinutes;
                siteTimeStamps.Add(currentDateTime.ToString("HH:mm"));

                lastDateTime = currentDateTime;
            }

            siteTimeStamps.RemoveAt(siteTimeStamps.Count() - 1);
            siteTimeStamps.Reverse();

            return siteTimeStamps.ToArray();
        }
    }
}
