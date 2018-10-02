using System;
using Common;
using Lykke.AzureStorage.Tables;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.HeartbeatCashout;

namespace Lykke.Job.BlockchainHeartbeat.AzureRepositories.HeartbeatCashout
{
    internal class HeartbeatCashoutEntity : AzureTableEntity
    {
        #region Fields

        // ReSharper disable MemberCanBePrivate.Global
        public DateTime StartMoment { get; set; }
        public DateTime? OperationFinishMoment { get; set; }

        public Guid OperationId { get; set; }
        public string ToAddress { get; set; }
        public string ToAddressExtension { get; set; }
        public decimal Amount { get; set; }
        public string AssetId { get; set; }

        public HeartbeatCashoutAggregate.State CurrentState { get; set; }

        public DateTime? LockAcquiredAt { get; set; }
        public DateTime? LockRejectedAt { get; set; }
        public DateTime? LastMomentRegisteredAt { get; set; }

        public TimeSpan MaxCashoutInactivePeriod { get; set; }

        // ReSharper restore MemberCanBePrivate.Global

        #endregion


        #region Keys

        public static string GetPartitionKey(Guid operationId)
        {
            // Use hash to distribute all records to the different partitions
            var hash = operationId.ToString().CalculateHexHash32(3);

            return $"{hash}";
        }

        public static string GetRowKey(Guid operationId)
        {
            return $"{operationId:D}";
        }

        #endregion


        #region Conversion

        public static HeartbeatCashoutEntity FromDomain(HeartbeatCashoutAggregate aggregate)
        {
            return new HeartbeatCashoutEntity
            {
                ETag = string.IsNullOrEmpty(aggregate.Version) ? "*" : aggregate.Version,
                PartitionKey = GetPartitionKey(aggregate.OperationId),
                RowKey = GetRowKey(aggregate.OperationId),
                StartMoment = aggregate.StartMoment,
                OperationFinishMoment = aggregate.OperationFinishMoment,
                OperationId = aggregate.OperationId,
                ToAddress = aggregate.ToAddress,
                ToAddressExtension = aggregate.ToAddressExtension,
                Amount = aggregate.Amount,
                AssetId = aggregate.AssetId,
                CurrentState = aggregate.CurrentState,
                LockAcquiredAt = aggregate.LockAcquiredAt,
                LockRejectedAt = aggregate.LockRejectedAt,
                LastMomentRegisteredAt = aggregate.LastMomentRegisteredAt,
                MaxCashoutInactivePeriod = aggregate.MaxCashoutInactivePeriod
            };
        }

        public HeartbeatCashoutAggregate ToDomain()
        {
            return HeartbeatCashoutAggregate.Restore(
                ETag,
                StartMoment,
                OperationFinishMoment,
                OperationId,
                ToAddress,
                ToAddressExtension,
                Amount,
                AssetId,
                CurrentState,
                LockAcquiredAt,
                LockRejectedAt,
                LastMomentRegisteredAt,
                MaxCashoutInactivePeriod);
        }

        #endregion
    }
}
