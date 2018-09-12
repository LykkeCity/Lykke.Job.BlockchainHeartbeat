using System;
using Lykke.AzureStorage.Tables;

namespace Lykke.Job.BlockchainHeartbeat.AzureRepositories.LastCashoutEventMoment
{
    internal class LastCashoutEventMomentEntity:AzureTableEntity
    {
        public string BlockchainType { get; set; }

        public string AssetId { get; set; }

        public DateTime Moment { get; set; }

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
