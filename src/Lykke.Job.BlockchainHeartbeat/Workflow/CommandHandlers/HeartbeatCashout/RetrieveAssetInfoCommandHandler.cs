using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.Events.HeartbeatCashout;
using Lykke.Service.Assets.Client;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.CommandHandlers.HeartbeatCashout
{
    public class RetrieveAssetInfoCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly IAssetsService _assetsService;

        public RetrieveAssetInfoCommandHandler(IChaosKitty chaosKitty, IAssetsService assetsService)
        {
            _chaosKitty = chaosKitty;
            _assetsService = assetsService;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RetrieveAssetInfoCommand command,
            IEventPublisher publisher)
        {
            var asset = await _assetsService.AssetGetAsync(command.AssetId);

            publisher.PublishEvent(new AssetInfoRetrievedEvent
            {
                DisplayId = asset.DisplayId,
                MultiplierPower = asset.MultiplierPower,
                AssetAddress = asset.AssetAddress,
                Accuracy = asset.Accuracy,
                BlockchainIntegrationLayerId = asset.BlockchainIntegrationLayerId,
                Blockchain = asset.Blockchain.ToString(),
                Type = asset.Type?.ToString(),
                IsTradable = asset.IsTradable,
                IsTrusted = asset.IsTrusted,
                KycNeeded = asset.KycNeeded,
                BlockchainWithdrawal = asset.BlockchainWithdrawal,
                CashoutMinimalAmount = (decimal)asset.CashoutMinimalAmount,
                LowVolumeAmount = (decimal?)asset.LowVolumeAmount ?? 0,
                LykkeEntityId = asset.LykkeEntityId,
                Moment = DateTime.UtcNow
            });

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
