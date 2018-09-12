using System;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainHeartbeat.Core.Domain.LastCashoutEventMoment
{
    public interface ILastCashoutEventMomentRepository
    {
        Task SetLastCashoutEventMomentAsync(string blockchainType, string assetId, DateTime eventMoment);
        Task<DateTime?> GetLastEventMomentAsync(string blockchainType, string assetId);
    }
}
