﻿using System;
using MessagePack;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Events.HeartbeatCashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class CashoutPreconditionPassedEvent
    {
        public Guid OperationId { get; set; }

        public DateTime Moment { get; set; }
    }
}
