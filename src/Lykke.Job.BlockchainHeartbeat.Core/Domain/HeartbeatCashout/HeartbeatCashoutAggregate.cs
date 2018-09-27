using System;

namespace Lykke.Job.BlockchainHeartbeat.Core.Domain.HeartbeatCashout
{
    public class HeartbeatCashoutAggregate
    {
        public string Version { get; }
        public DateTime StartMoment { get; }
        public DateTime? LockAcquiredAt { get; private set; }
        public DateTime? LockReleasedAt { get; private set; }
        public DateTime? OperationFinishMoment { get; private set; }
        public Guid OperationId { get; }
        public string ToAddress { get; }
        public string ToAddressExtension { get; }
        public decimal Amount { get; }
        public string AssetId { get; }

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
            DateTime? lockReleasedAt)
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
            LockReleasedAt = lockReleasedAt;
            ToAddressExtension = toAddressExtension;
        }

        public static HeartbeatCashoutAggregate StartNew(
            Guid operationId,
            string toAddress,
            string toAddressExtension,
            decimal amount,
            string assetId)
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
                lockReleasedAt: null);
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
            DateTime? lockReleasedAt)
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
                lockReleasedAt:lockReleasedAt);
        }
        public bool OnStarted()
        {
            if (CurrentState == State.Started)
            {
                return true;
            }

            return false;
        }
        
        public bool OnLockAcquired()
        {
            if (CurrentState == State.Started)
            {
                LockAcquiredAt = DateTime.UtcNow;
                CurrentState = State.LockAcquired;

                return true;
            }

            return false;
        }

        public bool OnFinished(DateTime finishMoment)
        {
            if (CurrentState == State.LockAcquired)
            {
                CurrentState = State.Finished;
                OperationFinishMoment = finishMoment;
                return true;
            }

            return false;
        }

        public bool OnLockReleased()
        {
            if (CurrentState == State.Finished)
            {
                CurrentState = State.LockReleased;
                LockReleasedAt = DateTime.UtcNow;

                return true;
            }

            return false;
        }

        public enum State
        {
            Started,
            LockAcquired,
            Finished,
            LockReleased
        }
    }
}
