using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Whatsapp.Flow.BuildingBlocks.EventBus.Abstractions;
using Whatsapp.Flow.BuildingBlocks.EventBus.Events;

namespace Whatsapp.Flow.BuildingBlocks.EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly string _queueName;
        private readonly int _retryCount;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
            IServiceProvider serviceProvider, IEventBusSubscriptionsManager subsManager, string queueName, int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _subsManager = subsManager ?? throw new ArgumentNullException(nameof(subsManager));
            _queueName = queueName;
            _retryCount = retryCount;
        }

        public void Publish(IntegrationEvent @event)
        {
            _logger.LogInformation("Publishing event: {EventName}", @event.GetType().Name);
            // Geçici implementasyon - gerçek RabbitMQ implementasyonu burada olacak
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);
            
            _subsManager.AddSubscription<T, TH>();
            // Geçici implementasyon - gerçek RabbitMQ subscription burada olacak
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);
            
            _subsManager.RemoveSubscription<T, TH>();
            // Geçici implementasyon - gerçek RabbitMQ unsubscription burada olacak
        }
        
        public void Dispose()
        {
            _logger.LogInformation("Disposing EventBusRabbitMQ");
        }
    }
} 