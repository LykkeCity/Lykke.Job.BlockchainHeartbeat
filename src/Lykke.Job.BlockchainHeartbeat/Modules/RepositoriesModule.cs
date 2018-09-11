using Autofac;
using Common.Log;
using Lykke.Job.BlockchainHeartbeat.Settings.JobSettings;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainHeartbeat.Modules
{
    public class RepositoriesModule : Module
    {
        private readonly IReloadingManager<DbSettings> _dbSettings;
        private readonly ILog _log;

        public RepositoriesModule(
            IReloadingManager<DbSettings> dbSettings,
            ILog log)
        {
            _log = log;
            _dbSettings = dbSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {

        }
    }
}
