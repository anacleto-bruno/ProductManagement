# 🛍️ Product Management System

> A comprehensive, enterprise-grade product management solution with a modern React frontend and a scalable .NET Azure Functions backend. Built for managing product catalogs at scale with advanced features like search, pagination, caching, and full CRUD operations.

[![.NET](https://img.shields.io/badge/.NET-9.0-512bd4.svg)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19.1-61dafb.svg)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.9-blue.svg)](https://www.typescriptlang.org/)
[![Azure Functions](https://img.shields.io/badge/Azure%20Functions-v4-0078d4.svg)](https://azure.microsoft.com/en-us/services/functions/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791.svg)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Architecture](#-architecture)
- [Technology Stack](#-technology-stack)
- [Project Structure](#-project-structure)
- [Quick Start](#-quick-start)
- [Development](#-development)
- [API Documentation](#-api-documentation)
- [Testing](#-testing)
- [Deployment](#-deployment)
- [Contributing](#-contributing)

---

## 🎯 Overview

The **Product Management System** is a full-stack solution designed for comprehensive product catalog management. It combines a powerful .NET backend with a modern React frontend to deliver a scalable, maintainable, and user-friendly product management experience.

### Key Highlights

- 🏗️ **Microservices Architecture** with separated frontend and backend concerns
- ⚡ **High Performance** with Redis caching and optimized database queries
- 🔒 **Type-Safe** development with TypeScript and C# strong typing
- 📱 **Responsive Design** that works on all devices
- 🧪 **Comprehensive Testing** with >90% backend and >80% frontend coverage
- 🐳 **Docker Support** with complete containerization
- 🌐 **Internationalization** ready for global deployment

---

## ✨ Features

### Core Product Management
- **Product CRUD Operations** - Create, read, update, and delete products
- **Advanced Search** - Real-time search across multiple product attributes
- **Pagination & Filtering** - Efficient handling of large product catalogs
- **Color & Size Management** - Support for product variants with colors and sizes
- **Data Seeding** - Generate sample data for testing and development
- **Bulk Operations** - Efficient batch processing for multiple products

### Technical Features
- **RESTful API** - Well-designed REST endpoints with OpenAPI documentation
- **Distributed Caching** - Redis integration for improved performance
- **Database Migrations** - Entity Framework Core migrations for schema management
- **Health Monitoring** - Built-in health checks and monitoring endpoints
- **Structured Logging** - Comprehensive logging with Serilog
- **Error Handling** - Graceful error handling with detailed responses

### User Experience
- **Modern UI/UX** - Material-UI components with custom theming
- **Dark Mode Support** - Toggle between light and dark themes
- **Real-time Updates** - Instant UI updates with optimistic rendering
- **Mobile Responsive** - Optimized for all screen sizes
- **Loading States** - Skeleton screens and progress indicators

---

## 🏗️ Architecture

The system follows a **microservices architecture** with clear separation between frontend and backend:

```
┌─────────────────────┐    HTTP/REST    ┌──────────────────────┐
│                     │ ◄──────────────► │                      │
│   React Frontend    │                  │   .NET Backend       │
│   (Port 3000)       │                  │   (Port 7071)        │
│                     │                  │                      │
└─────────────────────┘                  └───────────┬──────────┘
                                                     │
                                                     ▼
                                         ┌──────────────────────┐
                                         │                      │
                                         │   PostgreSQL DB      │
                                         │   (Port 5432)        │
                                         │                      │
                                         └──────────────────────┘
                                         ┌──────────────────────┐
                                         │                      │
                                         │   Redis Cache        │
                                         │   (Port 6379)        │
                                         │                      │
                                         └──────────────────────┘
```

### Backend Architecture
- **Azure Functions v4** - Serverless compute with Isolated Worker Model
- **Clean Architecture** - Separation of concerns across layers
- **Repository Pattern** - Generic and specialized repositories
- **SOLID Principles** - Maintainable and testable code
- **Result Pattern** - Structured error handling without exceptions

### Frontend Architecture
- **Component-Based** - Atomic design with reusable components
- **State Management** - React Query for server state, Zustand for client state
- **TypeScript Strict Mode** - Full type safety with Zod validation
- **Modern React Patterns** - Hooks, Context API, and concurrent features

---

## 🚀 Technology Stack

### Backend Technologies
- **.NET 9.0** - Latest .NET framework
- **Azure Functions v4** - Serverless compute platform
- **Entity Framework Core 8.0** - ORM with PostgreSQL provider
- **PostgreSQL 16** - Primary relational database
- **Redis 7** - Distributed caching layer
- **FluentValidation** - Input validation framework
- **Serilog** - Structured logging
- **xUnit** - Testing framework with FluentAssertions

### Frontend Technologies
- **React 19.1** - Modern UI framework with concurrent features
- **TypeScript 5.9** - Type-safe JavaScript development
- **Vite 7.1** - Lightning-fast build tool and dev server
- **Material-UI 7.3** - Comprehensive component library
- **React Query 5.90** - Server state management and caching
- **React Router 7.9** - Client-side routing
- **Zustand 5.0** - Lightweight state management
- **Zod 4.1** - Schema validation and type inference
- **i18next 25.5** - Internationalization framework

### DevOps & Infrastructure
- **Docker & Docker Compose** - Complete containerization
- **PostgreSQL** - Production-ready database
- **Redis** - High-performance caching
- **Swagger/OpenAPI** - Interactive API documentation
- **Jest & Vitest** - Comprehensive testing frameworks

---

## 📁 Project Structure

```
Product Management/
├── backend/                      # .NET Azure Functions API
│   ├── api/                      # Main API project
│   │   ├── Functions/            # Azure Function endpoints
│   │   ├── Services/             # Business logic layer
│   │   ├── Infrastructure/       # Data access & external services
│   │   ├── Entities/             # Domain models
│   │   ├── Dtos/                 # Data Transfer Objects
│   │   ├── Validators/           # FluentValidation validators
│   │   └── Migrations/           # EF Core migrations
│   └── tests/                    # Backend test suite
│
├── frontend/                     # React TypeScript SPA
│   ├── src/
│   │   ├── components/           # Reusable UI components
│   │   ├── pages/                # Route-based page components
│   │   ├── api/                  # API client and types
│   │   ├── hooks/                # Custom React hooks
│   │   ├── states/               # State management
│   │   ├── providers/            # Context providers
│   │   └── utils/                # Utility functions
│   └── public/                   # Static assets
│
├── .taskmaster/                  # Project documentation
│   └── docs/
│       └── product_management_prd.md
├── docker-compose.yml            # Multi-container setup
└── README.md                     # This file
```

---

## 🚀 Quick Start

### Prerequisites
- **Docker Desktop** - [Download here](https://www.docker.com/products/docker-desktop)
- **.NET 9.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Node.js 20+** - [Download here](https://nodejs.org/)
- **Azure Functions Core Tools v4** - [Installation guide](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local)

### Option 1: Docker Compose (Recommended)
```powershell
# Clone the repository
git clone <repository-url>
cd "Product Management"

# Start all services with Docker
docker-compose up --build

# The application will be available at:
# - Frontend: http://localhost:3000
# - Backend API: http://localhost:7071
# - Swagger UI: http://localhost:7071/api/swagger.json
```

### Option 2: Manual Setup

#### Backend Setup
```powershell
cd backend
dotnet restore
dotnet run --project api
```

#### Frontend Setup
```powershell
cd frontend
npm install
npm run dev
```

### Database Seeding
Once the backend is running, seed the database with sample data:
```bash
# Seed 100 sample products
curl -X POST http://localhost:7071/api/products/seed/100
```

---

## 🛠️ Development

### Backend Development
```powershell
cd backend

# Restore dependencies
dotnet restore

# Run the API locally
func start --port 7071

# Run tests
dotnet test

# Generate test coverage report
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage -reporttypes:Html
```

### Frontend Development
```powershell
cd frontend

# Install dependencies
npm install

# Start development server
npm run dev

# Run tests
npm test

# Run tests with coverage
npm run test:coverage

# Build for production
npm run build
```

### Database Management
```powershell
cd backend/api

# Create a new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Generate migration script
dotnet ef migrations script
```

---

## 📚 API Documentation

The API is documented using OpenAPI/Swagger and is available at:
- **Interactive Documentation**: http://localhost:7071/api/swagger.json
- **Health Check**: http://localhost:7071/api/health

### Key Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/products` | Get paginated products with search and filtering |
| `GET` | `/api/products/{id}` | Get a specific product by ID |
| `POST` | `/api/products` | Create a new product |
| `PUT` | `/api/products/{id}` | Update an existing product |
| `DELETE` | `/api/products/{id}` | Delete a product |
| `POST` | `/api/products/seed/{rows}` | Seed database with sample data |
| `GET` | `/api/health` | Health check endpoint |

---

## 🧪 Testing

### Backend Testing
The backend includes comprehensive testing with >90% coverage:
- **Unit Tests** - Business logic and service layer tests
- **Integration Tests** - Database and API endpoint tests
- **Repository Tests** - Data access layer tests

```powershell
# Run all backend tests
cd backend
dotnet test

# Generate HTML coverage report
./tests/generate-html-report.ps1
```

### Frontend Testing
The frontend includes 145+ tests with comprehensive coverage:
- **Component Tests** - React Testing Library tests
- **Hook Tests** - Custom hook testing
- **Integration Tests** - API integration tests
- **Accessibility Tests** - WCAG compliance tests

```bash
# Run all frontend tests
cd frontend
npm test

# Run tests with coverage
npm run test:coverage
```

---

## 🚀 Deployment

### Docker Production Deployment
```powershell
# Build production images
docker-compose -f docker-compose.prod.yml build

# Start production environment
docker-compose -f docker-compose.prod.yml up -d
```

### Azure Deployment
The backend is designed to deploy seamlessly to Azure Functions:
```powershell
cd backend/api

# Deploy to Azure
func azure functionapp publish <function-app-name>
```

### Static Site Deployment
The frontend builds to static files for deployment to any static hosting service:
```bash
cd frontend
npm run build

# Deploy the 'dist' folder to your hosting service
```

---

### Development Workflow
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

### Code Standards
- **Backend**: Follow C# conventions and use EditorConfig settings
- **Frontend**: Use ESLint and Prettier configurations
- **Tests**: Maintain >90% backend and >80% frontend test coverage
- **Documentation**: Update documentation for new features

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🔗 Related Documentation

- [Backend README](backend/README.md) - Detailed backend setup and API documentation
- [Frontend README](frontend/README.md) - Detailed frontend development guide
- [Product Requirements Document](/.taskmaster/docs/product_management_prd.md) - Complete project requirements and epics
- [API Documentation](http://localhost:7071/api/swagger.json) - Interactive API documentation
