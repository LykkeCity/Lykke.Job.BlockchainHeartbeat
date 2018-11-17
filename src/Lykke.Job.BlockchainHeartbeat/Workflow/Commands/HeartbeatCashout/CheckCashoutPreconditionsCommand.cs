using System;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class CheckCashoutPreconditionsCommand
    {
        public Guid OperationId { get; set; }

        public string AssetId { get; set; }

        public TimeSpan MaxCashoutInactivePeriod { get; set; }
    }
}
