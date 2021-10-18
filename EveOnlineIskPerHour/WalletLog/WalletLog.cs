using System;

namespace EveOnlineIskPerHour.WalletLog
{
    public record WalletLog(DateTime Date, string IskPayoutType, double Isk, double TotalIsk, string Reason);
}
