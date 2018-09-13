using System;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Events.HeartbeatCashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class CashoutLockReleasedEvent
    {
        public Guid OperationId { get; set; }
    }
}
