using System;
using System.Net.Http;
using Autofac;
using Lykke.Common.Chaos;
using Lykke.Job.BlockchainHeartbeat.Core.Services;
using Lykke.Job.BlockchainHeartbeat.Services;
using Lykke.Job.BlockchainHeartbeat.Settings;
using Lykke.Job.BlockchainHeartbeat.Settings.JobSettings;
using Lykke.Job.BlockchainHeartbeat.Workflow.PeriodicalHandlers;
using Lykke.Job.BlockchainHeartbeat.Workflow.Settings;
using Lykke.Service.Assets.Client;

namespace Lykke.Job.BlockchainHeartbeat.Modules
{
    public class JobModule : Module
    {
        private readonly BlockchainHeartbeatSettings _settings;
        private readonly AssetServiceClientSettings _assetServiceClientSettings;

        public JobModule(BlockchainHeartbeatSettings settings, AssetServiceClientSettings assetServiceClientSettings)
        {
            _settings = settings;
            _assetServiceClientSettings = assetServiceClientSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();
            
            builder.RegisterInstance(new AssetsService(new Uri(_assetServiceClientSettings.ServiceUrl), new HttpClient()))
                .As<IAssetsService>()
                .SingleInstance();

            builder.RegisterChaosKitty(_settings.ChaosKitty);

            foreach (var assetSettings in _settings.Cashout.Assets)
            {
                var periodicalHandlerSettings = new HeartbeatCashoutPeriodicalHandlerSettings
                {
                    Amount = assetSettings.Amount,
                    AssetId = assetSettings.AssetId,
                    ToAddress = assetSettings.ToAddress,
                    ToAddressExtension = assetSettings.ToAddressExtensions,
                    MaxCashoutInactivePeriod = assetSettings.MaxCashoutInactivePeriod,
                    ClientId =  _settings.Cashout.User.ClientId,
                    FeeCashoutTargetClientId = _settings.Cashout.FeeCashoutTargetClientId,
                    ClientBalance = _settings.Cashout.User.Balance
                };

                builder.RegisterType<HeartbeatCashoutPeriodicalHandler>()
                    .WithParameter(TypedParameter.From(periodicalHandlerSettings))
                    .WithParameter(TypedParameter.From(_settings.Cashout.TimerPeriod))
                    .As<IStartable>()
                    .SingleInstance();
            }
        }
    }
}
