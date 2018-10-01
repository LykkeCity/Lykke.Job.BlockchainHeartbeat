using System.Collections.Generic;
using Autofac;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Job.BlockchainCashoutProcessor.Contract;
using Lykke.Job.BlockchainHeartbeat.Settings.JobSettings;
using Lykke.Job.BlockchainHeartbeat.Workflow.CommandHandlers.CashoutRegistration;
using Lykke.Job.BlockchainHeartbeat.Workflow.CommandHandlers.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.CashoutRegistration;
using Lykke.Job.BlockchainHeartbeat.Workflow.Commands.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.Events.HeartbeatCashout;
using Lykke.Job.BlockchainHeartbeat.Workflow.Sagas;
using Lykke.Messaging;
using Lykke.Messaging.Contract;
using Lykke.Messaging.RabbitMq;
using Lykke.Messaging.Serialization;
using Lykke.Service.Operations.Contracts;

namespace Lykke.Job.BlockchainHeartbeat.Modules
{
    public class CqrsModule : Module
    {
        private readonly CqrsSettings _settings;
        private readonly string _rabbitMqVirtualHost;

        public CqrsModule(CqrsSettings settings, string rabbitMqVirtualHost = null)
        {
            _settings = settings;
            _rabbitMqVirtualHost = rabbitMqVirtualHost;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => new AutofacDependencyResolver(context)).As<IDependencyResolver>().SingleInstance();

            var rabbitMqSettings = new RabbitMQ.Client.ConnectionFactory
            {
                Uri = _settings.RabbitConnectionString
            };

            var rabbitMqEndpoint = _rabbitMqVirtualHost == null
                ? rabbitMqSettings.Endpoint.ToString()
                : $"{rabbitMqSettings.Endpoint}/{_rabbitMqVirtualHost}";


            builder.Register(c => new MessagingEngine(c.Resolve<ILogFactory>(),
                    new TransportResolver(new Dictionary<string, TransportInfo>
                    {
                        {
                            "RabbitMq",
                            new TransportInfo(rabbitMqEndpoint, rabbitMqSettings.UserName,
                                rabbitMqSettings.Password, "None", "RabbitMq")
                        }
                    }),
                    new RabbitMqTransportFactory(c.Resolve<ILogFactory>())))
                .As<IMessagingEngine>()
                .SingleInstance()
                .AutoActivate();

            // Sagas
            builder.RegisterType<CashoutRegistrationSaga>();
            builder.RegisterType<HeartBeatCashoutSaga>();

            // Command handlers

            builder.RegisterType<RegisterCashoutLastMomentCommandHandler>();
            builder.RegisterType<AcquireCashoutLockCommandHandler>();
            builder.RegisterType<ReleaseCashoutLockCommandHandler>();
            builder.RegisterType<StartHeartbeatCashoutCommandHandler>();
            builder.RegisterType<StartCryptoCashoutCommandHandler>();

            builder.Register(CreateEngine)
                .As<ICqrsEngine>()
                .SingleInstance()
                .AutoActivate();
        }

