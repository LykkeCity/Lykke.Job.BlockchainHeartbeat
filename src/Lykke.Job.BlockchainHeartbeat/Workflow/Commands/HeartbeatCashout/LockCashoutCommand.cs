using System;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LockCashoutCommand
    {
        public string BlockchainType { get; set; }

        public string AssetId { get; set; }

        public Guid OperationId { get; set; }
    }
}
