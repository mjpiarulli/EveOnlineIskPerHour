using EveOnlineIskPerHour.WalletLog;
using Microsoft.AspNetCore.Components;

namespace EveOnlineIskPerHour.Shared
{
    public partial class WalletLogCalculationResultView : ComponentBase
    {
        [Parameter]
        public WalletLogCalculationResult ResultModel { get; set; }
    }
}
