using System;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Events.CashoutRegistration
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class CashoutLastMomentRegisteredEvent
    {
        public Guid OperationId { get; set; }

        public DateTime Moment { get; set; }
    }
}
