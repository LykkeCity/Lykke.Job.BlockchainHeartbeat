using System;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainHeartbeat.Core.Domain.CashoutLock
{
    public interface ICashoutLockRepository
    {
        Task<bool> TryGetLockAsync(string blockchainType, string assetId, Guid operationId);
        Task<bool> ReleaseLockAsync(string blockchainType, string assetId, Guid operationId);
    }
}
