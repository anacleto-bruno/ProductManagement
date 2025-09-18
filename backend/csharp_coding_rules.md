# Coding Rules for {ProjectName} Functions

## Overview
This document outlines the coding standards, patterns, and conventions for the {ProjectName}Functions project built on .NET 9.0 Azure Functions v4 with Isolated Worker Model.

## Technology Stack

### Core Technologies
- **.NET 9.0** - Target framework
- **Azure Functions v4** - Isolated Worker Model
- **PostgreSQL** - Primary database with Entity Framework Core
- **Redis** - Caching layer
- **Docker** - Containerization
- **MongoDB** - Additional data storage (as indicated by MongoDB.Driver package)

---

## Architecture Patterns

### Clean Architecture
Maintain clear separation of concerns across layers:

```
{ProjectName}/
├── Functions/          # Azure Function endpoints (Presentation Layer)
├── Services/           # Business logic (Application Layer)
├── Infrastructure/     # External concerns (Infrastructure Layer)
├── Entities/          # Domain models (Domain Layer)
├── Dtos/              # Data Transfer Objects
├── Models/            # View models and request/response models
├── Helpers/           # Utility classes and extensions
├── Constants/         # Application constants
└── Enumerations/      # Enum definitions
```

### Dependency Flow Rules
- **Functions** → **Services** → **Infrastructure** → **Entities**
- **DTOs** can be used across all layers
- **Models** should primarily be used in Functions and Services
- **Entities** should not reference upper layers

---

## Naming Conventions

### Namespaces
- Use **lowercase** for namespace segments: `{ProjectName}.services`
- Follow project structure: `{ProjectName}.{FolderName}`

### Classes and Interfaces
- **PascalCase** for all class names: `ProductService`, `UserRepository`
- **Interface prefix**: Interfaces must start with `I`: `IProductService`, `IUserRepository`
- **Descriptive names**: Avoid abbreviations, use full descriptive names

### Methods and Properties
- **PascalCase** for public members: `GetUserAsync`, `IsActive`
- **camelCase** for private fields: `_connectionString`, `_logger`
- **Async suffix**: All async methods must end with `Async`: `ProcessDataAsync`

### Constants and Enums
- **PascalCase** for enum values: `Status.Active`, `Priority.High`
- **UPPER_CASE** for constants: `MAX_RETRY_COUNT`, `DEFAULT_TIMEOUT`

### Database Migrations
- **Date-based naming**: `YYYY.MM.DD` format for migration folders
- **Descriptive file names**: `001_CreateUserTable.sql`, `002_AddIndexToUserEmail.sql`

---

## Repository Pattern

### Generic Repository Structure
```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ProjectNameContext _context;
    
    public Repository(ProjectNameContext context)
    {
        _context = context;
    }
    // Implementation...
}
```

### Context Inheritance
- Use `ProjectNameContext` as the base DbContext
- Inherit from base repository for specific entity repositories
- Implement unit of work pattern for transaction management

---

## Dependency Injection

### Service Registration Patterns
```csharp
// In Program.cs
services.AddScoped<IProductService, ProductService>();
services.AddScoped<IUserRepository, UserRepository>();
services.AddSingleton<ICacheService, RedisCacheService>();
```

### Lifetime Management
- **Scoped**: Services that maintain state per request (Repositories, Business Services)
- **Singleton**: Stateless services, configuration, caching
- **Transient**: Lightweight, stateless utility services

### Constructor Injection
- Always use constructor injection
- Use readonly fields for injected dependencies
- Validate null dependencies in constructors

---

## Configuration Management

### appsettings Structure
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "...",
    "RedisConnection": "..."
  },
  "AzureWebJobsStorage": "...",
  "FeatureFlags": {
    "EnableAdvancedLogging": true,
    "UseRedisCache": true
  },
  "ExternalServices": {
    "ApiBaseUrl": "...",
    "Timeout": "00:00:30"
  }
}
```

### Environment-Specific Configuration
- **appsettings.json** - Base configuration
- **appsettings.Development.json** - Development overrides
- **appsettings.Docker.json** - Docker container settings
- **appsettings.Release.json** - Production overrides

### Configuration Binding
```csharp
public class ExternalServiceConfig
{
    public string ApiBaseUrl { get; set; }
    public TimeSpan Timeout { get; set; }
}

// In Program.cs
services.Configure<ExternalServiceConfig>(
    configuration.GetSection("ExternalServices"));
