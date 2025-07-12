using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RabbitMQ.Client;
using System;
using Whatsapp.Flow.BuildingBlocks.EventBus;
using Whatsapp.Flow.BuildingBlocks.EventBus.Abstractions;
using Whatsapp.Flow.BuildingBlocks.EventBus.RabbitMQ;
using Whatsapp.Flow.Services.Flow.Application.IntegrationEvents.EventHandling;
using Whatsapp.Flow.Services.Flow.Application.Interfaces;
using Whatsapp.Flow.Services.Flow.Application.Services;
using Whatsapp.Flow.Services.Flow.Domain.Repositories;
using Whatsapp.Flow.Services.Flow.Infrastructure.Repositories;
using Whatsapp.Flow.Services.FlowService.Application.IntegrationEvents.EventHandling;
using Whatsapp.Flow.Services.Identity.Application.IntegrationEvents.Events;
using Whatsapp.Flow.Services.WebhookService.Application.IntegrationEvents.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Whatsapp.Flow.API", Version = "v1" });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key must appear in the X-API-KEY header.",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-KEY",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            },
            new string[]{}
        }
    });
});

// --- Bizim Eklediğimiz Bağımlılıklar ---

// 1. MongoDB Bağlantısını Kaydetme
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("MongoDb");

    // Bağlantı ayarlarını manuel olarak ve kesin bir şekilde oluşturma
    var url = new MongoUrl(connectionString);
    var settings = MongoClientSettings.FromUrl(url);
    settings.Credential = MongoCredential.CreateCredential("admin", "root", "example");

    return new MongoClient(settings);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("WhatsappFlowDb"); // Flow'a özel veritabanı
});


// 2. Repository'leri Kaydetme
builder.Services.AddScoped<IFlowRepository, MongoFlowRepository>();
builder.Services.AddScoped<ITenantRepository, MongoTenantRepository>();
builder.Services.AddScoped<IFlowStateRepository, MongoFlowStateRepository>();


// 3. Application Services
builder.Services.AddHttpClient<IWhatsappService, WhatsappService>();
builder.Services.AddScoped<FlowEngine>();


// 4. Event Bus'ı Kaydetme
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddTransient<WhatsappMessageReceivedIntegrationEventHandler>();
builder.Services.AddTransient<TenantInfoUpdatedIntegrationEventHandler>();


// --- Bitiş ---

var app = builder.Build();

// --- API Key Middleware ---
app.Use(async (context, next) =>
{
    // Swagger UI için anahtar kontrolünü atla
    if (context.Request.Path.StartsWithSegments("/swagger"))
    {
        await next();
        return;
    }

    if (!context.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Api Key was not provided.");
        return;
    }

    var apiKey = app.Configuration.GetValue<string>("Authentication:ApiKey");

    if (!apiKey.Equals(extractedApiKey))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized client.");
        return;
    }

    await next();
});
// --- Bitiş ---

// Event Bus aboneliğini yapılandırma
var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<WhatsappMessageReceivedIntegrationEvent, WhatsappMessageReceivedIntegrationEventHandler>();
eventBus.Subscribe<TenantInfoUpdatedIntegrationEvent, TenantInfoUpdatedIntegrationEventHandler>();


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
