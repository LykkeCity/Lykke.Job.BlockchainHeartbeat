using Autofac;
using Lykke.Common.Log;
using Lykke.Job.BlockchainHeartbeat.AzureRepositories.CashoutLock;
using Lykke.Job.BlockchainHeartbeat.AzureRepositories.CashoutRegistration;
using Lykke.Job.BlockchainHeartbeat.AzureRepositories.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.AzureRepositories.LastCashoutEventMoment;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutLock;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutRegistration;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.LastCashoutEventMoment;
using Lykke.Job.BlockchainHeartbeat.Settings.JobSettings;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainHeartbeat.Modules
{
    public class RepositoriesModule : Module
    {
        private readonly IReloadingManager<DbSettings> _dbSettings;

        public RepositoriesModule(IReloadingManager<DbSettings> dbSettings)
        {
            _dbSettings = dbSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(p => CashoutRegistrationRepository.Create(
                    _dbSettings.Nested(x => x.DataConnString),
                    p.Resolve<ILogFactory>()))
                .As<ICashoutRegistrationRepository>()
                .SingleInstance();

            builder.Register(p => HeartbeatCashoutRepository.Create(
                    _dbSettings.Nested(x => x.DataConnString),
                    p.Resolve<ILogFactory>()))
                .As<IHeartbeatCashoutRepository>()
                .SingleInstance();

            builder.Register(p => CashoutLockRepository.Create(
                    _dbSettings.Nested(x => x.DataConnString),
                    p.Resolve<ILogFactory>()))
                .As<ICashoutLockRepository>()
                .SingleInstance();

            builder.Register(p => LastCashoutEventMomentRepository.Create(
                    _dbSettings.Nested(x => x.DataConnString),
                    p.Resolve<ILogFactory>()))
                .As<ILastCashoutEventMomentRepository>()
                .SingleInstance();
        }
    }
}
