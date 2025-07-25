using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Whatsapp.Flow.BuildingBlocks.EventBus.Abstractions;

namespace Whatsapp.Flow.BuildingBlocks.EventBus.Extensions
{
    public static class EventBusExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, string queueName)
        {
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            return services;
        }
    }
} 