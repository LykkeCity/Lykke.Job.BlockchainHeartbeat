using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Events.HeartbeatCashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class AssetInfoRetrievedEvent
    {
        public Guid OperationId { get; set; }

        public string DisplayId { get; set; }

        public int MultiplierPower { get; set; }

        public string AssetAddress { get; set; }

        public int Accuracy { get; set; }

        public string Blockchain { get; set; }

        public string Type { get; set; }

        public bool IsTradable { get; set; }

        public bool IsTrusted { get; set; }

        public bool KycNeeded { get; set; }

        public string BlockchainIntegrationLayerId { get; set; }

        public Decimal CashoutMinimalAmount { get; set; }

        public Decimal LowVolumeAmount { get; set; }

        public bool BlockchainWithdrawal { get; set; }

        public string LykkeEntityId { get; set; }

        public DateTime Moment { get; set; }
    }
}
