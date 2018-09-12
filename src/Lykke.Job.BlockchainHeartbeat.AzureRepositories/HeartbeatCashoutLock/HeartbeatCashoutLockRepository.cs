using System;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.HeartbeatCashoutLock;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainHeartbeat.AzureRepositories.HeartbeatCashoutLock
{
    public class HeartbeatCashoutLockRepository:IHeartbeatCashoutLockRepository
    {
        private readonly INoSQLTableStorage<HeartbeatCashoutLockEntity> _storage;

        public static IHeartbeatCashoutLockRepository Create(IReloadingManager<string> connectionString, ILogFactory logFactory)
        {
            var storage = AzureTableStorage<HeartbeatCashoutLockEntity>.Create(
                connectionString,
                "HeartbeatCashoutLocks",
                logFactory);

            return new HeartbeatCashoutLockRepository(storage);
        }

        private HeartbeatCashoutLockRepository(INoSQLTableStorage<HeartbeatCashoutLockEntity> storage)
        {
            _storage = storage;
        }

        public async Task<bool> TryGetLockAsync(string blockchainType, string assetId, Guid operationId)
        {
            var partitionKey = HeartbeatCashoutLockEntity.GetPartitionKey(blockchainType);
            var rowKey = HeartbeatCashoutLockEntity.GetRowKey(assetId);


            var lockEntity = await _storage.GetOrInsertAsync(partitionKey, rowKey,
                () => new HeartbeatCashoutLockEntity
                {
                    PartitionKey = partitionKey,
                    RowKey = rowKey,
                    OperationId = operationId
                });

            return lockEntity.OperationId == operationId;
        }

        public async Task<bool> ReleaseLockAsync(string blockchainType, string assetId, Guid operationId)
        {
            var partitionKey = HeartbeatCashoutLockEntity.GetPartitionKey(blockchainType);
            var rowKey = HeartbeatCashoutLockEntity.GetRowKey(assetId);

            return await _storage.DeleteIfExistAsync(
                partitionKey,
                rowKey,
                // Exactly the given transaction should own current lock to remove it
                lockEntity => lockEntity.OperationId == operationId);
        }
    }
}
