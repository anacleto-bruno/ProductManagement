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
using ProductManagement.validators;
using FluentValidation;

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

        // Business Services
        services.AddScoped<IProductService, ProductService>();

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