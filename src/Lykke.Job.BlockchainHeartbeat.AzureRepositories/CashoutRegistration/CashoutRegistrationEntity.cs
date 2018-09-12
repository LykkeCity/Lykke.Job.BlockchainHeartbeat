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
        public DateTime StartMoment { get; set; }
        public DateTime? OperationFinishMoment { get; set; }

        public Guid OperationId { get; set; }
        public Guid ClientId { get; set; }
        public string BlockchainType { get; set; }
        public string BlockchainAssetId { get; set; }
        public string HotWalletAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Amount { get; set; }
        public string AssetId { get; set; }

        public CashoutRegistrationAggregate.State CurrentState { get; set; }

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
                OperationFinishMoment = aggregate.OperationFinishMoment,
                OperationId = aggregate.OperationId,
                ClientId = aggregate.ClientId,
                BlockchainType = aggregate.BlockchainType,
                BlockchainAssetId = aggregate.BlockchainAssetId,
                HotWalletAddress = aggregate.HotWalletAddress,
                ToAddress = aggregate.ToAddress,
                Amount = aggregate.Amount,
                AssetId = aggregate.AssetId,
                CurrentState = aggregate.CurrentState
            };
        }

        public CashoutRegistrationAggregate ToDomain()
        {
            return CashoutRegistrationAggregate.Restore(
                ETag,
                StartMoment,
                OperationFinishMoment,
                OperationId,
                ClientId,
                BlockchainType,
                BlockchainAssetId,
                HotWalletAddress,
                ToAddress,
                Amount,
                AssetId,
                CurrentState);
        }

        #endregion
    }
}
