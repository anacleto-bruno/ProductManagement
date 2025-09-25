using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Functions.Worker;
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
    })
    .Build();

// Ensure database is created and migrations are applied
using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

host.Run();