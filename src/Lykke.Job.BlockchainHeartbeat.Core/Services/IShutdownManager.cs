using System.Threading.Tasks;

namespace Lykke.Job.BlockchainHeartbeat.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}