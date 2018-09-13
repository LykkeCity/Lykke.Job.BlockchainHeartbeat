using System;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainHeartbeat.Core.Domain.LastCashoutEventMoment
{
    public interface ILastCashoutEventMomentRepository
    {
        Task<bool> SetLastCashoutEventMomentAsync(string assetId, DateTime eventMoment);
        Task<DateTime?> GetLastEventMomentAsync(string assetId);
    }
}
