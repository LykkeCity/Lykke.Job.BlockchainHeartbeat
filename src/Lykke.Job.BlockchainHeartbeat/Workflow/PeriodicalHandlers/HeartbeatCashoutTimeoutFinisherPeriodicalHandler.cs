using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutLock;
using Lykke.Job.BlockchainHeartbeat.Workflow.BoundedContexts;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.Sagas;
using Lykke.Job.BlockchainHeartbeat.Workflow.Settings;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.PeriodicalHandlers
{
    public class HeartbeatCashoutTimeoutFinisherPeriodicalHandler : IStartable, IStopable
    {
        private readonly ITimerTrigger _timer;
        private readonly ILog _log;
        private readonly HeartbeatCashoutPeriodicalHandlerSettings _settings;
        private readonly ICqrsEngine _cqrsEngine;
        private readonly ICashoutLockRepository _cashoutLockRepository;

        public HeartbeatCashoutTimeoutFinisherPeriodicalHandler(HeartbeatCashoutPeriodicalHandlerSettings settings,
            TimeSpan timerPeriod,
            ILogFactory logFactory, 
            ICqrsEngine cqrsEngine,
            ICashoutLockRepository cashoutLockRepository)
        {
            _settings = settings;
            _cqrsEngine = cqrsEngine;
            _cashoutLockRepository = cashoutLockRepository;
            _log = logFactory.CreateLog(this);

            _timer = new TimerTrigger(
                $"{nameof(HeartbeatCashoutStarterPeriodicalHandler)} : {settings.AssetId}",
                timerPeriod,
                logFactory);

            _timer.Triggered += Execute;
        }

        public void Start()
        {
            _log.Info($"Starting {nameof(HeartbeatCashoutTimeoutFinisherPeriodicalHandler)}",
                context: _settings);

            _timer.Start();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private async Task Execute(ITimerTrigger timer,
            TimerTriggeredHandlerArgs args,
            CancellationToken cancellationToken)
        {
            var lockModel = await _cashoutLockRepository.GetLockAsync(_settings.AssetId);
            
            if (lockModel != null && DateTime.UtcNow - lockModel.Value.lockedAt > _settings.ExecutionTimeout)
            {
                _log.Warning("Cashout finished after timeout", context: new
                {
                    lockModel.Value.operationId, 
                    lockModel.Value.lockedAt,
                    _settings.ExecutionTimeout
                });

                _cqrsEngine.SendCommand(new ReleaseCashoutLockCommand
                    {
                        OperationId = lockModel.Value.operationId,
                        AssetId = _settings.AssetId
                    },
                    HeartbeatCashoutBoundedContext.Name,
                    HeartbeatCashoutBoundedContext.Name);
            }
        }
    }
}
