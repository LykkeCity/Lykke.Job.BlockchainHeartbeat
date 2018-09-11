using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Common.Chaos;
using Lykke.Common.Log;
using Lykke.Job.BlockchainHeartbeat.Core.Services;
using Lykke.Job.BlockchainHeartbeat.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.BlockchainHeartbeat.Modules
{
    public class JobModule : Module
    {
        private readonly ChaosSettings _chaosSettings;
        private readonly ServiceCollection _services;

        public JobModule(
            ChaosSettings chaosSettings)
        {
            _chaosSettings = chaosSettings;
            _services = new ServiceCollection();
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

            builder.RegisterChaosKitty(_chaosSettings);

            builder.Populate(_services);
        }
    }
}
