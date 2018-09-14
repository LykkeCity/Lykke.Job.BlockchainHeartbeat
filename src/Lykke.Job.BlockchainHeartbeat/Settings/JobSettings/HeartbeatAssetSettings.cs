using System;

namespace Lykke.Job.BlockchainHeartbeat.Settings.JobSettings
{
    public class HeartbeatAssetSettings
    {
        public Guid ClientId { get; set; }

        public string AssetId { get; set; }

        public string ToAddress { get; set; }

        public decimal Amount { get; set; }

        public TimeSpan MaxCashoutInactivePeriod { get; set; }
    }
}
