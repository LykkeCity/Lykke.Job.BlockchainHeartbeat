using System;
using Common;
using Lykke.AzureStorage.Tables;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.HeartbeatCashout;

namespace Lykke.Job.BlockchainHeartbeat.AzureRepositories.HeartbeatCashout
{
    internal class HeartbeatCashoutEntity : AzureTableEntity
    {
        #region Fields

        // ReSharper disable MemberCanBePrivate.Global
        public DateTime StartMoment { get; set; }
        public DateTime? OperationFinishMoment { get; set; }

        public Guid OperationId { get; set; }
        public string ToAddress { get; set; }
        public string ToAddressExtension { get; set; }
        public decimal Amount { get; set; }
        public string AssetId { get; set; }

        public HeartbeatCashoutAggregate.State CurrentState { get; set; }

        public DateTime? LockAcquiredAt { get; set; }
        public DateTime? LockRejectedAt { get; set; }
        public DateTime? LastMomentRegisteredAt { get; set; }


        public DateTime? PreconditionPassedAt { get; set; }
        public DateTime? PreconditionRejectedAt { get; set; }

        public TimeSpan MaxCashoutInactivePeriod { get; set; }
        public DateTime? AssetInfoRetrievedAt { get; set; }



        public string AssetDisplayId { get; set; }

        public int? AssetMultiplierPower { get; set; }

        public string AssetAddress { get; set; }

        public int? AssetAccuracy { get; set; }

        public string AssetBlockchain { get; set; }

        public string AssetType { get; set; }

        public bool? AssetIsTradable { get; set; }

        public bool? AssetIsTrusted { get; set; }

        public bool? AssetKycNeeded { get; set; }

        public string AssetBlockchainIntegrationLayerId { get; set; }

        public Decimal? AssetCashoutMinimalAmount { get; set; }

        public Decimal? AssetLowVolumeAmount { get; set; }

        public bool? AssetBlockchainWithdrawal { get; set; }

        public string AssetLykkeEntityId { get; set; }

        public Guid ClientId { get; set; }

        public string FeeCashoutTargetClientId { get; set; }
        public decimal ClientBalance { get; set; }

        // ReSharper restore MemberCanBePrivate.Global

        #endregion


        #region Keys

        public static string GetPartitionKey(Guid operationId)
        {
            // Use hash to distribute all records to the different partitions
            var hash = operationId.ToString().CalculateHexHash32(3);

            return $"{hash}";
        }

        public static string GetRowKey(Guid operationId)
        {
            return $"{operationId:D}";
        }

        #endregion


        #region Conversion

        public static HeartbeatCashoutEntity FromDomain(HeartbeatCashoutAggregate aggregate)
        {
            return new HeartbeatCashoutEntity
            {
                ETag =  string.IsNullOrEmpty(aggregate.Version) ? "*" : aggregate.Version,
                PartitionKey =  GetPartitionKey(aggregate.OperationId),
                RowKey =  GetRowKey(aggregate.OperationId),
                StartMoment =  aggregate.StartMoment,
                LockAcquiredAt =  aggregate.LockAcquiredAt,
                LockRejectedAt =  aggregate.LockRejectedAt,
                LastMomentRegisteredAt =  aggregate.LastMomentRegisteredAt,
                PreconditionPassedAt =  aggregate.PreconditionPassedAt,
                PreconditionRejectedAt =  aggregate.PreconditionRejectedAt,
                AssetInfoRetrievedAt =  aggregate.AssetInfoRetrievedAt,
                OperationFinishMoment =  aggregate.OperationFinishMoment,
                OperationId =  aggregate.OperationId,
                ToAddress =  aggregate.ToAddress,
                ToAddressExtension =  aggregate.ToAddressExtension,
                Amount =  aggregate.Amount,
                AssetId =  aggregate.AssetId,
                AssetDisplayId =  aggregate.AssetDisplayId,
                AssetMultiplierPower =  aggregate.AssetMultiplierPower,
                AssetAddress =  aggregate.AssetAddress,
                AssetAccuracy =  aggregate.AssetAccuracy,
                AssetBlockchain =  aggregate.AssetBlockchain,
                AssetType =  aggregate.AssetType,
                AssetIsTradable =  aggregate.AssetIsTradable,
                AssetIsTrusted =  aggregate.AssetIsTrusted,
                AssetKycNeeded =  aggregate.AssetKycNeeded,
                AssetBlockchainIntegrationLayerId =  aggregate.AssetBlockchainIntegrationLayerId,
                AssetCashoutMinimalAmount =  aggregate.AssetCashoutMinimalAmount,
                AssetLowVolumeAmount =  aggregate.AssetLowVolumeAmount,
                AssetBlockchainWithdrawal =  aggregate.AssetBlockchainWithdrawal,
                AssetLykkeEntityId =  aggregate.AssetLykkeEntityId,
                MaxCashoutInactivePeriod =  aggregate.MaxCashoutInactivePeriod,
                CurrentState =  aggregate.CurrentState,
                ClientId = aggregate.ClientId,
                ClientBalance = aggregate.ClientBalance,
                FeeCashoutTargetClientId = aggregate.FeeCashoutTargetClientId
            };
        }

        public HeartbeatCashoutAggregate ToDomain()
        {
            return HeartbeatCashoutAggregate.Restore(version: ETag,
                startMoment: DateTime.UtcNow,
                operationFinishMoment: OperationFinishMoment,
                operationId: OperationId,
                amount: Amount, assetId: AssetId,
                toAddress: ToAddress,
                toAddressExtension: ToAddressExtension,
                currentState: HeartbeatCashoutAggregate.State.Started,
                lockAcquiredAt: LockAcquiredAt,
                lockRejectedAt: LockRejectedAt,
                lastMomentRegisteredAt: LastMomentRegisteredAt,
                maxCashoutInactivePeriod: MaxCashoutInactivePeriod,
                preconditionPassedAt: PreconditionPassedAt,
                preconditionRejectedAt: PreconditionRejectedAt,
                assetInfoRetrievedAt: AssetInfoRetrievedAt,
                assetAccuracy: AssetAccuracy,
                assetAddress: AssetAddress,
                assetBlockchain: AssetBlockchain,
                assetBlockchainIntegrationLayerId: AssetBlockchainIntegrationLayerId,
                assetMultiplierPower: AssetMultiplierPower,
                assetType: AssetType,
                assetBlockchainWithdrawal: AssetBlockchainWithdrawal,
                assetCashoutMinimalAmount: AssetCashoutMinimalAmount,
                assetDisplayId: AssetDisplayId,
                assetIsTradable: AssetIsTradable,
                assetIsTrusted: AssetIsTrusted,
                assetKycNeeded: AssetKycNeeded,
                assetLowVolumeAmount: AssetLowVolumeAmount,
                assetLykkeEntityId: AssetLykkeEntityId,
                clientId: ClientId,
                feeCashoutTargetClientId: FeeCashoutTargetClientId,
                clientBalance: ClientBalance);
        }

        #endregion
    }
}
