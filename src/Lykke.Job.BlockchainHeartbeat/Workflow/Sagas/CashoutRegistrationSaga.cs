using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutRegistration;
using Lykke.Job.BlockchainHeartbeat.Workflow.BoundedContexts;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.CashoutRegistration;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Sagas
{
    public class CashoutRegistrationSaga
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly ICashoutRegistrationRepository _repository;

        public CashoutRegistrationSaga(IChaosKitty chaosKitty, ICashoutRegistrationRepository repository)
        {
            _chaosKitty = chaosKitty;
            _repository = repository;
        }

        [UsedImplicitly]
        private Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutStartedEvent evt,
            ICommandSender sender)
        {
            return OnStarted(evt.OperationId, evt.AssetId, DateTime.UtcNow, sender);
        }

        [UsedImplicitly]
        private Task Handle(BlockchainCashoutProcessor.Contract.Events.CrossClientCashoutStartedEvent evt,
            ICommandSender sender)
        {
            return OnStarted(evt.OperationId, evt.AssetId, DateTime.UtcNow, sender);
        }

        [UsedImplicitly]
        private Task Handle(BlockchainCashoutProcessor.Contract.Events.BatchedCashoutStartedEvent evt,
            ICommandSender sender)
        {
            return OnStarted(evt.OperationId, evt.AssetId, DateTime.UtcNow, sender);
        }

        [UsedImplicitly]
        private Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutCompletedEvent evt, ICommandSender sender)
        {
            return OnFinished(evt.OperationId, evt.AssetId, evt.StartMoment, sender);
        }

        [UsedImplicitly]
        private  Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutFailedEvent evt, ICommandSender sender)
        {
            return OnFinished(evt.OperationId, evt.AssetId, evt.StartMoment, sender);
        }

        [UsedImplicitly]
        private Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutsBatchCompletedEvent evt, ICommandSender sender)
        {
            return OnFinished(evt.BatchId, evt.AssetId, evt.StartMoment, sender);
        }

        [UsedImplicitly]
        private Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutsBatchFailedEvent evt, ICommandSender sender)
        {
            return OnFinished(evt.BatchId, evt.AssetId, evt.StartMoment, sender);
        }

        [UsedImplicitly]
        private Task Handle(BlockchainCashoutProcessor.Contract.Events.CrossClientCashoutCompletedEvent evt,
            ICommandSender sender)
        {
            return OnStarted(evt.OperationId, evt.AssetId, evt.FinishMoment, sender);
        }

        private async Task OnFinished(Guid operationId, string assetId, DateTime finishMoment, ICommandSender sender)
        {
            var aggregate = await _repository.GetOrAddAsync(
                operationId,
                () => CashoutRegistrationAggregate.StartNew(operationId,
                    assetId));

            aggregate.OnFinished(finishMoment);

            sender.SendCommand(new RegisterCashoutRegistrationLastMomentCommand
                {
                    AssetId = aggregate.AssetId,
                    Moment = finishMoment,
                    OperationId = aggregate.OperationId
                },
                CashoutRegistrationBoundedContext.Name);

            _chaosKitty.Meow(operationId);

            await _repository.SaveAsync(aggregate);
        }

        private async Task OnStarted(Guid operationId, string assetId, DateTime startMoment, ICommandSender sender)
        {
            var aggregate = await _repository.GetOrAddAsync(
                operationId,
                () => CashoutRegistrationAggregate.StartNew(operationId,
                    assetId));

            aggregate.OnStarted(DateTime.UtcNow);

            sender.SendCommand(new RegisterCashoutRegistrationLastMomentCommand
                {
                    AssetId = aggregate.AssetId,
                    Moment = startMoment,
                    OperationId = aggregate.OperationId
                },
                CashoutRegistrationBoundedContext.Name);

            _chaosKitty.Meow(operationId);

            await _repository.SaveAsync(aggregate);
        }
    }
}
