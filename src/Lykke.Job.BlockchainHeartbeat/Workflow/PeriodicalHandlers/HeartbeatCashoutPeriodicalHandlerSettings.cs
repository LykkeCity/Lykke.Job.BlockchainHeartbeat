using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.PeriodicalHandlers
{
    public class HeartbeatCashoutPeriodicalHandlerSettings
    {
        public Guid ClientId { get; set; }

        public string AssetId { get; set; }

        public string ToAddress { get; set; }

        public decimal Amount { get; set; }

        public TimeSpan MaxCashoutInactivePeriod { get; set; }
    }
}
