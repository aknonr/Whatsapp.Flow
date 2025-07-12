using Microsoft.Extensions.Logging;
using System;

namespace Whatsapp.Flow.BuildingBlocks.EventBus.RabbitMQ
{
    // GEÇİCİ - Derleme hatalarını çözmek için basitleştirildi.
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        public DefaultRabbitMQPersistentConnection(object factory, ILogger<DefaultRabbitMQPersistentConnection> logger, int retryCount = 5)
        {
            _logger = logger;
        }

        public bool IsConnected => false;

        public object CreateModel()
        {
            _logger.LogWarning("CreateModel is not implemented in the temporary version.");
            return null;
        }

        public bool TryConnect()
        {
            _logger.LogWarning("TryConnect is not implemented in the temporary version.");
            return true;
        }
        
        public void Dispose()
        {
            // Nothing to dispose in temp version
        }
    }
} 