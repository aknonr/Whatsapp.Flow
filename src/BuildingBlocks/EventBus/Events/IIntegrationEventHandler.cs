using System.Threading.Tasks;

namespace Whatsapp.Flow.BuildingBlocks.EventBus.Events
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
} 