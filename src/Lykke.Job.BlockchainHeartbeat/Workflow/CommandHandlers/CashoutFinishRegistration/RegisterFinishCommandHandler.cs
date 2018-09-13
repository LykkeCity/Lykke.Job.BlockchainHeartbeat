using System.Threading.Tasks;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.LastCashoutEventMoment;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.CashoutFinishRegistration;
using Lykke.Job.BlockchainHeartbeat.Workflow.Events.CashoutFinishRegistration;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.CommandHandlers.CashoutFinishRegistration
{
    public class RegisterFinishCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly ILastCashoutEventMomentRepository _lastMomentRepository;

        public RegisterFinishCommandHandler(IChaosKitty chaosKitty, 
            ILastCashoutEventMomentRepository lastMomentRepository)
        {
            _chaosKitty = chaosKitty;
            _lastMomentRepository = lastMomentRepository;
        }

        public async Task<CommandHandlingResult> Handle(RegisterFinishCommand command,
            IEventPublisher publisher)
        {
            if (await _lastMomentRepository.SetLastCashoutEventMomentAsync(command.AssetId, command.CashoutFinishedAt))
            {
                _chaosKitty.Meow(command.OperationId);

                publisher.PublishEvent(new FinishRegisteredEvent
                {
                    OperationId = command.OperationId
                });
            }

            return CommandHandlingResult.Ok();
        }
    }
}
