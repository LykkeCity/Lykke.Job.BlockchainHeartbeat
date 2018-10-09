using System;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutLock;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainHeartbeat.AzureRepositories.CashoutLock
{
    public class CashoutLockRepository:ICashoutLockRepository
    {
        private readonly INoSQLTableStorage<CashoutLockEntity> _storage;

        public static ICashoutLockRepository Create(IReloadingManager<string> connectionString, ILogFactory logFactory)
        {
            var storage = AzureTableStorage<CashoutLockEntity>.Create(
                connectionString,
                "HeartbeatCashoutLocks",
                logFactory);

            return new CashoutLockRepository(storage);
        }

        private CashoutLockRepository(INoSQLTableStorage<CashoutLockEntity> storage)
        {
            _storage = storage;
        }

        public async Task<bool> TryLockAsync(string assetId, Guid operationId, DateTime lockedAt)
        {
            var partitionKey = CashoutLockEntity.GetPartitionKey(assetId);
            var rowKey = CashoutLockEntity.GetRowKey(assetId);

            var lockEntity = await _storage.GetOrInsertAsync(partitionKey, rowKey,
                () => new CashoutLockEntity
                {
                    PartitionKey = partitionKey,
                    RowKey = rowKey,
                    OperationId = operationId,
                    AssetId = assetId,
                    LockedAt = lockedAt
                });

            return lockEntity?.OperationId == operationId;
        }

        public async Task<bool> ReleaseLockAsync(string assetId, Guid operationId)
        {
            var partitionKey = CashoutLockEntity.GetPartitionKey(assetId);
            var rowKey = CashoutLockEntity.GetRowKey(assetId);

            return await _storage.DeleteIfExistAsync(
                partitionKey,
                rowKey,
                // Exactly the given transaction should own current lock to remove it
                lockEntity => lockEntity.OperationId == operationId);
        }

        public async Task<bool> IsLockedAsync(string assetId)
        {
            var partitionKey = CashoutLockEntity.GetPartitionKey(assetId);
            var rowKey = CashoutLockEntity.GetRowKey(assetId);

            return await _storage.GetDataAsync(partitionKey, rowKey) != null;
        }

        public async Task<(DateTime lockedAt, Guid operationId)?> GetLockAsync(string assetId)
        {
            var partitionKey = CashoutLockEntity.GetPartitionKey(assetId);
            var rowKey = CashoutLockEntity.GetRowKey(assetId);

            var lockEntity = await _storage.GetDataAsync(partitionKey, rowKey);
            if (lockEntity != null)
            {
                return (lockEntity.LockedAt, lockEntity.OperationId);
            }

            return  null;
        }
    }
}
