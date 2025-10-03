# Product Management Backend API

A .NET 9 Azure Functions v4 (Isolated Worker Model) API for comprehensive product management with PostgreSQL, Entity Framework Core, and Docker support.

## 🚀 Features

- **CRUD Operations**: Complete product lifecycle management
- **Advanced Search & Filtering**: Search across multiple fields with pagination
- **Data Seeding**: Generate mock product data for testing (1-10,000 records)
- **Clean Architecture**: Following SOLID principles with clear separation of concerns
- **Database**: PostgreSQL with Entity Framework Core
- **Validation**: FluentValidation for robust input validation
- **Error Handling**: Comprehensive error handling with structured responses
- **Docker Support**: Containerized database and optional API deployment
- **Performance Optimized**: Efficient queries with proper indexing and pagination

## 🛠️ Technology Stack

- **.NET 9.0** - Target framework
- **Azure Functions v4** - Isolated Worker Model
- **PostgreSQL** - Primary database with Entity Framework Core
- **Entity Framework Core 9.0** - ORM with code-first approach
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **Docker** - Containerization
- **Bogus** - Mock data generation

## 📁 Project Structure

```
ProductManagement/
├── Functions/              # Azure Function endpoints (Presentation Layer)
├── Services/               # Business logic (Application Layer)
├── Infrastructure/         # External concerns (Infrastructure Layer)
│   ├── Repositories/       # Data access layer
│   └── Configurations/     # EF configurations
├── Entities/              # Domain models (Domain Layer)
├── Dtos/                  # Data Transfer Objects
├── Models/                # View models and request/response models
├── Helpers/               # Utility classes and extensions
├── Validators/            # FluentValidation validators
├── Database/              # Database initialization scripts
└── Docker/                # Docker configuration files
```

## 🚀 Quick Start

### Prerequisites

- .NET 9.0 SDK
- Docker and Docker Compose
- Azure Functions Core Tools v4
- PostgreSQL (if running without Docker)

### 1. Clone and Setup

```powershell
git clone <repository-url>
cd "Product Management\backend"
```

### 2. Start Database Services

```powershell
docker-compose up -d postgres redis
```

This starts:
- PostgreSQL on `localhost:5432`
- Redis on `localhost:6379`
- Initializes database with basic colors and sizes

### 3. Run the API

```powershell
# Restore dependencies
dotnet restore

# Run the Function App
func start --dotnet-isolated
```

The API will be available at `http://localhost:7071`

## 📊 Database Schema

### Core Entities

- **Products**: Main product information (name, description, model, brand, SKU, price, category)
- **Colors**: Available colors with optional hex codes
- **Sizes**: Available sizes with sorting order
- **ProductColors**: Many-to-many relationship between products and colors
- **ProductSizes**: Many-to-many relationship between products and sizes (includes stock quantity)

### Key Features

- **Unique SKU constraint** prevents duplicate product identifiers
- **Composite indexes** for optimized query performance
- **Soft relationships** allowing products to have multiple colors and sizes
- **Automatic timestamps** for audit trails

## 🔌 API Endpoints

### Product Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/products` | Get paginated products with search/filter |
| `GET` | `/api/products/{id}` | Get product by ID |
| `POST` | `/api/products` | Create new product |
| `PUT` | `/api/products/{id}` | Update product |
| `DELETE` | `/api/products/{id}` | Delete product |
| `POST` | `/api/products/seed/{count?}` | Seed database (default: 100, max: 10,000) |

### Query Parameters for GET /api/products

- `page` (int): Page number (default: 1)
- `pageSize` (int): Items per page (1-100, default: 20)
- `searchTerm` (string): Search across name, description, brand, model, SKU
- `category` (string): Filter by category
- `minPrice` (decimal): Minimum price filter
- `maxPrice` (decimal): Maximum price filter
- `sortBy` (string): Sort field (name, price, brand, model, category, createdAt)
- `descending` (bool): Sort direction (default: false)

### Example Requests

