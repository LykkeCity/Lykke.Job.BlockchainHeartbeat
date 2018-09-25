using System;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainHeartbeat.Core.Services
{
    public interface IWalletApiV2Provider
    {
        Task<(string accessToken, string notificationId)> AuthAsync(string email, string password, string parthnerId, string clientInfo);

        Task CreateCryptoCashoutAsync(Guid operationId, 
            string accessToken,
            string assetId,
            decimal amount, 
            string destinationAddress,
            string destinationAddressExtension);
    }
}
