using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Lykke.Job.BlockchainHeartbeat.Services.WalletApiV2.Contracts.Client
{
    internal class AuthRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ClientInfo { get; set; }
        public string PartnerId { get; set; }
    }
}
