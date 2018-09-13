using System;

namespace Lykke.Job.BlockchainHeartbeat.Core.Domain.HeartbeatCashout
{
    public class HeartbeatCashoutAggregate
    {
        public string Version { get; }
        public DateTime StartMoment { get; }
        public DateTime? OperationFinishMoment { get; private set; }
        public Guid OperationId { get; }
        public Guid ClientId { get; }
        public string ToAddress { get; }
        public decimal Amount { get; }
        public string AssetId { get; }

        public State CurrentState { get; private set; }

        private HeartbeatCashoutAggregate(string version,
            DateTime startMoment,
            DateTime? operationFinishMoment,
            Guid operationId,
            Guid clientId,
            string toAddress,
            decimal amount,
            string assetId,
            State currentState)
        {
            Version = version;
            StartMoment = startMoment;
            OperationFinishMoment = operationFinishMoment;
            OperationId = operationId;
            ClientId = clientId;
            ToAddress = toAddress;
            Amount = amount;
            AssetId = assetId;
            CurrentState = currentState;
        }

        public static HeartbeatCashoutAggregate StartNew(
            Guid operationId,
            Guid clientId,
            string toAddress,
            decimal amount,
            string assetId)
        {
            return new HeartbeatCashoutAggregate(version: null,
                startMoment: DateTime.UtcNow,
                operationFinishMoment: null,
                operationId: operationId,
                clientId: clientId,
                amount: amount, assetId: assetId,
                toAddress: toAddress,
                currentState: State.Started);
        }

        public static HeartbeatCashoutAggregate Restore(
            string version,
            DateTime startMoment,
            DateTime? operationFinishMoment,
            Guid operationId,
            Guid clientId,
            string toAddress,
            decimal amount,
            string assetId,
            State currentState)
        {
            return new HeartbeatCashoutAggregate(version: version,
                startMoment: startMoment,
                operationFinishMoment: operationFinishMoment,
                operationId: operationId,
                clientId: clientId,
                amount: amount, assetId: assetId,
                toAddress: toAddress,
                currentState: currentState);
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
                CurrentState = State.LockAcquired;

                return true;
            }

            return false;
        }

        public bool OnFinished(DateTime finisDateTime)
        {
            if (CurrentState == State.LockAcquired)
            {
                CurrentState = State.Finished;
                OperationFinishMoment = finisDateTime;
                return true;
            }

            return false;
        }

        public bool OnLockReleased()
        {
            if (CurrentState == State.Finished)
            {
                CurrentState = State.LockReleased;

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
