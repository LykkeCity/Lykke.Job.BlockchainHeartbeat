using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Lykke.Job.BlockchainHeartbeat.Settings.JobSettings;

namespace Lykke.Job.BlockchainHeartbeat.Settings
{
    public class CashoutSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public IEnumerable<HeartbeatAssetSettings> Assets { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public TimeSpan TimerPeriod { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public HeartBeatUserSettings User { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string FeeCashoutTargetClientId { get; set; }
    }
}
