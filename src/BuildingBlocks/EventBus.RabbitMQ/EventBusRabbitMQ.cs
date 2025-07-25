using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Whatsapp.Flow.BuildingBlocks.EventBus.Abstractions;
using Whatsapp.Flow.BuildingBlocks.EventBus.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
        private IModel _consumerChannel;
        private readonly string _brokerName = "whatsapp_flow_event_bus";
        private readonly string _exchangeName = "whatsapp_flow_exchange";

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
            IServiceProvider serviceProvider, IEventBusSubscriptionsManager subsManager, string queueName, int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _subsManager = subsManager ?? throw new ArgumentNullException(nameof(subsManager));
            _queueName = queueName;
            _retryCount = retryCount;
            
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var eventName = @event.GetType().Name;
            var routingKey = GetRoutingKey(@event);
            
            _logger.LogInformation("Publishing event: {EventName} with routing key: {RoutingKey}", eventName, routingKey);

            using var channel = _persistentConnection.CreateModel();
            channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2; // persistent

            channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: properties,
                body: body);
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            var routingKey = GetRoutingKey<T>();
            
            _logger.LogInformation("Subscribing to event {EventName} with routing key: {RoutingKey}", eventName, routingKey);
            
            DoInternalSubscription(eventName, routingKey);
            _subsManager.AddSubscription<T, TH>();
            StartBasicConsume();
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);
            
            _subsManager.RemoveSubscription<T, TH>();
        }

        private void DoInternalSubscription(string eventName, string routingKey)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                using var channel = _persistentConnection.CreateModel();
                channel.QueueBind(queue: _queueName,
                                exchange: _exchangeName,
                                routingKey: routingKey);
            }
        }

        private void StartBasicConsume()
        {
            if (_consumerChannel != null)
            {
                return;
            }

            _logger.LogInformation("Starting RabbitMQ basic consume");

            _consumerChannel = _persistentConnection.CreateModel();
            _consumerChannel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
            _consumerChannel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(_consumerChannel);
            consumer.Received += Consumer_Received;

            _consumerChannel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                await ProcessEvent(eventName, message);
                _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                _consumerChannel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogInformation("Processing RabbitMQ event: {EventName}", eventName);

            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    var handler = _serviceProvider.GetService(subscription.HandlerType);
                    if (handler == null) continue;

                    var eventType = _subsManager.GetEventTypeByName(eventName);
                    var integrationEvent = JsonSerializer.Deserialize(message, eventType);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                }
            }
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using var channel = _persistentConnection.CreateModel();
            channel.QueueUnbind(queue: _queueName,
                exchange: _exchangeName,
                routingKey: eventName);
        }

        private string GetRoutingKey<T>() where T : IntegrationEvent
        {
            return typeof(T).Name.ToLowerInvariant();
        }

        private string GetRoutingKey(IntegrationEvent @event)
        {
            return @event.GetType().Name.ToLowerInvariant();
        }
        
        public void Dispose()
        {
            _consumerChannel?.Dispose();
            _subsManager.OnEventRemoved -= SubsManager_OnEventRemoved;
            _logger.LogInformation("Disposing EventBusRabbitMQ");
        }
    }
} 