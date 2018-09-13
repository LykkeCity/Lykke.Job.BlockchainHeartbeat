using System;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Commands.CashoutFinishRegistration
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class RegisterFinishCommand
    {
        public Guid OperationId { get; set; }

        public DateTime CashoutFinishedAt { get; set; }

        public string AssetId { get; set; }
    }
}
