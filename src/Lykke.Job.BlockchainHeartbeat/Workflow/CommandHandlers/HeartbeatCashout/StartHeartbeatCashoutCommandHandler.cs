using System.Threading.Tasks;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.Events.HeartbeatCashout;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.CommandHandlers.HeartbeatCashout
{
    public class StartHeartbeatCashoutCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;

        public StartHeartbeatCashoutCommandHandler(IChaosKitty chaosKitty)
        {
            _chaosKitty = chaosKitty;
        }

        public Task<CommandHandlingResult> Handle(StartHeartbeatCashoutCommand command,
            IEventPublisher publisher)
        {
            publisher.PublishEvent(new HeartbeatCashoutStartedEvent
            {
                OperationId = command.OperationId
            });

            _chaosKitty.Meow(command.OperationId);

            return Task.FromResult(CommandHandlingResult.Ok());
        }
    }
}
