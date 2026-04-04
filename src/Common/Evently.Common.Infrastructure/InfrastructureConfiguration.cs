using Dapper;
using Evently.Common.Application.Caching;
using Evently.Common.Application.Clock;
using Evently.Common.Application.Data;
using Evently.Common.Application.EventBus;
using Evently.Common.Infrastructure.Authentication;
using Evently.Common.Infrastructure.Authorization;
using Evently.Common.Infrastructure.Caching;
using Evently.Common.Infrastructure.Clock;
using Evently.Common.Infrastructure.Data;
using Evently.Common.Infrastructure.Outbox;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quartz;
using StackExchange.Redis;

namespace Evently.Common.Infrastructure;

/// <summary>
/// Composition root de la couche Infrastructure commune.
/// </summary>
public static class InfrastructureConfiguration
{
    /// <summary>
    /// Enregistre tous les services techniques partagés : auth, bus, data, cache, télémétrie.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string serviceName,
        Action<IRegistrationConfigurator>[] moduleConfigureConsumers,
        string redisConnectionString)
    {
        // Configuration sécurité cross-module.
        services.AddAuthenticationInternal();
        services.AddAuthorizationInternal();

        // Services singleton simples et sans état.
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.TryAddSingleton<IEventBus, EventBus.EventBus>();

        // Intercepteur EF Core chargé d'alimenter l'outbox transactionnelle.
        services.TryAddSingleton<InsertOutboxMessagesInterceptor>();

        // Fabrique Dapper dédiée aux requêtes SQL bas niveau.
        services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();

        // Type handler nécessaire pour sérialiser/désérialiser les tableaux PostgreSQL.
        SqlMapper.AddTypeHandler(new GenericArrayHandler<string>());

        // Quartz est utilisé pour les jobs récurrents (inbox/outbox processing).
        services.AddQuartz(configurator =>
        {
            var scheduler = Guid.NewGuid();
            configurator.SchedulerId = $"default-id-{scheduler}";
            configurator.SchedulerName = $"default-name-{scheduler}";
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        // On tente Redis en priorité. Si indisponible, fallback in-memory
        // pour permettre à l'application de démarrer en environnement local.
        try
        {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddSingleton(connectionMultiplexer);
            services.AddStackExchangeRedisCache(options =>
                options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));
        }
        catch
        {
            services.AddDistributedMemoryCache();
        }

        services.TryAddSingleton<ICacheService, CacheService>();

        // Bus de messages interne (MassTransit + transport in-memory par défaut).
        services.AddMassTransit(configure =>
        {
            // Chaque module enregistre ses propres consumers.
            foreach (Action<IRegistrationConfigurator> configureConsumers in moduleConfigureConsumers)
            {
                configureConsumers(configure);
            }

            configure.SetKebabCaseEndpointNameFormatter();

            configure.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        // OpenTelemetry : traces web, HTTP sortant, EF, Redis et Npgsql.
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddRedisInstrumentation()
                    .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);

                tracing.AddOtlpExporter();
            });

        return services;
    }
}
