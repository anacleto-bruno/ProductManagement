using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.Repositories;
using ProductManagement.Infrastructure.Repositories.Interfaces;
using ProductManagement.Models.Configuration;
using ProductManagement.Services;
using ProductManagement.Services.Interfaces;
using StackExchange.Redis;
using FluentValidation;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureOpenApi()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Configure Entity Framework with PostgreSQL
        services.AddDbContext<ProductManagementContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });

            if (context.HostingEnvironment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // Configure Redis (always register, but conditionally connect)
        var featureFlags = configuration.GetSection("FeatureFlags").Get<FeatureFlagsConfig>();
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var logger = provider.GetService<ILogger<Program>>();

            if (featureFlags?.UseRedisCache != true)
            {
                logger?.LogInformation("Redis cache is disabled via feature flags");
                return null!;
            }

            var redisConnectionString = configuration.GetConnectionString("RedisConnection");
            if (string.IsNullOrEmpty(redisConnectionString))
            {
                logger?.LogWarning("Redis connection string is not configured");
                return null!;
            }

            try
            {
                var multiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
                logger?.LogInformation("Successfully connected to Redis at {ConnectionString}", redisConnectionString);
                return multiplexer;
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Failed to connect to Redis at {ConnectionString}. Continuing without cache.", redisConnectionString);
                return null!;
            }
        });

        // Register configuration objects
        services.Configure<ExternalServiceConfig>(configuration.GetSection("ExternalServices"));
        services.Configure<FeatureFlagsConfig>(configuration.GetSection("FeatureFlags"));
        services.Configure<ApplicationSettingsConfig>(configuration.GetSection("ApplicationSettings"));
        services.Configure<CacheSettingsConfig>(configuration.GetSection("CacheSettings"));

        // Register repositories and unit of work
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register services
        services.AddScoped<ISeedDataService, SeedDataService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICacheService, RedisCacheService>();

        // Register FluentValidation
        services.AddValidatorsFromAssemblyContaining<Program>();

        // Add health checks
        services.AddHealthChecks()
            .AddDbContextCheck<ProductManagementContext>();
    })
    .ConfigureLogging((context, logging) =>
    {
        var featureFlags = context.Configuration.GetSection("FeatureFlags").Get<FeatureFlagsConfig>();

        if (featureFlags?.EnableAdvancedLogging == true)
        {
            logging.SetMinimumLevel(LogLevel.Debug);
        }
        else
        {
            logging.SetMinimumLevel(LogLevel.Information);
        }
    })
    .Build();

host.Run();
