using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RabbitMQ.Client;
using System;
using Whatsapp.Flow.BuildingBlocks.EventBus;
using Whatsapp.Flow.BuildingBlocks.EventBus.Abstractions;
using Whatsapp.Flow.BuildingBlocks.EventBus.RabbitMQ;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;
using Whatsapp.Flow.Services.Identity.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR'ı ekliyoruz. Bu, Application katmanındaki tüm handler'ları otomatik olarak bulur.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));


// --- Bizim Eklediğimiz Bağımlılıklar ---

// 1. MongoDB Bağlantısını Kaydetme
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("MongoDb");
    return new MongoClient(connectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var dbName = builder.Configuration.GetValue<string>("DatabaseSettings:DatabaseName");
    return client.GetDatabase(dbName);
});


// 2. Repository'leri Kaydetme
builder.Services.AddScoped<IUserRepository, MongoUserRepository>();
builder.Services.AddScoped<ITenantRepository, MongoTenantRepository>();

// 3. Event Bus'ı Kaydetme
builder.Services.AddEventBus(builder.Configuration);

// --- Bitiş ---

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); 

app.Run();


public static class CustomExtensionMethods
{
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMQConfig = configuration.GetSection("RabbitMQ");
        var retryCount = rabbitMQConfig.GetValue<int>("RetryCount");

        services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQConfig["HostName"]
            };

            if (!string.IsNullOrEmpty(rabbitMQConfig["UserName"]))
            {
                factory.UserName = rabbitMQConfig["UserName"];
            }

            if (!string.IsNullOrEmpty(rabbitMQConfig["Password"]))
            {
                factory.Password = rabbitMQConfig["Password"];
            }

            var persistentConnection = new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            persistentConnection.TryConnect();
            return persistentConnection;
        });

        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        {
            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, sp, eventBusSubcriptionsManager, configuration["SubscriptionClientName"], retryCount);
        });

        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        return services;
    }
}
