using System;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class AcquireCashoutLockCommand
    {
        public string AssetId { get; set; }

        public Guid OperationId { get; set; }
    }
}
