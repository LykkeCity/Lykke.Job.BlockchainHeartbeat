using Lykke.Common.Chaos;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.HeartbeatCashout;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Sagas
{
    public class HeartBeatCashoutSaga
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly IHeartbeatCashoutRepository _repository;

        public HeartBeatCashoutSaga(IChaosKitty chaosKitty, IHeartbeatCashoutRepository repository)
        {
            _chaosKitty = chaosKitty;
            _repository = repository;
        }
    }
}
