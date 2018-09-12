using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Common.Chaos;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutRegistration;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.HeartbeatCashout;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.Sagas
{
    public class CashoutRegistrationSaga
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly ICashoutRegistrationRepository _repository;

        public CashoutRegistrationSaga(IChaosKitty chaosKitty, ICashoutRegistrationRepository repository)
        {
            _chaosKitty = chaosKitty;
            _repository = repository;
        }
    }
}
