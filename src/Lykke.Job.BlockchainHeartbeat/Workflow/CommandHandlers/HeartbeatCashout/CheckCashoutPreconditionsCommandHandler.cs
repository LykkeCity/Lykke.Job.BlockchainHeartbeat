using System;
using System.Threading.Tasks;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.LastCashoutEventMoment;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.Events.HeartbeatCashout;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.CommandHandlers.HeartbeatCashout
{
    public class CheckCashoutPreconditionsCommandHandler
    {
        private readonly ILastCashoutEventMomentRepository _lastCashoutEventMomentRepository;
        private readonly IChaosKitty _chaosKitty;

        public CheckCashoutPreconditionsCommandHandler(ILastCashoutEventMomentRepository lastCashoutEventMomentRepository, 
            IChaosKitty chaosKitty)
        {
            _lastCashoutEventMomentRepository = lastCashoutEventMomentRepository;
            _chaosKitty = chaosKitty;
        }
        
        public async Task<CommandHandlingResult> Handle(CheckCashoutPreconditionsCommand command,
            IEventPublisher publisher)
        {
            var lastMoment = await _lastCashoutEventMomentRepository.GetLastEventMomentAsync(command.AssetId) ?? DateTime.MinValue;
            
            if (DateTime.UtcNow - lastMoment > command.MaxCashoutInactivePeriod)
            {
                publisher.PublishEvent(new CashoutPreconditionPassedEvent
                {
                    OperationId = command.OperationId,
                    Moment = DateTime.UtcNow
                });
            }
            else
            {
                publisher.PublishEvent(new CashoutPreconditionRejectedEvent
                {
                    OperationId = command.OperationId,
                    Moment = DateTime.UtcNow
                });
            }

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
