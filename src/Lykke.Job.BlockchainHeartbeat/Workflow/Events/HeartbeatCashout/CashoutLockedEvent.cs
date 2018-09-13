using System;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Events.HeartbeatCashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class CashoutLockedEvent
    {
        public Guid OperationId { get; set; }
    }
}
