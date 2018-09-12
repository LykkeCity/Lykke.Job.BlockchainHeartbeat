using System;

namespace Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutRegistration
{
    public class CashoutRegistrationAggregate
    {
        public string Version { get; }
        public DateTime StartMoment { get; }
        public DateTime? OperationFinishMoment { get; private set; }
        public Guid OperationId { get; }
        public Guid ClientId { get; }
        public string BlockchainType { get; }
        public string BlockchainAssetId { get; }
        public string HotWalletAddress { get; }
        public string ToAddress { get; }
        public decimal Amount { get; }
        public string AssetId { get; }

        public State CurrentState { get; private set; }

        private CashoutRegistrationAggregate(string version,
            DateTime startMoment,
            DateTime? operationFinishMoment,
            Guid operationId,
            Guid clientId,
            string blockchainType,
            string blockchainAssetId,
            string hotWalletAddress,
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
            BlockchainType = blockchainType;
            BlockchainAssetId = blockchainAssetId;
            HotWalletAddress = hotWalletAddress;
            ToAddress = toAddress;
            Amount = amount;
            AssetId = assetId;
            CurrentState = currentState;
        }

        public static CashoutRegistrationAggregate StartNew(
            Guid operationId,
            Guid clientId,
            string blockchainType,
            string blockchainAssetId,
            string hotWalletAddress,
            string toAddress,
            decimal amount,
            string assetId)
        {
            return new CashoutRegistrationAggregate(version:null,
                startMoment: DateTime.UtcNow, 
                operationFinishMoment: null, 
                operationId: operationId,
                clientId: clientId, 
                blockchainType: blockchainType, 
                blockchainAssetId: blockchainAssetId,
                amount:amount, assetId:assetId,
                hotWalletAddress: hotWalletAddress,
                toAddress:toAddress,
                currentState: State.Started);
        }

        public static CashoutRegistrationAggregate Restore(
            string version,
            DateTime startMoment,
            DateTime? operationFinishMoment,
            Guid operationId,
            Guid clientId,
            string blockchainType,
            string blockchainAssetId,
            string hotWalletAddress,
            string toAddress,
            decimal amount,
            string assetId,
            State currentState)
        {
            return new CashoutRegistrationAggregate(version: version,
                startMoment: startMoment,
                operationFinishMoment: operationFinishMoment,
                operationId: operationId,
                clientId: clientId,
                blockchainType: blockchainType,
                blockchainAssetId: blockchainAssetId,
                amount: amount, assetId: assetId,
                hotWalletAddress: hotWalletAddress,
                toAddress: toAddress,
                currentState: currentState);
        }

        public enum State
        {
            Started
        }

    }
}
