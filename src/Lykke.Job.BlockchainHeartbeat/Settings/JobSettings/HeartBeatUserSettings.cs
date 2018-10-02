using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainHeartbeat.Settings.JobSettings
{
    public class HeartBeatUserSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Email { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Password { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string PartnerId { get; set; }

        [Optional]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string ClientInfo { get; set; }
    }
}
