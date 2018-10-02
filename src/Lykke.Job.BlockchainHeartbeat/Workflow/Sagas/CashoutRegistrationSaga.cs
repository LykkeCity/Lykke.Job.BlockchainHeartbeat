using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutRegistration;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.CashoutRegistration;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Sagas
{
    public class CashoutRegistrationSaga
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly ICashoutRegistrationRepository _repository;

        public static string BoundedContext = "bcn-integration.cashout-heartbeat-registration";

        public CashoutRegistrationSaga(IChaosKitty chaosKitty, ICashoutRegistrationRepository repository)
        {
            _chaosKitty = chaosKitty;
            _repository = repository;
        }

        [UsedImplicitly]
        private async Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutStartedEvent evt,
            ICommandSender sender)
        {
            var aggregate = await _repository.GetOrAddAsync(
                evt.OperationId,
                () => CashoutRegistrationAggregate.StartNew(evt.OperationId,
                    evt.AssetId));

            aggregate.OnStarted(DateTime.UtcNow);

            sender.SendCommand(new RegisterCashoutRegistrationLastMomentCommand
                {
                    AssetId = aggregate.AssetId,
                    Moment = aggregate.StartMoment ?? throw new ArgumentNullException(nameof(aggregate.StartMoment)),
                    OperationId = aggregate.OperationId
                },
                BoundedContext);

            _chaosKitty.Meow(evt.OperationId);

            await _repository.SaveAsync(aggregate);
        }

        [UsedImplicitly]
        private async Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutCompletedEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.GetOrAddAsync(
                evt.OperationId,
                () => CashoutRegistrationAggregate.StartNew(evt.OperationId, 
                    evt.AssetId));

            aggregate.OnFinished(evt.FinishMoment);

            sender.SendCommand(new RegisterCashoutRegistrationLastMomentCommand
                {
                    AssetId = aggregate.AssetId,
                    Moment = aggregate.FinishMoment ?? throw new ArgumentNullException(nameof(aggregate.FinishMoment)),
                    OperationId = aggregate.OperationId
                },
                BoundedContext);

            _chaosKitty.Meow(evt.OperationId);

            await _repository.SaveAsync(aggregate);
        }

        [UsedImplicitly]
        private async Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutFailedEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.GetOrAddAsync(
                evt.OperationId,
                () => CashoutRegistrationAggregate.StartNew(evt.OperationId,
                    evt.AssetId));

            aggregate.OnFinished(evt.FinishMoment);

            sender.SendCommand(new RegisterCashoutRegistrationLastMomentCommand
                {
                    AssetId = aggregate.AssetId,
                    Moment = aggregate.FinishMoment ?? throw new ArgumentNullException(nameof(aggregate.FinishMoment)),
                    OperationId = aggregate.OperationId
                },
                BoundedContext);

            _chaosKitty.Meow(evt.OperationId);

            await _repository.SaveAsync(aggregate);
        }
    }
}
