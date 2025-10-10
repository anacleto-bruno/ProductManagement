# Product Management API

A modern, cloud-native Product Management API built with .NET 9.0 and Azure Functions v4 using the Isolated Worker Model. This API provides comprehensive product management capabilities with advanced features like caching, validation, and comprehensive testing.

## 🚀 Features

### Core Functionality
- **Product Management**: Complete CRUD operations for products with colors and sizes
- **Advanced Filtering**: Search products by name, category, price range, and more
- **Pagination**: Efficient pagination for large product catalogs
- **Data Validation**: Input validation using FluentValidation
- **Error Handling**: Structured error responses with detailed messages

### Architecture & Design
- **Clean Architecture**: Separation of concerns across layers (Functions, Services, Infrastructure, Entities)
- **Repository Pattern**: Generic and specialized repositories with Entity Framework Core
- **SOLID Principles**: Following best practices for maintainable code
- **Dependency Injection**: Comprehensive DI container configuration
- **Result Pattern**: Structured error handling without exceptions for business logic

### Performance & Scalability
- **Redis Caching**: Distributed caching for improved performance
- **PostgreSQL**: Robust relational database with Entity Framework Core
- **Direct DTO Projection**: Optimized database queries without entity tracking overhead
- **Connection Pooling**: Efficient database connection management
- **Async/Await**: Non-blocking operations throughout the application

### DevOps & Quality
- **Docker Support**: Complete containerization with Docker Compose
- **Comprehensive Testing**: Unit tests, integration tests, and API tests
- **Code Coverage**: Detailed coverage reports with HTML visualization
- **Health Checks**: Built-in health monitoring endpoints
- **Swagger Documentation**: Interactive API documentation

## 🏗️ Technology Stack

### Backend Technologies
- **.NET 9.0** - Latest .NET framework
- **Azure Functions v4** - Serverless compute with Isolated Worker Model
- **Entity Framework Core 8.0** - ORM with PostgreSQL provider
- **FluentValidation** - Input validation framework
- **Serilog** - Structured logging

### Database & Caching
- **PostgreSQL 16** - Primary database
- **Redis 7** - Distributed caching layer
- **Entity Framework Migrations** - Database schema management

### Testing & Quality
- **xUnit** - Testing framework
- **FluentAssertions** - Fluent assertion library
- **FakeItEasy** - Mocking framework
- **Coverlet** - Code coverage collection
- **ReportGenerator** - HTML coverage reports

### Infrastructure
- **Docker & Docker Compose** - Containerization
- **Application Insights** - Telemetry and monitoring
- **Azure Functions Core Tools** - Local development

## 📁 Project Structure

```
backend/
├── api/                          # Main API project
│   ├── Functions/               # Azure Function endpoints
│   ├── Services/                # Business logic layer
│   ├── Infrastructure/          # Data access and external services
│   │   ├── Repositories/        # Repository implementations
│   │   └── Configurations/      # Entity configurations
│   ├── Entities/                # Domain models
│   ├── Dtos/                    # Data Transfer Objects
│   ├── Models/                  # Request/Response models
│   ├── Validators/              # FluentValidation validators
│   ├── Helpers/                 # Utility classes and extensions
│   ├── Migrations/              # Entity Framework migrations
│   └── wwwroot/                 # Static files (Swagger UI)
├── tests/                       # Test project
│   ├── Functions/               # Function endpoint tests
│   ├── Services/                # Business logic tests
│   ├── Infrastructure/          # Repository and data tests
│   ├── Integration/             # Integration tests
│   └── TestHelpers/             # Test utilities
└── coverage/                    # Test coverage reports
```

## 🛠️ Prerequisites

### Required Software
- **.NET 9.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Azure Functions Core Tools v4** - [Installation guide](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local)
- **Docker Desktop** - [Download here](https://www.docker.com/products/docker-desktop)
- **PowerShell 5.1+** - For running scripts

### Optional Tools
- **Visual Studio 2022 (17.8+)** or **VS Code** with Azure Functions extension
- **Azure CLI** - For cloud deployment
- **pgAdmin** - PostgreSQL database management tool

## 🚀 Getting Started

### 1. Clone the Repository
```powershell
git clone <repository-url>
cd "Product Management\backend"
```

### 2. Start Dependencies with Docker
```powershell
# Navigate to API directory
cd api

# Start PostgreSQL and Redis containers
docker-compose up -d

# Verify containers are running
docker-compose ps
```

### 3. Configure Database
```powershell
# Install Entity Framework tools (if not already installed)
dotnet tool install --global dotnet-ef

# Apply database migrations
dotnet ef database update --project api

# Verify database is seeded with sample data
```

### 4. Configure Application Settings
Create or update `api/local.settings.json`:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ConnectionStrings__DefaultConnection": "Host=localhost;Port=5432;Database=productmanagement;Username=postgres;Password=postgres",
    "ConnectionStrings__RedisConnection": "localhost:6379"
  }
}
```

### 5. Run the Application
```powershell
# Navigate to API directory (if not already there)
cd api

