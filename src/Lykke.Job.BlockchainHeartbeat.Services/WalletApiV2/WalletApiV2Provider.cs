using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Job.BlockchainHeartbeat.Core.Services;
using Lykke.Job.BlockchainHeartbeat.Services.WalletApiV2.Contracts.Client;
using Lykke.Job.BlockchainHeartbeat.Services.WalletApiV2.Contracts.Operations;

namespace Lykke.Job.BlockchainHeartbeat.Services
{
    public class WalletApiV2Provider: IWalletApiV2Provider
    {
        private readonly string _baseUrl;

        public WalletApiV2Provider(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<(string accessToken, string notificationId)> AuthAsync(string email, string password, string parthnerId, string clientInfo)
        {
            var authResult = await _baseUrl.AppendPathSegment("/api/client/auth")
                .PostJsonAsync(new AuthRequestModel
                {
                    ClientInfo = clientInfo,
                    Email = email,
                    PartnerId = parthnerId,
                    Password = password
                }).ReceiveJson<AuthResponseModel>();

            return (authResult.AccessToken, authResult.NotificationsId);
        }

        public async Task CreateCryptoCashoutAsync(Guid operationId,
            string accessToken,
            string assetId, 
            decimal amount, 
            string destinationAddress,
            string destinationAddressExtension)
        {
            await _baseUrl.AppendPathSegment("/api/operations/cashout/crypto")
                .SetQueryParam("id", operationId)
                .WithHeader("Authorization", $"Bearer: {accessToken}")
                .PostJsonAsync(new CreateCashoutRequest
                {
                    AssetId = assetId,
                    DestinationAddress = destinationAddress,
                    DestinationAddressExtension = destinationAddressExtension,
                    Volume = amount
                });
        }
    }
}
