using System;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutRegistration;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainHeartbeat.AzureRepositories.CashoutRegistration
{
    public class CashoutRegistrationRepository: ICashoutRegistrationRepository
    {
        private readonly AggregateRepository<CashoutRegistrationAggregate, CashoutRegistrationEntity> _aggregateRepository;

        public static ICashoutRegistrationRepository Create(
            IReloadingManager<string> connectionString,
            ILogFactory log)
        {
            var storage = AzureTableStorage<CashoutRegistrationEntity>.Create(
                connectionString,
                "HeartbeatCashoutRegistrations",
                log);

            return new CashoutRegistrationRepository(storage);
        }

        private CashoutRegistrationRepository(INoSQLTableStorage<CashoutRegistrationEntity> storage)
        {
            _aggregateRepository = new AggregateRepository<CashoutRegistrationAggregate, CashoutRegistrationEntity>(
                storage,
                mapAggregateToEntity: CashoutRegistrationEntity.FromDomain,
                mapEntityToAggregate: entity => Task.FromResult(entity.ToDomain()));
        }

        public Task<CashoutRegistrationAggregate> GetOrAddAsync(Guid aggregateId, Func<CashoutRegistrationAggregate> newAggregateFactory)
        {
            return _aggregateRepository.GetOrAddAsync(aggregateId, newAggregateFactory);
        }

        public Task<CashoutRegistrationAggregate> GetAsync(Guid operationId)
        {
            return _aggregateRepository.GetAsync(operationId);
        }

        public Task SaveAsync(CashoutRegistrationAggregate aggregate)
        {
            return _aggregateRepository.SaveAsync(aggregate);
        }

        public Task<CashoutRegistrationAggregate> TryGetAsync(Guid aggregateId)
        {
            return _aggregateRepository.TryGetAsync(aggregateId);
        }
    }
}
