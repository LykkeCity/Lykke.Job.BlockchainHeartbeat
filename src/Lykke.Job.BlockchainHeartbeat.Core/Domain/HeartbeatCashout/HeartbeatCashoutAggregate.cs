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
        public DateTime? PreconditionPassedAt { get; private set; }
        public DateTime? PreconditionRejectedAt { get; private set; }
        public DateTime? AssetInfoRetrievedAt { get; private set; }
        public DateTime? OperationFinishMoment { get; private set; }
        public Guid OperationId { get; }
        public string ToAddress { get; }
        public string ToAddressExtension { get; }
        public decimal Amount { get; }
        public string AssetId { get; }
        public string AssetDisplayId { get; private set; }
        public int? AssetMultiplierPower { get; private set; }
        public string AssetAddress { get; private set; }
        public int? AssetAccuracy { get; private set; }
        public string AssetBlockchain { get; private set; }
        public string AssetType { get; private set; }
        public bool? AssetIsTradable { get; private set; }
        public bool? AssetIsTrusted { get; private set; }
        public bool? AssetKycNeeded { get; private set; }
        public string AssetBlockchainIntegrationLayerId { get; private set; }
        public decimal? AssetCashoutMinimalAmount { get; private set; }
        public decimal? AssetLowVolumeAmount { get; private set; }
        public bool? AssetBlockchainWithdrawal { get; private set; }
        public string AssetLykkeEntityId { get; private set; }
        public TimeSpan MaxCashoutInactivePeriod { get; }
        public State CurrentState { get; private set; }
        public Guid ClientId { get; }
        public string FeeCashoutTargetClientId { get; }
        public decimal ClientBalance { get; }

        public HeartbeatCashoutAggregate(string version,
            DateTime startMoment,
            DateTime? lockAcquiredAt,
            DateTime? lockRejectedAt,
            DateTime? lastMomentRegisteredAt,
            DateTime? preconditionPassedAt,
            DateTime? preconditionRejectedAt,
            DateTime? assetInfoRetrievedAt,
            DateTime? operationFinishMoment,
            Guid operationId,
            string toAddress,
            string toAddressExtension,
            decimal amount,
            string assetId,
            string assetDisplayId,
            int? assetMultiplierPower,
            string assetAddress,
            int? assetAccuracy,
            string assetBlockchain,
            string assetType,
            bool? assetIsTradable,
            bool? assetIsTrusted,
            bool? assetKycNeeded,
            string assetBlockchainIntegrationLayerId,
            decimal? assetCashoutMinimalAmount,
            decimal? assetLowVolumeAmount,
            bool? assetBlockchainWithdrawal,
            string assetLykkeEntityId,
            TimeSpan maxCashoutInactivePeriod,
            State currentState,
            Guid clientId,
            string feeCashoutTargetClientId,
            decimal clientBalance)
        {
            Version = version;
            StartMoment = startMoment;
            LockAcquiredAt = lockAcquiredAt;
            LockRejectedAt = lockRejectedAt;
            LastMomentRegisteredAt = lastMomentRegisteredAt;
            PreconditionPassedAt = preconditionPassedAt;
            PreconditionRejectedAt = preconditionRejectedAt;
            AssetInfoRetrievedAt = assetInfoRetrievedAt;
            OperationFinishMoment = operationFinishMoment;
            OperationId = operationId;
            ToAddress = toAddress;
            ToAddressExtension = toAddressExtension;
            Amount = amount;
            AssetId = assetId;
            AssetDisplayId = assetDisplayId;
            AssetMultiplierPower = assetMultiplierPower;
            AssetAddress = assetAddress;
            AssetAccuracy = assetAccuracy;
            AssetBlockchain = assetBlockchain;
            AssetType = assetType;
            AssetIsTradable = assetIsTradable;
            AssetIsTrusted = assetIsTrusted;
            AssetKycNeeded = assetKycNeeded;
            AssetBlockchainIntegrationLayerId = assetBlockchainIntegrationLayerId;
            AssetCashoutMinimalAmount = assetCashoutMinimalAmount;
            AssetLowVolumeAmount = assetLowVolumeAmount;
            AssetBlockchainWithdrawal = assetBlockchainWithdrawal;
            AssetLykkeEntityId = assetLykkeEntityId;
            MaxCashoutInactivePeriod = maxCashoutInactivePeriod;
            CurrentState = currentState;
            ClientId = clientId;
            FeeCashoutTargetClientId = feeCashoutTargetClientId;
            ClientBalance = clientBalance;
        }

        public static HeartbeatCashoutAggregate Restore(string version,
            DateTime startMoment,
            DateTime? lockAcquiredAt,
            DateTime? lockRejectedAt,
            DateTime? lastMomentRegisteredAt,
            DateTime? preconditionPassedAt,
            DateTime? preconditionRejectedAt,
            DateTime? assetInfoRetrievedAt,
            DateTime? operationFinishMoment,
            Guid operationId,
            string toAddress,
            string toAddressExtension,
            decimal amount,
            string assetId,
            string assetDisplayId,
            int? assetMultiplierPower,
            string assetAddress,
            int? assetAccuracy,
            string assetBlockchain,
            string assetType,
            bool? assetIsTradable,
            bool? assetIsTrusted,
            bool? assetKycNeeded,
            string assetBlockchainIntegrationLayerId,
            decimal? assetCashoutMinimalAmount,
            decimal? assetLowVolumeAmount,
            bool? assetBlockchainWithdrawal,
            string assetLykkeEntityId,
            TimeSpan maxCashoutInactivePeriod,
            State currentState,
            Guid clientId,
            string feeCashoutTargetClientId,
            decimal clientBalance)
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
                maxCashoutInactivePeriod: maxCashoutInactivePeriod,
                preconditionPassedAt: preconditionPassedAt,
                preconditionRejectedAt: preconditionRejectedAt,
                assetInfoRetrievedAt: assetInfoRetrievedAt,
                assetAccuracy: assetAccuracy,
                assetAddress: assetAddress,
                assetBlockchain: assetBlockchain,
                assetBlockchainIntegrationLayerId: assetBlockchainIntegrationLayerId,
                assetMultiplierPower: assetMultiplierPower,
                assetType: assetType,
                assetBlockchainWithdrawal: assetBlockchainWithdrawal,
                assetCashoutMinimalAmount: assetCashoutMinimalAmount,
                assetDisplayId: assetDisplayId,
                assetIsTradable: assetIsTradable,
                assetIsTrusted: assetIsTrusted,
                assetKycNeeded: assetKycNeeded,
                assetLowVolumeAmount: assetLowVolumeAmount,
                assetLykkeEntityId: assetLykkeEntityId,
                clientId: clientId,
                feeCashoutTargetClientId: feeCashoutTargetClientId,
                clientBalance: clientBalance);
        }

        public static HeartbeatCashoutAggregate StartNew(Guid operationId,
            string toAddress,
            string toAddressExtension,
            decimal amount,
            string assetId,
            TimeSpan maxCashoutInactivePeriod,
            Guid clientId,
            string feeCashoutTargetClientId,
            decimal clientBalance)
        {
            return new HeartbeatCashoutAggregate(version: null,
                startMoment: DateTime.UtcNow,
                operationFinishMoment: null,
                operationId: operationId,
                amount: amount, assetId: assetId,
                toAddress: toAddress,
                toAddressExtension: toAddressExtension,
                currentState: State.Started,
                lockAcquiredAt: null,
                lockRejectedAt: null,
                lastMomentRegisteredAt: null,
                maxCashoutInactivePeriod: maxCashoutInactivePeriod,
                preconditionPassedAt: null,
                preconditionRejectedAt: null,
                assetInfoRetrievedAt: null,
                assetAccuracy: null,
                assetAddress: null,
                assetBlockchain: null,
                assetBlockchainIntegrationLayerId: null,
                assetMultiplierPower: null,
                assetType: null,
                assetBlockchainWithdrawal: null,
                assetCashoutMinimalAmount: null,
                assetDisplayId: null,
                assetIsTradable: null,
                assetIsTrusted: null,
                assetKycNeeded: null,
                assetLowVolumeAmount: null,
                assetLykkeEntityId: null,
                clientId: clientId,
                feeCashoutTargetClientId: feeCashoutTargetClientId,
                clientBalance: clientBalance);
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
                PreconditionPassedAt = moment;

                return true;
            }

            return false;
        }

        public bool OnPreconditionRejected(DateTime moment)
        {
            if (SwitchState(expectedState: State.LockAcquired, nextState: State.PreconditionRejected))
            {
                PreconditionRejectedAt = moment;

                return true;
            }

            return false;
        }

        public bool OnAssetInfoRetrieved(DateTime moment,
            string assetDisplayId,
            int? assetMultiplierPower,
            string assetAddress,
            int? assetAccuracy,
            string assetBlockchain,
            string assetType,
            bool? assetIsTradable,
            bool? assetIsTrusted,
            bool? assetKycNeeded,
            string assetBlockchainIntegrationLayerId,
            decimal? assetCashoutMinimalAmount,
            decimal? assetLowVolumeAmount,
            bool? assetBlockchainWithdrawal,
            string assetLykkeEntityId)
        {
            if (SwitchState(expectedState: State.PreconditionPassed, nextState: State.AssetInfoRetrieved))
            {
                AssetInfoRetrievedAt = moment;

                AssetDisplayId = assetDisplayId;
                AssetMultiplierPower = assetMultiplierPower;
                AssetAddress = assetAddress;
                AssetAccuracy = assetAccuracy;
                AssetBlockchain = assetBlockchain;
                AssetType = assetType;
                AssetIsTradable = assetIsTradable;
                AssetIsTrusted = assetIsTrusted;
                AssetKycNeeded = assetKycNeeded;
                AssetBlockchainIntegrationLayerId = assetBlockchainIntegrationLayerId;
                AssetCashoutMinimalAmount = assetCashoutMinimalAmount;
                AssetLowVolumeAmount = assetLowVolumeAmount;
                AssetBlockchainWithdrawal = assetBlockchainWithdrawal;
                AssetLykkeEntityId = assetLykkeEntityId;

                return true;
            }

            return false;
        }

        public bool OnFinished(DateTime moment)
        {
            if (SwitchState(expectedState: State.AssetInfoRetrieved, nextState: State.Finished))
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
            AssetInfoRetrieved,
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
