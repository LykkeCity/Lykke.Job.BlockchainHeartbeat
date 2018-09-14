using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainCashoutProcessor.Contract;
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
                    evt.ClientId, 
                    evt.ToAddress, 
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
                sender.SendCommand(new BlockchainCashoutProcessor.Contract.Commands.StartCashoutCommand
                {
                    OperationId = aggregate.OperationId,
                    AssetId = aggregate.AssetId,
                    Amount = aggregate.Amount,
                    ToAddress = aggregate.ToAddress,
                    ClientId = aggregate.ClientId
                }, BlockchainCashoutProcessorBoundedContext.Name);

                _chaosKitty.Meow(evt.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        [UsedImplicitly]
        private async Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutCompletedEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.TryGetAsync(evt.OperationId);

            if (aggregate == null)
            {
                //this is not a heartbeat cashout command
                return;
            }
            
            if (aggregate.OnFinished(evt.FinishMoment))
            {
                sender.SendCommand(new ReleaseCashoutLockCommand
                    {
                        AssetId = aggregate.AssetId,
                        OperationId = aggregate.OperationId
                    }, 
                    BoundedContext);

                _chaosKitty.Meow(evt.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        [UsedImplicitly]
        private async Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutFailedEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.TryGetAsync(evt.OperationId);

            if (aggregate == null)
            {
                //this is not a heartbeat cashout command
                return;
            }

            if (aggregate.OnFinished(evt.FinishMoment))
            {
                sender.SendCommand(new ReleaseCashoutLockCommand
                    {
                        AssetId = aggregate.AssetId,
                        OperationId = aggregate.OperationId
                    },
                    BoundedContext);

                _chaosKitty.Meow(evt.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        [UsedImplicitly]
        private async Task Handle(CashoutLockReleasedEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.TryGetAsync(evt.OperationId);

            if (aggregate == null)
            {
                //this is not a heartbeat cashout command
                return;
            }

            if (aggregate.OnLockReleased()) 
            {
                await _repository.SaveAsync(aggregate);

                _chaosKitty.Meow(evt.OperationId);
            }
        }
    }
}
