using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutRegistration;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.CashoutFinishRegistration;
using Lykke.Job.BlockchainHeartbeat.Workflow.Events.CashoutFinishRegistration;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Sagas
{
    public class CashoutFinishRegistrationSaga
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly ICashoutFinishRegistrationRepository _repository;

        public static string BoundedContext = "bcn-integration.cashout-finish-registration";

        public CashoutFinishRegistrationSaga(IChaosKitty chaosKitty, ICashoutFinishRegistrationRepository repository)
        {
            _chaosKitty = chaosKitty;
            _repository = repository;
        }

        [UsedImplicitly]
        private async Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutCompletedEvent evt, ICommandSender sender)
        {
            throw new NotImplementedException("Add failed event and use cashout completed date");

            var aggregate = await _repository.GetOrAddAsync(
                evt.OperationId,
                () => CashoutFinishRegistrationAggregate.StartNew(evt.OperationId, 
                    evt.AssetId, 
                    DateTime.UtcNow));

            if (aggregate.OnRegistrationStarted())
            {
                sender.SendCommand(new RegisterFinishCommand
                    {
                        AssetId = aggregate.AssetId,
                        CashoutFinishedAt = aggregate.CashoutFinishedAt,
                        OperationId = aggregate.OperationId
                    }, 
                    BoundedContext);
                
                _chaosKitty.Meow(evt.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        [UsedImplicitly]
        private async Task Handle(FinishRegisteredEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.GetAsync(evt.OperationId);

            if (aggregate.OnFinishRegistered())
            {
                await _repository.SaveAsync(aggregate);

                _chaosKitty.Meow(evt.OperationId);
            }
        }
    }
}