#### Create Product
```json
POST /api/products
{
  "name": "Premium T-Shirt",
  "description": "High-quality cotton t-shirt",
  "model": "PT-2024",
  "brand": "Fashion Co",
  "sku": "FSH-PT-2024-001",
  "price": 29.99,
  "category": "Clothing",
  "colorIds": [1, 2, 3],
  "sizeIds": [2, 3, 4]
}
```

#### Search Products
```
GET /api/products?searchTerm=shirt&category=Clothing&minPrice=20&maxPrice=100&sortBy=price&page=1&pageSize=10
```

#### Seed Database
```
POST /api/products/seed/500
```

## 🧪 Response Examples

### Paginated Products Response
```json
{
  "data": [
    {
      "id": 1,
      "name": "Premium T-Shirt",
      "model": "PT-2024",
      "brand": "Fashion Co",
      "sku": "FSH-PT-2024-001",
      "price": 29.99,
      "category": "Clothing"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 150,
  "totalPages": 8,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

### Single Product Response
```json
{
  "id": 1,
  "name": "Premium T-Shirt",
  "description": "High-quality cotton t-shirt",
  "model": "PT-2024",
  "brand": "Fashion Co",
  "sku": "FSH-PT-2024-001",
  "price": 29.99,
  "category": "Clothing",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z",
  "colors": [
    {"id": 1, "name": "Red", "hexCode": "#FF0000"},
    {"id": 2, "name": "Blue", "hexCode": "#0000FF"}
  ],
  "sizes": [
    {"id": 2, "name": "Small", "code": "S", "sortOrder": 2},
    {"id": 3, "name": "Medium", "code": "M", "sortOrder": 3}
  ]
}
```

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection | localhost connection |
| `ConnectionStrings__RedisConnection` | Redis connection | localhost:6379 |
| `FeatureFlags__EnableAdvancedLogging` | Enhanced logging | true |
| `FeatureFlags__UseRedisCache` | Redis caching | true |

### appsettings.json Structure
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=productmanagement;Username=postgres;Password=postgres;Port=5432",
    "RedisConnection": "localhost:6379"
  },
  "FeatureFlags": {
    "EnableAdvancedLogging": true,
    "UseRedisCache": true
  }
}
```

## 🚀 Deployment

### Docker Deployment

```powershell
# Build and run everything with Docker
docker-compose up --build

# Or just the database services
docker-compose up -d postgres redis
```

### Azure Functions Deployment

```powershell
# Publish to Azure
func azure functionapp publish <your-function-app-name>
```

## 🧪 Testing

### Manual Testing with HTTP files

Create test requests in VS Code or use tools like Postman:

```http
### Get all products
GET http://localhost:7071/api/products

### Create product
POST http://localhost:7071/api/products
Content-Type: application/json

{
  "name": "Test Product",
  "model": "TP-001",
  "brand": "Test Brand",
  "sku": "TB-TP-001",
  "price": 19.99,
  "colorIds": [1, 2],
  "sizeIds": [2, 3]
}

### Seed database
POST http://localhost:7071/api/products/seed/100
```

## 🔍 Performance Features

- **Database Indexing**: Optimized indexes on searchable fields
- **Direct DTO Projection**: Efficient queries without entity tracking
- **Pagination**: All list operations support pagination
- **Query Extensions**: Reusable, composable query filters
- **Connection Pooling**: Efficient database connection management

## 🛡️ Error Handling

The API provides structured error responses:

```json
// Validation errors
{
  "errors": [
    "Product name is required",
    "Price must be greater than 0"
  ]
}

// General errors
{
  "error": "Product not found"
}
```

## 📝 Logging

Structured logging with Serilog includes:
- Request/response logging
- Error tracking with correlation IDs
- Performance metrics
- Business operation auditing

## 🤝 Contributing

1. Follow the coding guidelines in `copilot-instructions.md`
2. Use clean architecture patterns
3. Write comprehensive tests
4. Update documentation for new features

## 📄 License

This project is licensed under the MIT License.