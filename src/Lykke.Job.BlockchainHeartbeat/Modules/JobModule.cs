using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Common.Chaos;
using Lykke.Common.Log;
using Lykke.Job.BlockchainHeartbeat.Core.Services;
using Lykke.Job.BlockchainHeartbeat.Services;
using Lykke.Job.BlockchainHeartbeat.Settings.JobSettings;
using Lykke.Job.BlockchainHeartbeat.Workflow.PeriodicalHandlers;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.BlockchainHeartbeat.Modules
{
    public class JobModule : Module
    {
        private readonly BlockchainHearbeatSettings _settings;

        public JobModule(BlockchainHearbeatSettings settings)
        {
            _settings = settings;
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

            builder.RegisterChaosKitty(_settings.ChaosKitty);

            foreach (var assetSettings in _settings.Assets)
            {
                var periodicalHandlerSettings = new HeartbeatCashoutPeriodicalHandlerSettings
                {
                    Amount = assetSettings.Amount,
                    AssetId = assetSettings.AssetId,
                    ClientId = assetSettings.ClientId,
                    ToAddress = assetSettings.ToAddress,
                    MaxCashoutInactivePeriod = assetSettings.MaxCashoutInactivePeriod
                };

                builder.RegisterType<HeartbeatCashoutPeriodicalHandler>()
                    .WithParameter(TypedParameter.From(periodicalHandlerSettings))
                    .WithParameter(TypedParameter.From(_settings.TimerPeriod))
                    .As<IStartable>()
                    .SingleInstance();
            }
        }
    }
}
