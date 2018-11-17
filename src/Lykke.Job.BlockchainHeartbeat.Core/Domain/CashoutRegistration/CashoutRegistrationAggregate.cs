using System;

namespace Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutRegistration
{
    public class CashoutRegistrationAggregate
    {
        public string Version { get; }
        public DateTime? StartMoment { get; private set; }

        public DateTime? FinishMoment { get; private set; }
        public Guid OperationId { get; }
        public string AssetId { get; }

        private CashoutRegistrationAggregate(string version,
            DateTime? startMoment,
            Guid operationId,
            string assetId,
            DateTime? finishMoment)
        {
            Version = version;
            StartMoment = startMoment;
            OperationId = operationId;
            AssetId = assetId;
            FinishMoment = finishMoment;
        }

        public static CashoutRegistrationAggregate StartNew(
            Guid operationId,
            string assetId)
        {
            return new CashoutRegistrationAggregate(version:null,
                startMoment: null, 
                operationId: operationId,
                assetId: assetId,
                finishMoment: null);
        }

        public static CashoutRegistrationAggregate Restore(
            string version,
            DateTime? startMoment,
            Guid operationId,
            string assetId,
            DateTime? finishMoment)
        {
            return new CashoutRegistrationAggregate(version: version,
                startMoment: startMoment,
                operationId: operationId,
                assetId: assetId,
                finishMoment: finishMoment);
        }

        public void OnStarted(DateTime moment)
        {
            StartMoment = moment;
        }

        public void OnFinished(DateTime moment)
        {
            FinishMoment = moment;
        }
    }
}