```

---

## Testing Strategy

### Project Structure
Mirror the main project structure in test project:
```
{ProjectName}.Tests/
├── Functions/          # Function endpoint tests
├── Services/           # Business logic tests
├── Infrastructure/     # Infrastructure layer tests
├── Helpers/           # Utility test classes
├── Dtos/              # DTO validation tests
└── Models/            # Model tests
```

### Testing Frameworks
- **xUnit** - Primary testing framework
- **FluentAssertions** - Assertion library
- **FakeItEasy** - Mocking framework (as indicated by TestsBase)
- **Microsoft.EntityFrameworkCore.InMemory** - In-memory database for unit tests

### Test Naming Convention
```csharp
[Fact]
public async Task GetUserAsync_WithValidId_ReturnsUser()
{
    // Method_Scenario_ExpectedResult
}

[Theory]
[InlineData(1, "John")]
[InlineData(2, "Jane")]
public async Task GetUserAsync_WithMultipleIds_ReturnsCorrectUsers(int id, string expectedName)
{
    // Theory tests for multiple inputs
}
```

### Test Categories
1. **Unit Tests**: Test individual components in isolation
2. **Integration Tests**: Test component interactions with real database
3. **API Tests**: End-to-end function testing
4. **Smoke Tests**: Basic functionality verification

---

## Azure Functions Architecture Patterns

### Clean Function Design Principles

#### Base Class Pattern
- **Create abstract base classes** to eliminate code duplication across functions
- **Implement common infrastructure** for HTTP response handling, error management, and request parsing
- **Use generic base classes** to handle different function types (with/without validation)

```csharp
// Example: Base class for functions without input validation
public abstract class BaseFunction
{
    protected readonly ILogger _logger;

    protected BaseFunction(ILogger logger) => _logger = logger;

    protected async Task<HttpResponseData> ExecuteSafelyAsync<TResponse>(
        HttpRequestData request, Func<Task<TResponse>> handler)
    {
        // Centralized error handling and response creation
    }
}

// Example: Base class for functions with input validation
public abstract class BaseFunctionWithValidation<TRequest, TValidator>
    where TRequest : new()
    where TValidator : AbstractValidator<TRequest>, new()
{
    protected async Task<HttpResponseData> ExecuteWithValidationAsync<TResponse>(
        HttpRequestData request, Func<TRequest, Task<TResponse>> handler)
    {
        // Automatic request parsing, validation, and response handling
    }
}
```

#### Infrastructure Helper Classes
- **Centralize HTTP operations** in dedicated helper classes
- **Standardize JSON serialization** with consistent naming conventions
- **Create reusable request/response utilities** for common operations

```csharp
// HTTP Response Helper
public static class HttpResponseHelper
{
    public static Task<HttpResponseData> CreateJsonResponseAsync<T>(
        HttpRequestData request, T data, HttpStatusCode statusCode = HttpStatusCode.OK);

    public static Task<HttpResponseData> CreateErrorResponseAsync(
        HttpRequestData request, string message, HttpStatusCode statusCode);
}

// Request Helper
public static class RequestHelper
{
    public static Task<T> ParseJsonBodyAsync<T>(HttpRequestData request) where T : new();
}
```

### Function Implementation Patterns

#### ✅ RECOMMENDED: Clean Function Pattern
```csharp
public class ProductFunction : BaseFunctionWithValidation<ProductRequestDto, ProductRequestValidator>
{
    private readonly IProductService _productService;

    public ProductFunction(ILogger<ProductFunction> logger, IProductService productService,
        ProductRequestValidator validator) : base(logger, validator)
    {
        _productService = productService;
    }

