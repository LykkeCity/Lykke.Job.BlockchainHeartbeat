﻿using System;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Events.HeartbeatCashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class HeartbeatCashoutStartedEvent
    {
        public Guid ClientId { get; set; }
        public Guid OperationId { get; set; }
        public string ToAddress { get; set; }
        public string ToAddressExtension { get; set; }
        public decimal Amount { get; set; }
        public string AssetId { get; set; }
        public TimeSpan MaxCashoutInactivePeriod { get; set; }

        public string FeeCashoutTargetClientId { get; set; }

        public decimal ClientBalance { get; set; }
    }
}
