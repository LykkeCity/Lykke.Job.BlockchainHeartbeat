﻿using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Job.BlockchainHeartbeat.Settings.Blockchain
{
    [UsedImplicitly]
    public class BlockchainsIntegrationSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public IReadOnlyList<BlockchainSettings> Blockchains { get; set; }
    }
}
