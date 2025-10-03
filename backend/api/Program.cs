using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using ProductManagement.infrastructure;
using ProductManagement.infrastructure.repositories;
using ProductManagement.services;
using ProductManagement.services.caching;
using ProductManagement.services.decorators;
using ProductManagement.validators;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        // Configure Application Insights
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Configure Entity Framework with PostgreSQL
        var connectionString = context.Configuration["ConnectionStrings__DefaultConnection"] 
            ?? context.Configuration["ConnectionStrings:DefaultConnection"]
            ?? throw new InvalidOperationException("DefaultConnection string not found");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            
            // Configure for development
            if (context.HostingEnvironment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // Repository Pattern Registration
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProductRepository, ProductRepository>();

        // Redis Configuration (optional)
        var redisConnectionString = context.Configuration["ConnectionStrings__Redis"] 
            ?? context.Configuration["ConnectionStrings:Redis"];
        
        // Configure cache options
        services.Configure<CacheOptions>(context.Configuration.GetSection(CacheOptions.SectionName));
        
        // Cache services (register NoOp fallback; override with Redis below if available)
        services.AddSingleton<ICacheService, NoOpCacheService>();

        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            try
            {
                services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(provider =>
                    StackExchange.Redis.ConnectionMultiplexer.Connect(redisConnectionString));
                
                // Replace NoOp cache with Redis-backed implementation
                services.AddSingleton<ICacheService>(provider =>
                {
                    var mux = provider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
                    var logger = provider.GetRequiredService<ILogger<RedisCacheService>>();
                    return new RedisCacheService(mux, logger);
                });
            }
            catch (Exception ex)
            {
                // Log Redis connection failure but don't fail startup
                Console.WriteLine($"Redis connection failed: {ex.Message}");
            }
        }

        // Business Services (register concrete then decorated interface)
        services.AddScoped<ProductService>();
        services.AddScoped<IProductService>(sp =>
        {
            var inner = sp.GetRequiredService<ProductService>();
            var cache = sp.GetRequiredService<ICacheService>();
            var logger = sp.GetRequiredService<ILogger<CachedProductService>>();
            var cacheOptions = sp.GetRequiredService<IOptions<CacheOptions>>();
            return new CachedProductService(inner, cache, logger, cacheOptions);
        });
        services.AddScoped<IHealthCheckService, HealthCheckService>();

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();
        services.AddScoped<CreateProductRequestValidator>();
        services.AddScoped<UpdateProductRequestValidator>();
        services.AddScoped<PaginationRequestValidator>();

        // OpenAPI Configuration
        services.AddSingleton<IOpenApiConfigurationOptions>(_ =>
        {
            var options = new OpenApiConfigurationOptions()
            {
                Info = new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Product Management API",
                    Description = "A comprehensive REST API for managing products, colors, and sizes with full CRUD operations, pagination, search, and data seeding capabilities.",
                    Contact = new OpenApiContact()
                    {
                        Name = "Product Management Team",
                        Email = "support@productmanagement.com"
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                },
                Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
                OpenApiVersion = OpenApiVersionType.V3,
                IncludeRequestingHostName = true,
                ForceHttps = false,
                ForceHttp = false
            };

            return options;
        });
    })
    .Build();

// Ensure database is created and migrations are applied
using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

host.Run();