using System;

namespace Whatsapp.Flow.BuildingBlocks.EventBus.RabbitMQ
{
    // GEÇİCİ - Derleme hatalarını çözmek için basitleştirildi.
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        object CreateModel(); // Geçici olarak object kullanıldı.
    }
} 