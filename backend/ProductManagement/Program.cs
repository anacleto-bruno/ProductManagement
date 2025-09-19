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

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
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

        // Configure Redis (if enabled)
        var featureFlags = configuration.GetSection("FeatureFlags").Get<FeatureFlagsConfig>();
        if (featureFlags?.UseRedisCache == true)
        {
            var redisConnectionString = configuration.GetConnectionString("RedisConnection");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                services.AddSingleton<IConnectionMultiplexer>(provider =>
                {
                    var logger = provider.GetService<ILogger<Program>>();
                    try
                    {
                        return ConnectionMultiplexer.Connect(redisConnectionString);
                    }
                    catch (Exception ex)
                    {
                        logger?.LogWarning(ex, "Failed to connect to Redis. Continuing without cache.");
                        return null!;
                    }
                });
            }
        }

        // Register configuration objects
        services.Configure<ExternalServiceConfig>(configuration.GetSection("ExternalServices"));
        services.Configure<FeatureFlagsConfig>(configuration.GetSection("FeatureFlags"));
        services.Configure<ApplicationSettingsConfig>(configuration.GetSection("ApplicationSettings"));

        // Register repositories and unit of work
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register services
        services.AddScoped<ISeedDataService, SeedDataService>();
        services.AddScoped<IProductService, ProductService>();

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
