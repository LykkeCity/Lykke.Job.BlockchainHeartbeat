using System;
using Common;
using Lykke.AzureStorage.Tables;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutRegistration;

namespace Lykke.Job.BlockchainHeartbeat.AzureRepositories.CashoutRegistration
{
    internal class CashoutRegistrationEntity:AzureTableEntity
    {
        #region Fields

        // ReSharper disable MemberCanBePrivate.Global
        public DateTime? StartMoment { get; set; }
        public Guid OperationId { get; set; }
        public string AssetId { get; set; }
        public DateTime? FinishMoment { get; set; }

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

        public static CashoutRegistrationEntity FromDomain(CashoutRegistrationAggregate aggregate)
        {
            return new CashoutRegistrationEntity
            {
                ETag = string.IsNullOrEmpty(aggregate.Version) ? "*" : aggregate.Version,
                PartitionKey = GetPartitionKey(aggregate.OperationId),
                RowKey = GetRowKey(aggregate.OperationId),
                StartMoment = aggregate.StartMoment,
                OperationId = aggregate.OperationId,
                AssetId = aggregate.AssetId,
                FinishMoment = aggregate.FinishMoment
            };
        }

        public CashoutRegistrationAggregate ToDomain()
        {
            return CashoutRegistrationAggregate.Restore(
                ETag,
                StartMoment,
                OperationId,
                AssetId,
                FinishMoment);
        }

        #endregion
    }
}
