using System;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Commands.CashoutRegistration
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class RegisterCashoutRegistrationLastMomentCommand
    {
        public Guid OperationId { get; set; }

        public DateTime Moment { get; set; }

        public string AssetId { get; set; }
    }
}
