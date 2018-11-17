using System;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class ReleaseCashoutLockCommand
    {
        public Guid OperationId { get; set; }
        
        public string AssetId { get; set; }
    }
}
