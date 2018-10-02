using System;
using System.Threading.Tasks;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.LastCashoutEventMoment;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.CashoutRegistration;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.Events.HeartbeatCashout;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.CommandHandlers.HeartbeatCashout
{
    public class RegisterHeartbeatCashoutLastMomentCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly ILastCashoutEventMomentRepository _lastMomentRepository;

        public RegisterHeartbeatCashoutLastMomentCommandHandler(IChaosKitty chaosKitty, 
            ILastCashoutEventMomentRepository lastMomentRepository)
        {
            _chaosKitty = chaosKitty;
            _lastMomentRepository = lastMomentRepository;
        }

        public async Task<CommandHandlingResult> Handle(RegisterHeartbeatCashoutLastMomentCommand momentCommand,
            IEventPublisher publisher)
        {
            await _lastMomentRepository.SetLastCashoutEventMomentAsync(momentCommand.AssetId,
                momentCommand.Moment);

            _chaosKitty.Meow(momentCommand.OperationId);

            publisher.PublishEvent(new CashoutLastMomentRegisteredEvent
            {
                OperationId = momentCommand.OperationId,
                Moment = DateTime.UtcNow
            });

            return CommandHandlingResult.Ok();
        }
    }
}
