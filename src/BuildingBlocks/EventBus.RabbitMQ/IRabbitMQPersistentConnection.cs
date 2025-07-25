using System;
using RabbitMQ.Client;

namespace Whatsapp.Flow.BuildingBlocks.EventBus.RabbitMQ
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();
    }
} 