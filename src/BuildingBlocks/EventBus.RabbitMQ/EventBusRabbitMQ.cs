using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Whatsapp.Flow.BuildingBlocks.EventBus.Abstractions;
using Whatsapp.Flow.BuildingBlocks.EventBus.Events;

namespace Whatsapp.Flow.BuildingBlocks.EventBus.RabbitMQ
{
    // GEÇİCİ - Derleme hatalarını çözmek için basitleştirildi.
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly ILogger<EventBusRabbitMQ> _logger;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
            IServiceProvider serviceProvider, IEventBusSubscriptionsManager subsManager, string queueName, int retryCount = 5)
        {
            _logger = logger;
        }

        public void Publish(IntegrationEvent @event)
        {
            _logger.LogWarning("Publish is not implemented in the temporary version.");
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _logger.LogWarning("Subscribe is not implemented in the temporary version.");
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _logger.LogWarning("Unsubscribe is not implemented in the temporary version.");
        }
        
        public void Dispose()
        {
             // Nothing to dispose
        }
    }
} 