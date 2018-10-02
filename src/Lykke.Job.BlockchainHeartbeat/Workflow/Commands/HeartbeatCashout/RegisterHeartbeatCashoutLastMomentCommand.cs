using System;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class RegisterHeartbeatCashoutLastMomentCommand
    {
        public Guid OperationId { get; set; }

        public DateTime Moment { get; set; }

        public string AssetId { get; set; }
    }
}
