namespace Lykke.Job.BlockchainHeartbeat.Services.WalletApiV2.Contracts.Operations
{
    public class CreateCashoutRequest
    {
        public string AssetId { get; set; }
        public decimal Volume { get; set; }
        public string DestinationAddress { get; set; }
        public string DestinationAddressExtension { get; set; }
    }
}
