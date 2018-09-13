using System;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout
{
    public class ReleaseCashoutLockCommand
    {
        public Guid OperationId { get; set; }
        
        public string BlockchainType { get; set; }

        public string AssetId { get; set; }
    }
}
