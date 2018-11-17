using System;
using System.Threading.Tasks;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.LastCashoutEventMoment;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.CashoutRegistration;

namespace Lykke.Job.BlockchainHeartbeat.Workflow.CommandHandlers.CashoutRegistration
{
    public class RegisterCashoutRegistrationLastMomentCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly ILastCashoutEventMomentRepository _lastMomentRepository;

        public RegisterCashoutRegistrationLastMomentCommandHandler(IChaosKitty chaosKitty, 
            ILastCashoutEventMomentRepository lastMomentRepository)
        {
            _chaosKitty = chaosKitty;
            _lastMomentRepository = lastMomentRepository;
        }

        public async Task<CommandHandlingResult> Handle(RegisterCashoutRegistrationLastMomentCommand momentCommand,
            IEventPublisher publisher)
        {
            await _lastMomentRepository.SetLastCashoutEventMomentAsync(momentCommand.AssetId,
                momentCommand.Moment);

            _chaosKitty.Meow(momentCommand.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
