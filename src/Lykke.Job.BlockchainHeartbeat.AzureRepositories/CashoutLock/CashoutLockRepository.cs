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

        public async Task<bool> TryGetLockAsync(string blockchainType, string assetId, Guid operationId)
        {
            var partitionKey = CashoutLockEntity.GetPartitionKey(blockchainType);
            var rowKey = CashoutLockEntity.GetRowKey(assetId);


            var lockEntity = await _storage.GetOrInsertAsync(partitionKey, rowKey,
                () => new CashoutLockEntity
                {
                    PartitionKey = partitionKey,
                    RowKey = rowKey,
                    OperationId = operationId
                });

            return lockEntity.OperationId == operationId;
        }

        public async Task<bool> ReleaseLockAsync(string blockchainType, string assetId, Guid operationId)
        {
            var partitionKey = CashoutLockEntity.GetPartitionKey(blockchainType);
            var rowKey = CashoutLockEntity.GetRowKey(assetId);

            return await _storage.DeleteIfExistAsync(
                partitionKey,
                rowKey,
                // Exactly the given transaction should own current lock to remove it
                lockEntity => lockEntity.OperationId == operationId);
        }
    }
}
