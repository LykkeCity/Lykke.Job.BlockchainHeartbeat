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
        public Guid ClientId { get; set; }
        public string ToAddress { get; set; }
        public decimal Amount { get; set; }
        public string AssetId { get; set; }

        public HeartbeatCashoutAggregate.State CurrentState { get; set; }

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
                ClientId = aggregate.ClientId,
                ToAddress = aggregate.ToAddress,
                Amount = aggregate.Amount,
                AssetId = aggregate.AssetId,
                CurrentState = aggregate.CurrentState
            };
        }

        public HeartbeatCashoutAggregate ToDomain()
        {
            return HeartbeatCashoutAggregate.Restore(
                ETag,
                StartMoment,
                OperationFinishMoment,
                OperationId,
                ClientId,
                ToAddress,
                Amount,
                AssetId,
                CurrentState);
        }

        #endregion
    }
}
