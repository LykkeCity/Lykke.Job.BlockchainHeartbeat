using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout
{
    public class StartCryptoCashoutCommand
    {
        public Guid OperationId { get; set; }
        public string AssetId { get; set; }
        public string ToAddress { get; set; }
        public string ToAddressExtension { get; set; }
        public decimal Amount { get; set; }
    }
}
