using System;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.HeartbeatCashout;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainHeartbeat.AzureRepositories.HeartbeatCashout
{
    public class HeartbeatCashoutRepository: IHeartbeatCashoutRepository
    {
        private readonly AggregateRepository<HeartbeatCashoutAggregate, HeartbeatCashoutEntity> _aggregateRepository;

        public static IHeartbeatCashoutRepository Create(
            IReloadingManager<string> connectionString,
            ILogFactory log)
        {
            var storage = AzureTableStorage<HeartbeatCashoutEntity>.Create(
                connectionString,
                "HearbeatCashouts",
                log);

            return new HeartbeatCashoutRepository(storage);
        }

        private HeartbeatCashoutRepository(INoSQLTableStorage<HeartbeatCashoutEntity> storage)
        {
            _aggregateRepository = new AggregateRepository<HeartbeatCashoutAggregate, HeartbeatCashoutEntity>(
                storage,
                mapAggregateToEntity: HeartbeatCashoutEntity.FromDomain,
                mapEntityToAggregate: entity => Task.FromResult(entity.ToDomain()));
        }

        public Task<HeartbeatCashoutAggregate> GetOrAddAsync(Guid aggregateId, Func<HeartbeatCashoutAggregate> newAggregateFactory)
        {
            return _aggregateRepository.GetOrAddAsync(aggregateId, newAggregateFactory);
        }

        public Task<HeartbeatCashoutAggregate> GetAsync(Guid operationId)
        {
            return _aggregateRepository.GetAsync(operationId);
        }

        public Task SaveAsync(HeartbeatCashoutAggregate aggregate)
        {
            return _aggregateRepository.SaveAsync(aggregate);
        }

        public Task<HeartbeatCashoutAggregate> TryGetAsync(Guid aggregateId)
        {
            return _aggregateRepository.TryGetAsync(aggregateId);
        }
    }
}
