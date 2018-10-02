using Autofac;
using Lykke.Common.Chaos;
using Lykke.Job.BlockchainHeartbeat.Core.Services;
using Lykke.Job.BlockchainHeartbeat.Services;
using Lykke.Job.BlockchainHeartbeat.Services.WalletApiV2;
using Lykke.Job.BlockchainHeartbeat.Settings.JobSettings;
using Lykke.Job.BlockchainHeartbeat.Workflow.CommandHandlers.HeartbeatCashout.Settings;
using Lykke.Job.BlockchainHeartbeat.Workflow.PeriodicalHandlers;

namespace Lykke.Job.BlockchainHeartbeat.Modules
{
    public class JobModule : Module
    {
        private readonly BlockchainHeartbeatSettings _settings;

        public JobModule(BlockchainHeartbeatSettings settings)
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
            
            builder.RegisterType<WalletApiV2Provider>()
                .WithParameter(TypedParameter.From(_settings.WalletApiV2Url))
                .As<IWalletApiV2Provider>();

            builder.RegisterInstance(new CryptoCashoutUserSettings
            {
                ClientInfo = _settings.Cashout.User.ClientInfo,
                Email = _settings.Cashout.User.Email,
                PartnerId = _settings.Cashout.User.PartnerId,
                Password = _settings.Cashout.User.Password
            }).SingleInstance();

            builder.RegisterChaosKitty(_settings.ChaosKitty);

            foreach (var assetSettings in _settings.Cashout.Assets)
            {
                var periodicalHandlerSettings = new HeartbeatCashoutPeriodicalHandlerSettings
                {
                    Amount = assetSettings.Amount,
                    AssetId = assetSettings.AssetId,
                    ToAddress = assetSettings.ToAddress,
                    ToAddressExtension = assetSettings.ToAddressExtensions,
                    MaxCashoutInactivePeriod = assetSettings.MaxCashoutInactivePeriod
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
