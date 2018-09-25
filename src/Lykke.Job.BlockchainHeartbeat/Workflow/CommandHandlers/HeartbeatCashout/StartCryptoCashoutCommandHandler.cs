using System.Threading;
using System.Threading.Tasks;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Services;
using Lykke.Job.BlockchainHeartbeat.Workflow.CommandHandlers.HeartbeatCashout.Settings;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.CommandHandlers.HeartbeatCashout
{
    public class StartCryptoCashoutCommandHandler
    {
        private readonly CryptoCashoutUserSettings _cashoutUserSettings;
        private readonly IChaosKitty _chaosKitty;
        private readonly IWalletApiV2Provider _walletApiProvider;
        private readonly SemaphoreSlim _semaphoreSlim;

        public StartCryptoCashoutCommandHandler(CryptoCashoutUserSettings cashoutUserSettings, 
            IChaosKitty chaosKitty,
            IWalletApiV2Provider walletApiProvider)
        {
            _cashoutUserSettings = cashoutUserSettings;
            _chaosKitty = chaosKitty;
            _walletApiProvider = walletApiProvider;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async Task<CommandHandlingResult> Handle(StartCryptoCashoutCommand command,
            IEventPublisher publisher)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                var authResult = await _walletApiProvider.AuthAsync(_cashoutUserSettings.Email,
                    _cashoutUserSettings.Password,
                    _cashoutUserSettings.ParthnerId,
                    _cashoutUserSettings.ClientInfo);

                _chaosKitty.Meow(command.OperationId);

                await _walletApiProvider.CreateCryptoCashoutAsync(command.OperationId,
                    authResult.accessToken,
                    command.AssetId,
                    command.Amount,
                    command.ToAddress,
                    command.ToAddressExtension);
            }
            finally
            {
                _semaphoreSlim.Release();
            }


            return CommandHandlingResult.Ok();
        }
    }
}
