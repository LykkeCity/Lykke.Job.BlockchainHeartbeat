using System;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Settings
{
    public class HeartbeatCashoutPeriodicalHandlerSettings
    {
        public string AssetId { get; set; }

        public string ToAddress { get; set; }

        public string ToAddressExtension { get; set; }

        public decimal Amount { get; set; }

        public TimeSpan MaxCashoutInactivePeriod { get; set; }
    }
}
