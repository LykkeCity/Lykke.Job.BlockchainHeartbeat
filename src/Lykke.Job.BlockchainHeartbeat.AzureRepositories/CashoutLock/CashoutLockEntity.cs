using System;
using Lykke.AzureStorage.Tables;

namespace Lykke.Job.BlockchainHeartbeat.AzureRepositories.CashoutLock
{
    internal class CashoutLockEntity:AzureTableEntity
    {
        public string AssetId { get; set; }

        public Guid OperationId { get; set; }
        
        public DateTime LockedAt { get; set; }

        public static string GetPartitionKey(string assetId)
        {
            return assetId;
        }

        public static string GetRowKey(string assetId)
        {
            return assetId;
        }
    }
}
