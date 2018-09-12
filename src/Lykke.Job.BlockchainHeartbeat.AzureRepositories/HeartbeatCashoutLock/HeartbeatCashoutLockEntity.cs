using System;
using Lykke.AzureStorage.Tables;

namespace Lykke.Job.BlockchainHeartbeat.AzureRepositories.HeartbeatCashoutLock
{
    internal class HeartbeatCashoutLockEntity:AzureTableEntity
    {
        public string BlockchainType { get; set; }

        public string AssetId { get; set; }

        public Guid OperationId { get; set; }

        public static string GetPartitionKey(string blockchainType)
        {
            return blockchainType;
        }

        public static string GetRowKey(string assetId)
        {
            return assetId;
        }
    }
}
