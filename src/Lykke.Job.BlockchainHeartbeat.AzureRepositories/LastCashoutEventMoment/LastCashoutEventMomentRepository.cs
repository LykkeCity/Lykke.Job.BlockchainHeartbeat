using System;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.LastCashoutEventMoment;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainHeartbeat.AzureRepositories.LastCashoutEventMoment
{
    public class LastCashoutEventMomentRepository: ILastCashoutEventMomentRepository
    {
        private readonly INoSQLTableStorage<LastCashoutEventMomentEntity> _storage;

        public static ILastCashoutEventMomentRepository Create(IReloadingManager<string> connectionString, ILogFactory logFactory)
        {
            var storage = AzureTableStorage<LastCashoutEventMomentEntity>.Create(
                connectionString,
                "LastCashoutEventMoments",
                logFactory);

            return new LastCashoutEventMomentRepository(storage);
        }

        private LastCashoutEventMomentRepository(INoSQLTableStorage<LastCashoutEventMomentEntity> storage)
        {
            _storage = storage;
        }

        public Task SetLastCashoutEventMomentAsync(string blockchainType, string assetId, DateTime eventMoment)
        {
            return _storage.InsertOrReplaceAsync(new LastCashoutEventMomentEntity
            {
                AssetId = assetId,
                BlockchainType = blockchainType,
                Moment = eventMoment,
                PartitionKey = LastCashoutEventMomentEntity.GetPartitionKey(blockchainType),
                RowKey = LastCashoutEventMomentEntity.GetRowKey(assetId)
            }, p => p.Moment <= eventMoment);
        }

        public async Task<DateTime?> GetLastEventMomentAsync(string blockchainType, string assetId)
        {
            return (await _storage.GetDataAsync(
                    LastCashoutEventMomentEntity.GetPartitionKey(blockchainType),
                    LastCashoutEventMomentEntity.GetRowKey(assetId)))
                ?.Moment;
        }
    }
}
