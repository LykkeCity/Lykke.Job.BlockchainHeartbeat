using System;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutRegistration;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainHeartbeat.AzureRepositories.CashoutRegistration
{
    public class CashoutFinishRegistrationRepository: ICashoutFinishRegistrationRepository
    {
        private readonly AggregateRepository<CashoutFinishRegistrationAggregate, CashoutRegistrationEntity> _aggregateRepository;

        public static ICashoutFinishRegistrationRepository Create(
            IReloadingManager<string> connectionString,
            ILogFactory log)
        {
            var storage = AzureTableStorage<CashoutRegistrationEntity>.Create(
                connectionString,
                "HeartbeatCashoutRegistrations",
                log);

            return new CashoutFinishRegistrationRepository(storage);
        }

        private CashoutFinishRegistrationRepository(INoSQLTableStorage<CashoutRegistrationEntity> storage)
        {
            _aggregateRepository = new AggregateRepository<CashoutFinishRegistrationAggregate, CashoutRegistrationEntity>(
                storage,
                mapAggregateToEntity: CashoutRegistrationEntity.FromDomain,
                mapEntityToAggregate: entity => Task.FromResult(entity.ToDomain()));
        }

        public Task<CashoutFinishRegistrationAggregate> GetOrAddAsync(Guid aggregateId, Func<CashoutFinishRegistrationAggregate> newAggregateFactory)
        {
            return _aggregateRepository.GetOrAddAsync(aggregateId, newAggregateFactory);
        }

        public Task<CashoutFinishRegistrationAggregate> GetAsync(Guid operationId)
        {
            return _aggregateRepository.GetAsync(operationId);
        }

        public Task SaveAsync(CashoutFinishRegistrationAggregate aggregate)
        {
            return _aggregateRepository.SaveAsync(aggregate);
        }

        public Task<CashoutFinishRegistrationAggregate> TryGetAsync(Guid aggregateId)
        {
            return _aggregateRepository.TryGetAsync(aggregateId);
        }
    }
}
