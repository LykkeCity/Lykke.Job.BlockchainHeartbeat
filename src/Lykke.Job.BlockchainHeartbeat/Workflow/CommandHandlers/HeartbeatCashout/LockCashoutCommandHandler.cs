using System.Threading.Tasks;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutLock;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.Events.HeartbeatCashout;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.CommandHandlers.HeartbeatCashout
{
    public class LockCashoutCommandHandler
    {
        private readonly ICashoutLockRepository _cashoutLockRepository;
        private readonly IChaosKitty _chaosKitty;

        public LockCashoutCommandHandler(ICashoutLockRepository cashoutLockRepository, 
            IChaosKitty chaosKitty)
        {
            _cashoutLockRepository = cashoutLockRepository;
            _chaosKitty = chaosKitty;
        }

        public async Task<CommandHandlingResult> Handle(LockCashoutCommand command,
            IEventPublisher publisher)
        {
            if(await _cashoutLockRepository.TryGetLockAsync(command.BlockchainType, 
                command.AssetId, 
                command.OperationId))
            {
                publisher.PublishEvent(new CashoutLockedEvent
                {
                    OperationId = command.OperationId
                });
            }

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
