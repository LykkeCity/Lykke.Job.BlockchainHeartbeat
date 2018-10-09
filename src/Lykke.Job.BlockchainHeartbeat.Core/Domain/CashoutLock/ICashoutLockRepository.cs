using System;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutLock
{
    public interface ICashoutLockRepository
    {
        Task<bool> TryLockAsync(string assetId, Guid operationId, DateTime lockedAt);
        Task<bool> ReleaseLockAsync(string assetId, Guid operationId);
        Task<bool> IsLockedAsync(string assetId);
        Task<(DateTime lockedAt, Guid operationId)?> GetLockAsync(string assetId);
    }
}
