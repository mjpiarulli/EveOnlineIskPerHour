using System;

namespace EveOnlineIskPerHour.WalletLog
{
    public record WalletLog(DateTime Date, string IskPayoutType, int Isk, int TotalIsk, string Reason);
}
