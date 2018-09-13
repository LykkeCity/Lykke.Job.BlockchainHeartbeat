using System;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutLock
{
    public interface ICashoutLockRepository
    {
        Task<bool> TryGetLockAsync(string assetId, Guid operationId);
        Task<bool> ReleaseLockAsync(string assetId, Guid operationId);
        Task<bool> IsLockedAsync(string assetId);
    }
}
