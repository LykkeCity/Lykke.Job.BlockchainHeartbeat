using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.BlockchainHeartbeat.Services.WalletApiV2.Contracts.Client
{
    internal class AuthResponseModel
    {
        public string AccessToken { get; set; }
        public string NotificationsId { get; set; }
    }
}
