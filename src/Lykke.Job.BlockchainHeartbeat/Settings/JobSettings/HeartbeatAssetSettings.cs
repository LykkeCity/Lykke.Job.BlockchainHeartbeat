using System;
using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainHeartbeat.Settings.JobSettings
{
    public class HeartbeatAssetSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string AssetId { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string ToAddress { get; set; }

        [Optional]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string ToAddressExtensions { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public decimal Amount { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public TimeSpan MaxCashoutInactivePeriod { get; set; }
    }
}
