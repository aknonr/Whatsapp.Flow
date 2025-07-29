using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RabbitMQ.Client;
using System;
using Whatsapp.Flow.BuildingBlocks.EventBus;
using Whatsapp.Flow.BuildingBlocks.EventBus.Abstractions;
using Whatsapp.Flow.BuildingBlocks.EventBus.RabbitMQ;
using Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Commands;
using Whatsapp.Flow.Services.Identity.Application.Features.User.Commands;
using Whatsapp.Flow.Services.Identity.Application.Interfaces;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;
using Whatsapp.Flow.Services.Identity.Infrastructure.Repositories;
using Whatsapp.Flow.Services.Identity.Infrastructure.Services;
using Whatsapp.Flow.Services.Identity.Application.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authorization;
using Whatsapp.Flow.Services.Identity.API.Security;
using FluentValidation;
using Whatsapp.Flow.Services.Identity.Application.Common.Behaviors;
using System.Reflection;
using Whatsapp.Flow.Services.Identity.API.Middleware;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Swashbuckle.AspNetCore.SwaggerGen;
using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using System.Text;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

// Health Checks
var rabbitMQConfig = builder.Configuration.GetSection("RabbitMQ");
var rabbitMQConnectionString = new StringBuilder();

if (!string.IsNullOrEmpty(rabbitMQConfig["UserName"]) && !string.IsNullOrEmpty(rabbitMQConfig["Password"]))
{
    rabbitMQConnectionString.Append($"amqp://{rabbitMQConfig["UserName"]}:{rabbitMQConfig["Password"]}@{rabbitMQConfig["HostName"]}");
}
else
{
    rabbitMQConnectionString.Append($"amqp://{rabbitMQConfig["HostName"]}");
}


builder.Services.AddHealthChecks()
    .AddMongoDb(
        builder.Configuration.GetConnectionString("MongoDb"),
        name: "mongodb-check",
        tags: new string[] { "database" })
    .AddRedis(
        builder.Configuration.GetConnectionString("Redis"),
        name: "redis-check",
        tags: new string[] { "cache" })
    .AddRabbitMQ(
        rabbitMQConnectionString.ToString(),
        name: "rabbitmq-check",
        tags: new string[] { "message-broker" });


// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiPolicy", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
        if (allowedOrigins != null && allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});

// Rate Limiting
builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 100;
        options.Window = TimeSpan.FromSeconds(60);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;
    })
    .AddPolicy("login_policy", context => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: context.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromSeconds(60)
        })));


// Add services to the container.
builder.Services.AddHttpContextAccessor();


// AppSettings'den ayarları okuma ve DI'a ekleme
var jwtSettings = new JwtSettings();
builder.Configuration.Bind(JwtSettings.SectionName, jwtSettings);
builder.Services.AddSingleton(Options.Create(jwtSettings));

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
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

// Redis Cache
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));
builder.Services.AddSingleton<ICacheService, RedisCacheService>();


// Repositories
builder.Services.AddScoped<ITenantRepository, MongoTenantRepository>();
builder.Services.AddScoped<IUserRepository, MongoUserRepository>();
builder.Services.AddScoped<ISubscriptionRepository, MongoSubscriptionRepository>();
builder.Services.AddScoped<ITenantRoleRepository, MongoTenantRoleRepository>();

// Services
builder.Services.AddScoped<IPasswordService, PasswordService>();

// MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreateTenantCommandValidator).Assembly);

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

// Command Handlers (MediatR kullanmayanlar için)
builder.Services.AddScoped<RegisterUserCommandHandler>();


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
// 3. Event Bus'ı Kaydetme
builder.Services.AddEventBus(builder.Configuration);

// Authorization servislerini kaydetme
builder.Services.AddAuthorization();

// Authorization
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();



// Controllers'ı kaydetme
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add a custom operation filter which sets default values
    options.OperationFilter<SwaggerDefaultValues>();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
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

// --- Bitiş ---

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseRateLimiter();

app.UseMiddleware<ExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity.API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors("ApiPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers()
   .RequireRateLimiting("fixed");

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

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "Identity Service API",
            Version = description.ApiVersion.ToString(),
            Description = "API for managing tenants, users, roles, and subscriptions."
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        // TODO: IsDeprecated extension method bulunamadığı için geçici olarak devre dışı
        // operation.Deprecated |= apiDescription.IsDeprecated();

        if (operation.Parameters == null)
        {
            return;
        }

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);
            if (parameter.Description == null)
            {
                parameter.Description = description.ModelMetadata?.Description;
            }
            if (parameter.Schema.Default == null && description.DefaultValue != null)
            {
                parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
            }

            parameter.Required |= description.IsRequired;
        }
    }
}
