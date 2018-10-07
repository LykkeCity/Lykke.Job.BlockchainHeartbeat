using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout
{
    public class RetrieveAssetInfoCommand
    {
        public Guid OperationId { get; set; }

        public string AssetId { get; set; }
    }
}
