using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class StartCryptoCashoutCommand
    {
        public Guid OperationId { get; set; }
        public string AssetId { get; set; }
        public string ToAddress { get; set; }
        public string ToAddressExtension { get; set; }
        public decimal Amount { get; set; }
    }
}
