using System;

namespace Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutRegistration
{
    public class CashoutFinishRegistrationAggregate
    {
        public string Version { get; }
        public DateTime StartMoment { get; }

        public DateTime CashoutFinishedAt { get; }
        public Guid OperationId { get; }
        public string AssetId { get; }

        public State CurrentState { get; private set; }

        private CashoutFinishRegistrationAggregate(string version,
            DateTime startMoment,
            Guid operationId,
            string assetId,
            State currentState,
            DateTime cashoutFinishedAt)
        {
            Version = version;
            StartMoment = startMoment;
            OperationId = operationId;
            AssetId = assetId;
            CurrentState = currentState;
            CashoutFinishedAt = cashoutFinishedAt;
        }

        public static CashoutFinishRegistrationAggregate StartNew(
            Guid operationId,
            string assetId,
            DateTime cashoutFinishedAt)
        {
            return new CashoutFinishRegistrationAggregate(version:null,
                startMoment: DateTime.UtcNow, 
                operationId: operationId,
                assetId: assetId,
                currentState: State.Started,
                cashoutFinishedAt:cashoutFinishedAt);
        }

        public static CashoutFinishRegistrationAggregate Restore(
            string version,
            DateTime startMoment,
            Guid operationId,
            string assetId,
            State currentState,
            DateTime cashoutCompletedAt)
        {
            return new CashoutFinishRegistrationAggregate(version: version,
                startMoment: startMoment,
                operationId: operationId,
                assetId: assetId,
                currentState: currentState,
                cashoutFinishedAt:cashoutCompletedAt);
        }

        public bool OnRegistrationStarted()
        {
            if (CurrentState == State.Started)
            {
                return true;
            }

            return false;
        }


        public bool OnFinishRegistered()
        {
            if (CurrentState == State.Started)
            {
                CurrentState = State.RegistrationCompleted;

                return true;
            }

            return false;
        }

        public enum State
        {
            Started,
            RegistrationCompleted
        }
    }
}
