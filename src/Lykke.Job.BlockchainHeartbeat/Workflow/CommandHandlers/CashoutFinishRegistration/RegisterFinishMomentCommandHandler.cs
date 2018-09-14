using System.Threading.Tasks;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.LastCashoutEventMoment;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.CashoutFinishRegistration;
using Lykke.Job.BlockchainHeartbeat.Workflow.Events.CashoutFinishRegistration;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.CommandHandlers.CashoutFinishRegistration
{
    public class RegisterFinishMomentCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly ILastCashoutEventMomentRepository _lastMomentRepository;

        public RegisterFinishMomentCommandHandler(IChaosKitty chaosKitty, 
            ILastCashoutEventMomentRepository lastMomentRepository)
        {
            _chaosKitty = chaosKitty;
            _lastMomentRepository = lastMomentRepository;
        }

        public async Task<CommandHandlingResult> Handle(RegisterFinishMomentCommand momentCommand,
            IEventPublisher publisher)
        {
            if (await _lastMomentRepository.SetLastCashoutEventMomentAsync(momentCommand.AssetId,
                momentCommand.CashoutFinishedAt))
            {
                _chaosKitty.Meow(momentCommand.OperationId);

                publisher.PublishEvent(new FinishMomentRegisteredEvent
                {
                    OperationId = momentCommand.OperationId
                });
            }

            return CommandHandlingResult.Ok();
        }
    }
}
