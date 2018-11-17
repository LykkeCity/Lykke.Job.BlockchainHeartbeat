using System;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Settings
{
    public class HeartbeatCashoutPeriodicalHandlerSettings
    {
        public Guid ClientId { get; set; }
        public string AssetId { get; set; }

        public string ToAddress { get; set; }
        public TimeSpan ExecutionTimeout { get; set; }
        public string ToAddressExtension { get; set; }
        public decimal Amount { get; set; }
        public TimeSpan MaxCashoutInactivePeriod { get; set; }
        public string FeeCashoutTargetClientId { get; set; }
        public decimal ClientBalance { get; set; }
    }
}