        private CqrsEngine CreateEngine(IComponentContext ctx)
        {
            var defaultRetryDelay = (long)_settings.RetryDelay.TotalMilliseconds;

            const string commandsPipeline = "commands";
            const string defaultRoute = "self";
            const string eventsRoute = "events";
            var messageEngine = ctx.Resolve<IMessagingEngine>();
            var logFactory = ctx.Resolve<ILogFactory>();
            var dependencyResolver = ctx.Resolve<IDependencyResolver>();

            return new CqrsEngine(logFactory,
                dependencyResolver,
                messageEngine,
                new DefaultEndpointProvider(),
                true,
                Register.DefaultEndpointResolver(new RabbitMqConventionEndpointResolver(
                    "RabbitMq",
                    SerializationFormat.MessagePack,
                    environment: "lykke")),
                Register.BoundedContext(CashoutRegistrationSaga.BoundedContext)
                    .FailedCommandRetryDelay(defaultRetryDelay)

                    .ListeningCommands(typeof(RegisterCashoutLastMomentCommand))
                    .On(defaultRoute)
                    .WithCommandsHandler<RegisterCashoutLastMomentCommandHandler>()

                    .ProcessingOptions(defaultRoute).MultiThreaded(4).QueueCapacity(1024)
                    .ProcessingOptions(eventsRoute).MultiThreaded(4).QueueCapacity(1024),

                Register.Saga<CashoutRegistrationSaga>($"{CashoutRegistrationSaga.BoundedContext}.saga")
                    .ListeningEvents(
                        typeof(BlockchainCashoutProcessor.Contract.Events.CashoutStartedEvent),
                        typeof(BlockchainCashoutProcessor.Contract.Events.CashoutFailedEvent),
                        typeof(BlockchainCashoutProcessor.Contract.Events.CashoutCompletedEvent))
                    .From(BlockchainCashoutProcessorBoundedContext.Name)
                    .On(defaultRoute)
                    .PublishingCommands(typeof(RegisterCashoutLastMomentCommand))
                    .To(CashoutRegistrationSaga.BoundedContext)
                    .With(commandsPipeline),

                Register.BoundedContext(HeartBeatCashoutSaga.BoundedContext)
                    .FailedCommandRetryDelay(defaultRetryDelay)

                    .ListeningCommands(typeof(StartHeartbeatCashoutCommand))
                    .On(defaultRoute)
                    .WithLoopback()
                    .WithCommandsHandler<StartHeartbeatCashoutCommandHandler>()
                    .PublishingEvents(typeof(HeartbeatCashoutStartedEvent))
                    .With(eventsRoute)

                    .ListeningCommands(typeof(AcquireCashoutLockCommand))
                    .On(defaultRoute)
                    .WithCommandsHandler<AcquireCashoutLockCommandHandler>()
                    .PublishingEvents(typeof(CashoutLockAcquiredEvent))
                    .With(eventsRoute)
                    
                    .ListeningCommands(typeof(RegisterCashoutLastMomentCommand))
                    .On(defaultRoute)
                    .WithCommandsHandler<RegisterCashoutLastMomentCommandHandler>()

                    .ListeningCommands(typeof(StartCryptoCashoutCommand))
                    .On(defaultRoute)
                    .WithCommandsHandler<StartCryptoCashoutCommandHandler>()

                    .ListeningCommands(typeof(ReleaseCashoutLockCommand))
                    .On(defaultRoute)
                    .WithCommandsHandler<ReleaseCashoutLockCommandHandler>()

                    .ProcessingOptions(defaultRoute).MultiThreaded(4).QueueCapacity(1024)
                    .ProcessingOptions(eventsRoute).MultiThreaded(4).QueueCapacity(1024),

                Register.Saga<HeartBeatCashoutSaga>($"{HeartBeatCashoutSaga.BoundedContext}.saga")
                    .ListeningEvents(typeof(HeartbeatCashoutStartedEvent))
                    .From(HeartBeatCashoutSaga.BoundedContext)
                    .On(defaultRoute)
                    .PublishingCommands(typeof(AcquireCashoutLockCommand))
                    .To(HeartBeatCashoutSaga.BoundedContext)
                    .With(commandsPipeline)

                    .ListeningEvents(typeof(CashoutLockAcquiredEvent))
                    .From(HeartBeatCashoutSaga.BoundedContext)
                    .On(defaultRoute)
                    .PublishingCommands(typeof(StartCryptoCashoutCommand), typeof(RegisterCashoutLastMomentCommand))
                    .To(HeartBeatCashoutSaga.BoundedContext)
                    .With(commandsPipeline)

                    .ListeningEvents(
                        typeof(Service.Operations.Contracts.Events.OperationCompletedEvent),
                        typeof(Service.Operations.Contracts.Events.OperationCorruptedEvent),
                        typeof(Service.Operations.Contracts.Events.OperationFailedEvent))
                    .From(OperationsBoundedContext.Name)
                    .On(defaultRoute)
                    .PublishingCommands(typeof(ReleaseCashoutLockCommand))
                    .To(HeartBeatCashoutSaga.BoundedContext)
                    .With(commandsPipeline)
                );
        }
    }
}
