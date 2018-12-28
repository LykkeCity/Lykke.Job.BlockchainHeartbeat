using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.BoundedContexts;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.Events.HeartbeatCashout;
using Lykke.Service.Kyc.Abstractions.Domain.Verification;
using Lykke.Service.Operations.Contracts;
using Lykke.Service.Operations.Contracts.Cashout;
using Lykke.Service.Operations.Contracts.Commands;
using Lykke.Service.Operations.Contracts.Events;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Sagas
{
    public class HeartBeatCashoutSaga
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly IHeartbeatCashoutRepository _repository;
        private readonly ILog _log;


        public HeartBeatCashoutSaga(IChaosKitty chaosKitty, IHeartbeatCashoutRepository repository, ILogFactory logFactory)
        {
            _chaosKitty = chaosKitty;
            _repository = repository;
            _log = logFactory.CreateLog(this);
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
                    evt.AssetId,
                    evt.MaxCashoutInactivePeriod,
                    evt.ClientId,
                    evt.FeeCashoutTargetClientId,
                    evt.ClientBalance));

            _chaosKitty.Meow(evt.OperationId);

            if (aggregate.OnStarted())
            {
                sender.SendCommand(new AcquireCashoutLockCommand
                    {
                        OperationId = evt.OperationId,
                        AssetId = evt.AssetId
                    },
                    HeartbeatCashoutBoundedContext.Name);
                
                _chaosKitty.Meow(evt.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        #region Cashout Lock Result

        [UsedImplicitly]
        private async Task Handle(CashoutLockAcquiredEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.GetAsync(evt.OperationId);

            if (aggregate.OnLockAcquired(evt.Moment))
            {
                sender.SendCommand(new CheckCashoutPreconditionsCommand
                {
                    OperationId = aggregate.OperationId,
                    AssetId = aggregate.AssetId,
                    MaxCashoutInactivePeriod = aggregate.MaxCashoutInactivePeriod
                }, HeartbeatCashoutBoundedContext.Name);

                _chaosKitty.Meow(evt.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        [UsedImplicitly]
        private async Task Handle(CashoutLockRejectedEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.GetAsync(evt.OperationId);

            if (aggregate.OnLockRejected(evt.Moment))
            {
                _chaosKitty.Meow(evt.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        #endregion

        #region Cashout Precondition Result
        
        [UsedImplicitly]
        private async Task Handle(CashoutPreconditionPassedEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.GetAsync(evt.OperationId);

            if (aggregate.OnPreconditionPassed(evt.Moment))
            {
                sender.SendCommand(new RetrieveAssetInfoCommand
                {
                    AssetId = aggregate.AssetId,
                    OperationId = aggregate.OperationId
                }, HeartbeatCashoutBoundedContext.Name);

                _chaosKitty.Meow(evt.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        [UsedImplicitly]
        private async Task Handle(CashoutPreconditionRejectedEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.GetAsync(evt.OperationId);

            if (aggregate.OnPreconditionRejected(evt.Moment))
            {
                sender.SendCommand(new ReleaseCashoutLockCommand
                    {
                        AssetId = aggregate.AssetId,
                        OperationId = aggregate.OperationId
                    },
                    HeartbeatCashoutBoundedContext.Name);

                _chaosKitty.Meow(evt.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        #endregion

        public async Task Handle(AssetInfoRetrievedEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.GetAsync(evt.OperationId);

            if (aggregate.OnAssetInfoRetrieved(evt.Moment,
                evt.DisplayId,
                evt.MultiplierPower,
                evt.AssetAddress,
                evt.Accuracy,
                evt.Blockchain,
                evt.Type,
                evt.IsTradable,
                evt.IsTrusted,
                evt.KycNeeded,
                evt.BlockchainIntegrationLayerId,
                evt.CashoutMinimalAmount,
                evt.LowVolumeAmount,
                evt.BlockchainWithdrawal,
                evt.LykkeEntityId))
            {
                sender.SendCommand(new CreateCashoutCommand
                {
                    OperationId = aggregate.OperationId,
                    DestinationAddress = aggregate.ToAddress,
                    DestinationAddressExtension = aggregate.ToAddressExtension,
                    Volume = aggregate.Amount,
                    Asset = new AssetCashoutModel
                    {
                        Id = aggregate.AssetId,
                        DisplayId = aggregate.AssetDisplayId,
                        MultiplierPower = aggregate.AssetMultiplierPower ?? throw new ArgumentNullException(nameof(aggregate.AssetMultiplierPower)),
                        AssetAddress = aggregate.AssetAddress,
                        Accuracy = aggregate.AssetAccuracy ?? throw new ArgumentNullException(nameof(aggregate.AssetAccuracy)),
                        BlockchainIntegrationLayerId = aggregate.AssetBlockchainIntegrationLayerId,
                        Blockchain = aggregate.AssetBlockchain,
                        Type = aggregate.AssetType,
                        IsTradable = aggregate.AssetIsTradable ?? throw new ArgumentNullException(nameof(aggregate.AssetIsTradable)),
                        IsTrusted = aggregate.AssetIsTrusted ?? throw new ArgumentNullException(nameof(aggregate.AssetIsTrusted)),
                        KycNeeded = aggregate.AssetKycNeeded ?? throw new ArgumentNullException(nameof(aggregate.AssetKycNeeded)),
                        BlockchainWithdrawal = aggregate.AssetBlockchainWithdrawal ?? throw new ArgumentNullException(nameof(aggregate.AssetBlockchainWithdrawal)),
                        CashoutMinimalAmount = aggregate.AssetCashoutMinimalAmount ?? throw new ArgumentNullException(nameof(aggregate.AssetCashoutMinimalAmount)),
                        LowVolumeAmount = aggregate.AssetLowVolumeAmount ?? throw new ArgumentNullException(nameof(aggregate.AssetLowVolumeAmount)),
                        LykkeEntityId = aggregate.AssetLykkeEntityId ?? throw new ArgumentNullException(nameof(aggregate.AssetLykkeEntityId))
                    },
                    Client = new ClientCashoutModel
                    {
                        Id = aggregate.ClientId,
                        Balance = aggregate.ClientBalance,
                        CashOutBlocked = false,
                        KycStatus = KycStatus.Ok.ToString(),
                        ConfirmationType = "google"
                    },
                    GlobalSettings = new GlobalSettingsCashoutModel
                    {
                        TwoFactorEnabled = false,
                        CashOutBlocked = false, 

                        FeeSettings = new FeeSettingsCashoutModel
                        {
                            TargetClients = new Dictionary<string, string>
                            {
                                { "Cashout", aggregate.FeeCashoutTargetClientId }
                            }
                        },
                    }
                }, OperationsBoundedContext.Name);
                _chaosKitty.Meow(aggregate.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        #region CashoutFinishEvents

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
        private async Task Handle(Service.Operations.Contracts.Events.OperationFailedEvent evt,
            ICommandSender sender)
        {
            if (evt.ErrorCode == "DuplicatedOperation")
            {
                _log.Warning($"Duplicated operation: {nameof(OperationFailedEvent)} ignored", context: evt);

                return;
            }

            await HandleOperationFinishEvent(evt.OperationId, sender);
        }

        private async Task HandleOperationFinishEvent(Guid operationId, ICommandSender sender)
        {
            var aggregate = await _repository.TryGetAsync(operationId);

            if (aggregate == null)
            {
                //this is not a heartbeat cashout operation
                return;
            }

            //TODO suggest to put timestamp in contract
            if (aggregate.OnFinished(DateTime.UtcNow))
            {
                sender.SendCommand(new RegisterHeartbeatCashoutLastMomentCommand
                {
                    AssetId = aggregate.AssetId,
                    Moment = aggregate.OperationFinishMoment ?? throw new ArgumentNullException(nameof(aggregate.OperationFinishMoment)),
                    OperationId = aggregate.OperationId
                }, HeartbeatCashoutBoundedContext.Name);

                _chaosKitty.Meow(operationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        #endregion

        [UsedImplicitly]
        private async Task Handle(CashoutLastMomentRegisteredEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.GetAsync(evt.OperationId);

            if (aggregate.OnLastMomentRegistered(evt.Moment))
            {
                sender.SendCommand(new ReleaseCashoutLockCommand
                    {
                        AssetId = aggregate.AssetId,
                        OperationId = aggregate.OperationId
                    },
                    HeartbeatCashoutBoundedContext.Name);

                _chaosKitty.Meow(evt.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }
    }
}
