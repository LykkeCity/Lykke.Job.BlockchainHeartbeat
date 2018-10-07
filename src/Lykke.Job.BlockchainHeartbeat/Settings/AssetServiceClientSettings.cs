using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainHeartbeat.Settings
{
    public class AssetServiceClientSettings
    {
        [HttpCheck("api/isalive")]
        public string ServiceUrl { get; set; }
    }
}
