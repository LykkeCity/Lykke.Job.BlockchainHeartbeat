using System;
using System.Collections.Generic;
using Autofac;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Job.BlockchainHeartbeat.Settings.JobSettings;
using Lykke.Job.BlockchainHeartbeat.Workflow;
using Lykke.Job.BlockchainHeartbeat.Workflow.Sagas;
using Lykke.Messaging;
using Lykke.Messaging.Contract;
using Lykke.Messaging.RabbitMq;

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

            builder.Register(c => new RetryDelayProvider(
                    _settings.SourceAddressLockingRetryDelay,
                    _settings.WaitForTransactionRetryDelay,
                    _settings.NotEnoughBalanceRetryDelay))
                .AsSelf();

            // Sagas
            builder.RegisterType<CashoutFinishRegistrationSaga>();
            builder.RegisterType<HeartBeatCashoutSaga>();

            // Command handlers

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
            var messageEngine = ctx.Resolve<IMessagingEngine>();

           throw new NotImplementedException();
        }
    }
}
