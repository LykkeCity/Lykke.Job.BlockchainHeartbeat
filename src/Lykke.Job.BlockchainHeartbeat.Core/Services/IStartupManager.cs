using System.Threading.Tasks;

namespace Lykke.Job.BlockchainHeartbeat.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}