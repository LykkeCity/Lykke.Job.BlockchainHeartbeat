﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutLock;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.LastCashoutEventMoment;
using Lykke.Job.BlockchainHeartbeat.Workflow.BoundedContexts;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.Sagas;
using Lykke.Job.BlockchainHeartbeat.Workflow.Settings;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.PeriodicalHandlers
{
    public class HeartbeatCashoutStarterPeriodicalHandler:IStartable, IStopable
    {
        private readonly ITimerTrigger _timer;
        private readonly HeartbeatCashoutPeriodicalHandlerSettings _settings;
        private readonly ILastCashoutEventMomentRepository _lastMomentRepo;
        private DateTime _startedAt;
        private readonly ICashoutLockRepository _cashoutLockRepository;
        private readonly ICqrsEngine _cqrsEngine;
        private readonly ILog _log;

        public HeartbeatCashoutStarterPeriodicalHandler(
            HeartbeatCashoutPeriodicalHandlerSettings settings,
            TimeSpan timerPeriod,
            ILogFactory logFactory, 
            ILastCashoutEventMomentRepository lastMomentRepo, 
            ICashoutLockRepository cashoutLockRepository, 
            ICqrsEngine cqrsEngine)
        {
            _settings = settings;
            _lastMomentRepo = lastMomentRepo;
            _cashoutLockRepository = cashoutLockRepository;
            _cqrsEngine = cqrsEngine;
            _log = logFactory.CreateLog(this);

            _timer = new TimerTrigger(
                $"{nameof(HeartbeatCashoutStarterPeriodicalHandler)} : {settings.AssetId}",
                timerPeriod,
                logFactory);

            _timer.Triggered += Execute;

        }

        public void Start()
        {
            _log.Info($"Starting {nameof(HeartbeatCashoutStarterPeriodicalHandler)}", 
                context: _settings);

            _timer.Start();
            _startedAt = DateTime.UtcNow;
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
            var checkDate = await _lastMomentRepo.GetLastEventMomentAsync(_settings.AssetId) 
                            ?? _startedAt; // run heartbeat with delay if there are no cashout registered

            if (DateTime.UtcNow - checkDate > _settings.MaxCashoutInactivePeriod &&
                !await _cashoutLockRepository.IsLockedAsync(_settings.AssetId))
            {
                var opId = Guid.NewGuid();

                _log.Info("Starting heartbeat cashout", context:new { opId, checkDate, _settings.AssetId});

                _cqrsEngine.SendCommand(new StartHeartbeatCashoutCommand
                    {
                        Amount = _settings.Amount,
                        AssetId = _settings.AssetId,
                        OperationId = opId,
                        ToAddress = _settings.ToAddress,
                        ToAddressExtension = _settings.ToAddressExtension,
                        MaxCashoutInactivePeriod = _settings.MaxCashoutInactivePeriod,
                        ClientId = _settings.ClientId,
                        FeeCashoutTargetClientId = _settings.FeeCashoutTargetClientId,
                        ClientBalance = _settings.ClientBalance
                    }, 
                    HeartbeatCashoutBoundedContext.Name, 
                    HeartbeatCashoutBoundedContext.Name);
            }
        }
    }
}
