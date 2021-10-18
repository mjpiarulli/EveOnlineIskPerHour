using System;

namespace EveOnlineIskPerHour.WalletLog
{
    public record WalletLogCalculationResult(double IskPerHour, double TotalIncome, TimeSpan TotalTime, 
        TimeSpan BestSiteTime, TimeSpan AverageSiteTime, TimeSpan WorstSiteTime, 
        int MeasuredSites);
    
}