# Start the Azure Functions host
func start
```

The API will be available at:
- **API Base URL**: `http://localhost:7071`
- **Swagger UI**: `http://localhost:7071/api/swagger/ui`
- **Health Check**: `http://localhost:7071/api/health`

## 🧪 Testing

### Running Unit Tests
```powershell
# Navigate to backend directory
cd "Product Management\backend"

# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run specific test project
dotnet test tests/ProductManagement.UnitTests.csproj
```

### Generate Test Coverage Reports
```powershell
# Navigate to tests directory
cd tests

# Generate HTML coverage report
.\generate-html-report.ps1
```

This will:
1. Run all tests with coverage collection
2. Generate Cobertura XML reports
3. Create detailed HTML coverage reports
4. Open the coverage report in your default browser

### Coverage Report Locations
- **XML Reports**: `tests/coverage-results/`
- **HTML Reports**: `tests/coverage-html/`
- **Summary**: View `tests/coverage-html/index.html` for detailed coverage metrics

### Test Categories
- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test database operations and service integrations
- **API Tests**: End-to-end function testing
- **Repository Tests**: Data access layer testing

### Test Results Interpretation
- **Line Coverage**: Percentage of code lines executed during tests
- **Branch Coverage**: Percentage of code branches (if/else, switch) tested
- **Method Coverage**: Percentage of methods called during tests
- **Class Coverage**: Percentage of classes with at least one test

## 📊 API Endpoints

### Product Management
- `GET /api/products` - Get paginated products with filtering
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update existing product
- `DELETE /api/products/{id}` - Delete product

### Utility Endpoints
- `GET /api/health` - Application health check
- `GET /api/swagger/ui` - Interactive API documentation

### Query Parameters (GET /api/products)
- `search` - Search in product name, description, or brand
- `category` - Filter by product category
- `minPrice` - Minimum price filter
- `maxPrice` - Maximum price filter
- `sortBy` - Sort field (name, price, createdat)
- `descending` - Sort direction (true/false)
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 20, max: 100)

## 🐳 Docker Development

### Using Docker Compose
```powershell
# Start all services (PostgreSQL + Redis)
docker-compose up -d

# View service logs
docker-compose logs -f

# Stop all services
docker-compose down

# Reset database (remove volumes)
docker-compose down -v
```

### Individual Container Management
```powershell
# PostgreSQL operations
docker exec -it productmanagement-postgres psql -U postgres -d productmanagement

# Redis operations
docker exec -it productmanagement-redis redis-cli
```

## 📈 Performance Monitoring

### Health Checks
The application includes comprehensive health checks:
- Database connectivity
- Redis cache availability
- Application startup status

Access health endpoint: `GET /api/health`

### Logging
Structured logging with Serilog provides:
- Request/response logging
- Error tracking with stack traces
- Performance metrics
- Custom business event logging

### Caching Strategy
- **Redis Distributed Cache**: For frequently accessed product data
- **Connection Pooling**: Efficient database connections
- **Query Optimization**: Direct DTO projections, no entity tracking

## 🚀 Deployment

### Local Development
1. Use Docker Compose for dependencies
2. Run Azure Functions locally with `func start`
3. Access Swagger UI for API testing

### Production Deployment
1. **Azure Functions**: Deploy to Azure Functions Premium Plan
2. **Azure Database for PostgreSQL**: Managed PostgreSQL service
3. **Azure Cache for Redis**: Managed Redis service
4. **Application Insights**: Monitoring and telemetry

### Environment Configuration
Set up environment-specific `appsettings.json` files:
- `appsettings.Development.json`
- `appsettings.Docker.json`
- `appsettings.Production.json`

## 🤝 Contributing

### Development Workflow
1. Create feature branch from `main`
2. Implement changes following coding standards
3. Add comprehensive tests (unit + integration)
4. Ensure all tests pass with good coverage
5. Update documentation as needed
6. Create pull request with detailed description

### Coding Standards
- Follow C# naming conventions and best practices
- Use async/await for all I/O operations
- Implement proper error handling with Result pattern
- Add XML documentation for public APIs
- Maintain high test coverage (>80%)

### Commit Message Format
```
feat: add new product filtering capability
fix: resolve caching issue with product updates
docs: update API documentation
test: add integration tests for product service
```

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🔄 Changelog

### Version 1.0.0
- Initial release with core product management functionality
- Complete test suite with coverage reporting
- Docker development environment
- Swagger API documentation
- Redis caching implementation
- PostgreSQL database with Entity Framework Core

---

**Built with ❤️ using .NET 9.0 and Azure Functions**