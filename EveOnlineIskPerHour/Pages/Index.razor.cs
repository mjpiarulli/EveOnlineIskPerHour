using Blazorise.Charts;
using EveOnlineIskPerHour.WalletLog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EveOnlineIskPerHour.Pages
{
    public partial class Index : ComponentBase
    {
        public string WalletLog = string.Empty;
        public EditContext EditContext;
        public ValidationMessageStore MessageStore;

        public IEnumerable<WalletLog.WalletLog> WalletLogs;
        public WalletLogCalculationResult WalletLogCalculationResult;

        List<string> backgroundColors = new List<string> { ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(54, 162, 235, 0.2f), ChartColor.FromRgba(255, 206, 86, 0.2f), ChartColor.FromRgba(75, 192, 192, 0.2f), ChartColor.FromRgba(153, 102, 255, 0.2f), ChartColor.FromRgba(255, 159, 64, 0.2f) };
        List<string> borderColors = new List<string> { ChartColor.FromRgba(255, 99, 132, 1f), ChartColor.FromRgba(54, 162, 235, 1f), ChartColor.FromRgba(255, 206, 86, 1f), ChartColor.FromRgba(75, 192, 192, 1f), ChartColor.FromRgba(153, 102, 255, 1f), ChartColor.FromRgba(255, 159, 64, 1f) };


        protected override async Task OnInitializedAsync()
        {
            EditContext = new EditContext(WalletLog);
            MessageStore = new ValidationMessageStore(EditContext);
            WalletLogCalculationResult = null;            
        }

        private async Task UpdateStats()
        {
            EditContext = new EditContext(WalletLog);
            MessageStore = new ValidationMessageStore(EditContext);

            if (!ValidateWalletLog() || !EditContext.Validate())
                return;                     

            RefreshIskPerHour(WalletLogs);
            await HandleRedraw();
        }       

        private bool ValidateWalletLog()
        {
            var result = true;

            const int walletLogMaxLength = 30000;

            MessageStore.Clear();

            if(WalletLog.Length > walletLogMaxLength)
            {
                MessageStore.Add(() => WalletLog, $"Can only parse {walletLogMaxLength} characters.  There are {WalletLog.Length} characters here.");
                return false;
            }

            WalletLogs = WalletLogParser.Parse(WalletLog);

            if (WalletLogs == null || !WalletLogs.Any())
            {
                MessageStore.Add(() => WalletLog, "Unable to parse wallet logs.  Please make sure you are ctrl + c on you in game wallet logs and try again.");
                result = false;
            }

            return result;
        }

        private void RefreshIskPerHour(IEnumerable<WalletLog.WalletLog> walletLogs)
        {
            WalletLogCalculationResult = WalletLogCalculator.CalculateStats(walletLogs);
        }        

        LineChart<double> lineChart;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await HandleRedraw();                
            }
        }

        async Task HandleRedraw()
        {
            if (lineChart == null)
                return;

            await lineChart.Clear();

            var labels = WalletLogs == null ? Array.Empty<string>() : WalletLogCalculator.GetSiteTimeStamps(WalletLogs);

            await lineChart.AddLabelsDatasetsAndUpdate(labels, GetLineChartDataset());
        }

        LineChartDataset<double> GetLineChartDataset()
        {
            var data = WalletLogs == null ? new List<double>() : WalletLogCalculator.GetSiteTimeDurationsInMinutes(WalletLogs);

            return new LineChartDataset<double>
            {
                Label = "Site Times",
                Data = data,
                BackgroundColor = backgroundColors,
                BorderColor = borderColors,
                Fill = true,
                PointRadius = 2,
                BorderDash = new List<int> { }
            };
        }       
        
    }
}
