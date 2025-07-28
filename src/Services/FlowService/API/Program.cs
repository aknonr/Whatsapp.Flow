using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json.Serialization;
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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Flow Service API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// JWT Authentication
var jwtKey = builder.Configuration["JwtSettings:Key"];
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

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
builder.Services.AddScoped<IFlowTemplateRepository, MongoFlowTemplateRepository>();


// 3. Application Services
builder.Services.AddHttpClient<IWhatsappService, WhatsappService>();
builder.Services.AddScoped<FlowEngine>();
builder.Services.AddScoped<IFlowNoteService, FlowNoteService>();


// 4. Event Bus'ı Kaydetme
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddTransient<WhatsappMessageReceivedIntegrationEventHandler>();
builder.Services.AddTransient<TenantInfoUpdatedIntegrationEventHandler>();


// --- Bitiş ---

var app = builder.Build();

// Event Bus aboneliğini yapılandırma
var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<Whatsapp.Flow.Services.Flow.Application.IntegrationEvents.Events.WhatsappMessageReceivedIntegrationEvent, WhatsappMessageReceivedIntegrationEventHandler>();
eventBus.Subscribe<TenantInfoUpdatedIntegrationEvent, TenantInfoUpdatedIntegrationEventHandler>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flow Service API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger");
        return;
    }
    await next();
});

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
