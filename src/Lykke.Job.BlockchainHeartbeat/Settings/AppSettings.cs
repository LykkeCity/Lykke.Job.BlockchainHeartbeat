using JetBrains.Annotations;
using Lykke.Job.BlockchainHeartbeat.Settings.JobSettings;
using Lykke.Job.BlockchainHeartbeat.Settings.SlackNotifications;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainHeartbeat.Settings
{
    [UsedImplicitly]
    public class AppSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public BlockchainHeartbeatSettings BlockchainHeartbeatJob { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public SlackNotificationsSettings SlackNotifications { get; set; }

        [Optional]
        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public AssetServiceClientSettings AssetsServiceClient { get; set; }
    }
}
