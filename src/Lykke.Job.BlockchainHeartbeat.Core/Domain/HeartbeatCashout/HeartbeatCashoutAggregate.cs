using System;

namespace Lykke.Job.BlockchainHeartbeat.Core.Domain.HeartbeatCashout
{
    public class HeartbeatCashoutAggregate
    {
        public string Version { get; }
        public DateTime StartMoment { get; }
        public DateTime? LockAcquiredAt { get; private set; }
        public DateTime? LockRejectedAt { get; private set; }
        public DateTime? LastMomentRegisteredAt { get; private set; }
        public DateTime? OperationFinishMoment { get; private set; }
        public Guid OperationId { get; }
        public string ToAddress { get; }
        public string ToAddressExtension { get; }
        public decimal Amount { get; }
        public string AssetId { get; }

        public TimeSpan MaxCashoutInactivePeriod { get; }

        public State CurrentState { get; private set; }

        private HeartbeatCashoutAggregate(string version,
            DateTime startMoment,
            DateTime? operationFinishMoment,
            Guid operationId,
            string toAddress,
            string toAddressExtension,
            decimal amount,
            string assetId,
            State currentState,
            DateTime? lockAcquiredAt,
            DateTime? lockRejectedAt,
            DateTime? lastMomentRegisteredAt,
            TimeSpan maxCashoutInactivePeriod)
        {
            Version = version;
            StartMoment = startMoment;
            OperationFinishMoment = operationFinishMoment;
            OperationId = operationId;
            ToAddress = toAddress;
            Amount = amount;
            AssetId = assetId;
            CurrentState = currentState;
            LockAcquiredAt = lockAcquiredAt;
            ToAddressExtension = toAddressExtension;
            LockRejectedAt = lockRejectedAt;
            LastMomentRegisteredAt = lastMomentRegisteredAt;
            MaxCashoutInactivePeriod = maxCashoutInactivePeriod;
        }

        public static HeartbeatCashoutAggregate StartNew(
            Guid operationId,
            string toAddress,
            string toAddressExtension,
            decimal amount,
            string assetId,
            TimeSpan maxCashoutInactivePeriod)
        {
            return new HeartbeatCashoutAggregate(version: null,
                startMoment: DateTime.UtcNow,
                operationFinishMoment: null,
                operationId: operationId,
                amount: amount, assetId: assetId,
                toAddress: toAddress,
                toAddressExtension:toAddressExtension,
                currentState: State.Started,
                lockAcquiredAt: null,
                lockRejectedAt: null,
                lastMomentRegisteredAt: null,
                maxCashoutInactivePeriod: maxCashoutInactivePeriod);
        }

        public static HeartbeatCashoutAggregate Restore(
            string version,
            DateTime startMoment,
            DateTime? operationFinishMoment,
            Guid operationId,
            string toAddress,
            string toAddressExtension,
            decimal amount,
            string assetId,
            State currentState,
            DateTime? lockAcquiredAt,
            DateTime? lockRejectedAt,
            DateTime? lastMomentRegisteredAt,
            TimeSpan maxCashoutInactivePeriod)
        {
            return new HeartbeatCashoutAggregate(version: version,
                startMoment: startMoment,
                operationFinishMoment: operationFinishMoment,
                operationId: operationId,
                amount: amount, assetId: assetId,
                toAddress: toAddress,
                toAddressExtension:toAddressExtension,
                currentState: currentState,
                lockAcquiredAt: lockAcquiredAt,
                lockRejectedAt: lockRejectedAt,
                lastMomentRegisteredAt: lastMomentRegisteredAt,
                maxCashoutInactivePeriod: maxCashoutInactivePeriod);
        }
        public bool OnStarted()
        {
            if (CurrentState == State.Started)
            {
                return true;
            }

            return false;
        }
        
        public bool OnLockAcquired(DateTime moment)
        {
            if (SwitchState(expectedState: State.Started, nextState: State.LockAcquired))
            {
                LockAcquiredAt = moment;

                return true;
            }

            return false;
        }

        public bool OnLockRejected(DateTime moment)
        {
            if (SwitchState(expectedState: State.Started, nextState: State.LockRejected))
            {
                LockRejectedAt = moment;

                return true;
            }

            return false;
        }

        public bool OnPreconditionPassed(DateTime moment)
        {
            if (SwitchState(expectedState: State.LockAcquired, nextState: State.PreconditionPassed))
            {
                return true;
            }

            return false;
        }

        public bool OnPreconditionRejected(DateTime moment)
        {
            if (SwitchState(expectedState: State.LockAcquired, nextState: State.PreconditionRejected))
            {
                return true;
            }

            return false;
        }

        public bool OnFinished(DateTime moment)
        {
            if (SwitchState(expectedState: State.PreconditionPassed, nextState: State.Finished))
            {
                OperationFinishMoment = moment;

                return true;
            }

            return false;
        }
        
        public bool OnLastMomentRegistered(DateTime moment)
        {
            if (SwitchState(expectedState: State.Finished, nextState: State.LastMomentRegistered))
            {
                LastMomentRegisteredAt = moment;

                return true;
            }

            return false;
        }

        public enum State
        {
            Started,
            LockAcquired,
            LockRejected,
            PreconditionPassed,
            PreconditionRejected,
            Finished,
            LastMomentRegistered,
        }

        private bool SwitchState(State expectedState, State nextState)
        {
            if (CurrentState < expectedState)
            {
                // Throws to retry and wait until aggregate will be in the required state
                throw new InvalidAggregateStateException(CurrentState, expectedState, nextState);
            }

            if (CurrentState > expectedState)
            {
                // Aggregate already in the next state, so this event can be just ignored
                return false;
            }

            CurrentState = nextState;

            return true;
        }
    }
}
