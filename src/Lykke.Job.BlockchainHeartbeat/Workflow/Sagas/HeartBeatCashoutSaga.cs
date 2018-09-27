using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.Events.HeartbeatCashout;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Sagas
{
    public class HeartBeatCashoutSaga
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly IHeartbeatCashoutRepository _repository;

        public static string BoundedContext = "bcn-integration.cashout-hearbeat";

        public HeartBeatCashoutSaga(IChaosKitty chaosKitty, IHeartbeatCashoutRepository repository)
        {
            _chaosKitty = chaosKitty;
            _repository = repository;
        }

        [UsedImplicitly]
        private async Task Handle(HeartbeatCashoutStartedEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.GetOrAddAsync(
                evt.OperationId,
                () => HeartbeatCashoutAggregate.StartNew(evt.OperationId, 
                    evt.ToAddress, 
                    evt.ToAddressExtension,
                    evt.Amount,
                    evt.AssetId));

            _chaosKitty.Meow(evt.OperationId);

            if (aggregate.OnStarted())
            {
                sender.SendCommand(new AcquireCashoutLockCommand
                    {
                        OperationId = evt.OperationId,
                        AssetId = evt.AssetId
                    },
                    BoundedContext);
                
                _chaosKitty.Meow(evt.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        [UsedImplicitly]
        private async Task Handle(CashoutLockAcquiredEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.GetAsync(evt.OperationId);

            if (aggregate.OnLockAcquired())
            {
                sender.SendCommand(new StartCryptoCashoutCommand
                {
                    OperationId = aggregate.OperationId,
                    AssetId = aggregate.AssetId,
                    Amount = aggregate.Amount,
                    ToAddress = aggregate.ToAddress,
                    ToAddressExtension = aggregate.ToAddressExtension
                }, BoundedContext);

                _chaosKitty.Meow(evt.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        #region FinishEvents

        [UsedImplicitly]
        private Task Handle(Service.Operations.Contracts.Events.OperationCompletedEvent evt,
            ICommandSender sender)
        {
            return HandleOperationFinishEvent(evt.OperationId, sender);
        }

        [UsedImplicitly]
        private Task Handle(Service.Operations.Contracts.Events.OperationCorruptedEvent evt,
            ICommandSender sender)
        {
            return HandleOperationFinishEvent(evt.OperationId, sender);
        }

        [UsedImplicitly]
        private Task Handle(Service.Operations.Contracts.Events.OperationFailedEvent evt,
            ICommandSender sender)
        {
            return HandleOperationFinishEvent(evt.OperationId, sender);
        }

        private async Task HandleOperationFinishEvent(Guid operationId, ICommandSender sender)
        {
            var aggregate = await _repository.TryGetAsync(operationId);

            if (aggregate == null)
            {
                //this is not a heartbeat cashout operation
                return;
            }

            if (aggregate.OnFinished(DateTime.UtcNow))
            {
                sender.SendCommand(new ReleaseCashoutLockCommand
                    {
                        AssetId = aggregate.AssetId,
                        OperationId = aggregate.OperationId
                    },
                    BoundedContext);

                _chaosKitty.Meow(operationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        #endregion
    }
}
