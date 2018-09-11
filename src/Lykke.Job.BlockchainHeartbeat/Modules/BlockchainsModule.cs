using System.Linq;
using Autofac;
using Common.Log;
using Lykke.Job.BlockchainHeartbeat.Settings.Blockchain;
using Lykke.Job.BlockchainHeartbeat.Settings.JobSettings;
using Lykke.Service.BlockchainApi.Client;
using Lykke.Service.BlockchainSignFacade.Client;

namespace Lykke.Job.BlockchainHeartbeat.Modules
{
    public class BlockchainsModule : Module
    {
        private readonly BlockchainHearbeatSettings _blockchainHearbeatSettings;
        private readonly BlockchainsIntegrationSettings _blockchainsIntegrationSettings;
        private readonly ILog _log;

        public BlockchainsModule(
            BlockchainHearbeatSettings blockchainHearbeatSettings,
            BlockchainsIntegrationSettings blockchainsIntegrationSettings,
            ILog log)
        {
            _blockchainHearbeatSettings = blockchainHearbeatSettings;
            _blockchainsIntegrationSettings = blockchainsIntegrationSettings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {

        }
    }
}
