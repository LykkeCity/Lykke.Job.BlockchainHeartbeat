using System;
using System.Threading.Tasks;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutLock;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.Events.HeartbeatCashout;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.CommandHandlers.HeartbeatCashout
{
    public class AcquireCashoutLockCommandHandler
    {
        private readonly ICashoutLockRepository _cashoutLockRepository;
        private readonly IChaosKitty _chaosKitty;

        public AcquireCashoutLockCommandHandler(ICashoutLockRepository cashoutLockRepository, 
            IChaosKitty chaosKitty)
        {
            _cashoutLockRepository = cashoutLockRepository;
            _chaosKitty = chaosKitty;
        }

        public async Task<CommandHandlingResult> Handle(AcquireCashoutLockCommand command,
            IEventPublisher publisher)
        {
            var lockMoment = DateTime.UtcNow;
            if (await _cashoutLockRepository.TryLockAsync(command.AssetId, 
                command.OperationId, lockMoment))
            {
                _chaosKitty.Meow(command.OperationId);

                publisher.PublishEvent(new CashoutLockAcquiredEvent
                {
                    OperationId = command.OperationId,
                    Moment = lockMoment
                });
            }
            else
            {
                _chaosKitty.Meow(command.OperationId);

                publisher.PublishEvent(new CashoutLockRejectedEvent
                {
                    OperationId = command.OperationId,
                    Moment = DateTime.UtcNow
                });
            }

            return CommandHandlingResult.Ok();
        }
    }
}
