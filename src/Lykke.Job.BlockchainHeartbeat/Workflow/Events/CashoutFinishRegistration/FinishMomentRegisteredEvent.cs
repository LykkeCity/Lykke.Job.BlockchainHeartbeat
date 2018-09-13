using System;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Events.CashoutFinishRegistration
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class FinishMomentRegisteredEvent
    {
        public Guid OperationId { get; set; }
    }
}