    [Function("ProcessProduct")]
    public async Task<HttpResponseData> ProcessAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] HttpRequestData req)
    {
        return await ExecuteWithValidationAsync<ProductResponseDto>(req, async productRequest =>
        {
            // Focus only on business logic - infrastructure is handled by base class
            var result = await _productService.ProcessAsync(productRequest);
            return result;
        });
    }
}
```

#### ❌ AVOID: Manual Infrastructure Pattern
```csharp
// Don't implement manual try-catch, JSON parsing, validation, and response creation
public class ProductFunction
{
    [Function("ProcessProduct")]
    public async Task<HttpResponseData> ProcessAsync(HttpRequestData req)
    {
        try
        {
            // Manual JSON parsing
            // Manual validation
            // Manual error handling
            // Manual response creation
        }
        catch (Exception ex)
        {
            // Manual error response
        }
    }
}
```

### Code Reduction Benefits
- **Eliminate 60-80% of boilerplate code** in function implementations
- **Centralize error handling** - no need for try-catch in every function
- **Automatic input validation** with consistent error responses
- **Standardized JSON serialization** across all endpoints
- **Consistent logging patterns** throughout the application

### Function Design Rules
1. **Always inherit from base classes** - never implement infrastructure manually
2. **Focus functions on business logic only** - let infrastructure handle HTTP concerns
3. **Use dependency injection** for all services and validators
4. **Implement specific, action-oriented function names** (avoid generic names like "Run")
5. **Throw business exceptions** - let base classes handle HTTP error responses
6. **Use HttpRequestData/HttpResponseData** pattern for isolated worker model

### Validation Integration
- **Integrate FluentValidation** seamlessly with base classes
- **Automatic validation error responses** with structured field-level errors
- **Consistent error format** across all validated endpoints
- **Separation of validation logic** from function implementation

### Error Handling Strategy
- **Centralized exception handling** in base classes
- **Structured error responses** with consistent format
- **Automatic logging** of errors with correlation context
- **Business logic exceptions** automatically converted to appropriate HTTP responses
- **Validation errors** automatically formatted as structured responses

---

## Code Quality Standards

### SOLID Principles
- **Single Responsibility**: Each class should have one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Derived classes must be substitutable for base classes
- **Interface Segregation**: Clients shouldn't depend on interfaces they don't use
- **Dependency Inversion**: Depend on abstractions, not concretions

### Error Handling
```csharp
public async Task<Result<User>> GetUserAsync(int id)
{
    try
    {
        if (id <= 0)
            return Result<User>.Failure("Invalid user ID");
            
        var user = await _repository.GetByIdAsync(id);
        return user != null 
            ? Result<User>.Success(user)
            : Result<User>.Failure("User not found");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
        return Result<User>.Failure("An error occurred while retrieving the user");
    }
}
```

### Logging Standards
- Use structured logging with Microsoft.Extensions.Logging
- Include correlation IDs for request tracking
- Log at appropriate levels: Trace, Debug, Information, Warning, Error, Critical
- Include relevant context in log messages

### Performance Guidelines
- Use async/await for I/O operations
- Implement caching for frequently accessed data
- Use connection pooling for database connections
- Implement pagination for large data sets
- Use streaming for large file operations

---

## Security Guidelines

### Authentication & Authorization
- Validate all input parameters
- Implement proper CORS policies
- Use HTTPS for all external communications

### Data Protection
- Sanitize HTML content using `HtmlSanitizer`
- Validate and sanitize all user inputs
- Use parameterized queries to prevent SQL injection
- Encrypt sensitive data at rest and in transit

### Secret Management
- Store secrets in Azure Key Vault
- Use managed identities when possible
- Never commit secrets to source control
- Rotate secrets regularly

---

## Docker & Deployment

### Dockerfile Standards
- Use multi-stage builds for optimization
- Follow least privilege principle
- Minimize layer count
- Use specific base image versions

### Environment Variables
- Use environment variables for configuration
- Provide default values where appropriate
- Document all required environment variables

---

## Documentation Standards

### Code Documentation
- Use XML documentation comments for public APIs
- Include examples in documentation
- Document complex business logic
- Keep documentation up-to-date with code changes

### README Requirements
- Project description and purpose
- Setup and installation instructions
- Configuration requirements
- API documentation
- Testing instructions
- Deployment procedures

### Task Management Integration
- Commit changes when completing a task in task-manager
- Include task ID and summary in commit message
- Follow format: `task(#ID): Summary of completed work`
- Reference any related issues or pull requests in commit body
- Ensure all automated tests pass before committing

---

## Continuous Integration

### Build Standards
- All builds must pass without warnings
- Run all tests before merging
- Perform static code analysis
- Generate code coverage reports

### Quality Gates
- Minimum 80% code coverage for new code
- All critical and high-severity security issues must be resolved
- Performance benchmarks must be met
- All integration tests must pass

---

## Additional Considerations

### Monitoring & Observability
- Implement Application Insights integration
- Add custom metrics for business operations
- Create dashboards for key performance indicators
- Set up alerting for critical failures

### Scalability
- Design for horizontal scaling
- Implement circuit breaker patterns
- Use async patterns throughout
- Consider implementing CQRS for complex read/write scenarios

### Maintenance
- Regular dependency updates
- Database migration reviews
- Performance monitoring and optimization
- Security vulnerability assessments

---

## Questions for Improvement

1. **API Versioning**: How do you handle API versioning in your Azure Functions?
2. **Event-Driven Architecture**: Are you using Event Grid, Service Bus, or other messaging patterns?
3. **Background Processing**: Do you need guidelines for timer-triggered functions or queue processing?
4. **Cross-Cutting Concerns**: What patterns do you use for concerns like auditing, caching, and rate limiting?
5. **Database Patterns**: Do you use any specific patterns like CQRS, Event Sourcing, or Domain Events?
6. **Integration Patterns**: How do you handle integration with external services and APIs?
7. **Deployment Strategies**: What deployment patterns do you follow (blue-green, canary, etc.)?
8. **Monitoring and Alerting**: What specific monitoring and alerting strategies should be included?

---

*This document should be treated as a living document and updated as the project evolves and new patterns emerge.*
