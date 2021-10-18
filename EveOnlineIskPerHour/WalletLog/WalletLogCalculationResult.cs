using System;

namespace EveOnlineIskPerHour.WalletLog
{
    public record WalletLogCalculationResult(int IskPerHour, int TotalIncome, TimeSpan TotalTime, 
        TimeSpan BestSiteTime, TimeSpan AverageSiteTime, TimeSpan WorstSiteTime, 
        int MeasuredSites);
    
}
