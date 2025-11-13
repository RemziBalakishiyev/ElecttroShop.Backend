# 🛒 ElectroShop API

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-316192?logo=postgresql)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Modern, scalable e-commerce API built with ASP.NET Core 8.0, following Clean Architecture and Domain-Driven Design principles.

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Technologies](#-technologies)
- [Getting Started](#-getting-started)
- [API Documentation](#-api-documentation)
- [Database Setup](#-database-setup)
- [Authentication](#-authentication)
- [Project Structure](#-project-structure)
- [Contributing](#-contributing)
- [License](#-license)

## ✨ Features

- ✅ **CQRS Pattern** - Commands and Queries separation using MediatR
- ✅ **Result Pattern** - Consistent error handling without exceptions
- ✅ **Validation** - FluentValidation for request validation
- ✅ **Pagination** - Efficient pagination for all list endpoints
- ✅ **JWT Authentication** - Secure authentication with access and refresh tokens
- ✅ **Soft Delete** - Soft delete pattern for data retention
- ✅ **Global Exception Handling** - Centralized error handling middleware
- ✅ **Structured Logging** - Serilog for application logging
- ✅ **Docker Support** - Ready-to-use Docker Compose configuration
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **DDD & SOLID** - Domain-Driven Design and SOLID principles

## 🏗️ Architecture

This project follows **Clean Architecture** and **Domain-Driven Design (DDD)** principles:

```
┌─────────────────────────────────────────────────────────┐
│                   ElectroShop.WebApi                    │
│              (API Layer - Controllers)                  │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│              ElectroShop.Application                    │
│    (Business Logic - CQRS, DTOs, Validators)           │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                 ElectroShop.Domain                      │
│        (Domain Entities, Value Objects, Events)         │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│              ElectroShop.Persistence                    │
│      (Data Access - EF Core, Repositories)              │
└─────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

- **ElectroShop.Domain** - Core business logic, entities, value objects, domain events
- **ElectroShop.Application** - Use cases, CQRS handlers, DTOs, validations
- **ElectroShop.Persistence** - Data access, EF Core configurations, repositories
- **ElectroShop.WebApi** - Controllers, middleware, API configuration

## 🛠️ Technologies

- **.NET 8.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core 8.0** - ORM for database operations
- **PostgreSQL 16** - Relational database
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Validation library
- **Mapster** - High-performance object mapping
- **Serilog** - Structured logging
- **JWT** - Authentication tokens
- **Docker & Docker Compose** - Containerization

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 16+](https://www.postgresql.org/download/) (or Docker)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (optional)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/ElectroShop-API.git
cd ElectroShop-API
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Configure database connection**

Update `appsettings.json` or `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ElectroShopDb;Username=postgres;Password=postgres"
  }
}
```

### Database Setup

#### Option 1: Using Docker (Recommended)

```bash
docker-compose up -d
```

This will start PostgreSQL on `localhost:5432`.

#### Option 2: Local PostgreSQL

Ensure PostgreSQL is running and create a database named `ElectroShopDb`.

### Running Migrations

#### Using Package Manager Console (Visual Studio)

1. Open Package Manager Console in Visual Studio
2. Run the following commands:

```powershell
# Create initial migration
Add-Migration InitialMigration -Project ElectroShop.Persistence -StartupProject ElectroShop.WebApi

# Apply migrations to database
Update-Database -Project ElectroShop.Persistence -StartupProject ElectroShop.WebApi
```

#### Using Command Line

```bash
# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Create migration
dotnet ef migrations add InitialMigration --project src/ElectroShop.Persistence --startup-project src/ElectroShop.WebApi

# Update database
dotnet ef database update --project src/ElectroShop.Persistence --startup-project src/ElectroShop.WebApi
```

### Running the Application

```bash
dotnet run --project src/ElectroShop.WebApi
```

The API will be available at:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `https://localhost:5001/swagger`

## 📚 API Documentation

### Swagger UI

Once the application is running, access the interactive Swagger documentation at:

```
https://localhost:5001/swagger
```

### Main Endpoints

#### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh-token` - Refresh access token

#### Products
- `GET /api/products` - Get paginated products list
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/search` - Search products
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product (soft delete)
- `PATCH /api/products/{id}/price` - Change product price
- `PATCH /api/products/{id}/stock` - Change product stock

#### Categories
- `GET /api/categories` - Get paginated categories list
- `GET /api/categories/{id}` - Get category by ID
- `GET /api/categories/slug/{slug}` - Get category by slug
- `POST /api/categories` - Create new category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

#### Brands
- `GET /api/brands` - Get paginated brands list
- `GET /api/brands/{id}` - Get brand by ID
- `POST /api/brands` - Create new brand
- `PUT /api/brands/{id}` - Update brand
- `DELETE /api/brands/{id}` - Delete brand

#### Customers
- `GET /api/customers/{id}` - Get customer by ID
- `GET /api/customers/email/{email}` - Get customer by email
- `POST /api/customers/register` - Register new customer
- `PUT /api/customers/{id}` - Update customer

#### Orders
- `GET /api/orders/{id}` - Get order by ID
- `GET /api/orders/customer/{customerId}` - Get orders by customer
- `POST /api/orders` - Create new order
- `POST /api/orders/{orderId}/items` - Add item to order
- `DELETE /api/orders/{orderId}/items/{productId}` - Remove item from order
- `PATCH /api/orders/{id}/mark-paid` - Mark order as paid

## 🔐 Authentication

### Default Users (Seed Data)

The application includes seed data with the following default users:

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@electroshop.az` | `Admin123!` |
| Agent | `agent1@electroshop.az` | `Agent123!` |
| Agent | `agent2@electroshop.az` | `Agent123!` |

### Authentication Flow

1. **Login** - Send credentials to `/api/auth/login`
2. **Receive Tokens** - Get access token and refresh token
3. **Use Access Token** - Include `Authorization: Bearer {token}` header in requests
4. **Refresh Token** - When access token expires, use refresh token at `/api/auth/refresh-token`

### Example Request

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@electroshop.az",
  "password": "Admin123!"
}
```

Response:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_here...",
  "user": {
    "id": "...",
    "email": "admin@electroshop.az",
    "fullName": "Administrator",
    "role": "Admin"
  }
}
```

## 📁 Project Structure

```
ElectronicNumberOne/
├── src/
│   ├── ElectroShop.Domain/              # Domain Layer
│   │   ├── Entities/                    # Domain entities
│   │   ├── ValueObjects/                # Value objects (Money, Sku)
│   │   ├── Enums/                       # Enumerations
│   │   ├── Events/                      # Domain events
│   │   └── Primitives/                  # Base classes
│   │
│   ├── ElectroShop.Application/         # Application Layer
│   │   ├── Features/                    # CQRS features
│   │   │   ├── Products/
│   │   │   ├── Categories/
│   │   │   ├── Brands/
│   │   │   ├── Customers/
│   │   │   ├── Orders/
│   │   │   └── Auth/
│   │   ├── DTOs/                        # Data Transfer Objects
│   │   ├── Mappings/                    # Mapster configurations
│   │   ├── Behaviours/                  # MediatR pipeline behaviours
│   │   ├── Common/                      # Common utilities
│   │   │   ├── Results/                 # Result pattern
│   │   │   └── Options/                 # Configuration options
│   │   └── Services/                    # Application services
│   │
│   ├── ElectroShop.Persistence/         # Persistence Layer
│   │   ├── Contexts/                    # DbContext
│   │   ├── Configurations/              # EF Core configurations
│   │   ├── Repositories/                # Repository implementations
│   │   ├── Seeders/                     # Database seeders
│   │   └── Helpers/                     # Helper classes
│   │
│   └── ElectroShop.WebApi/              # API Layer
│       ├── Controllers/                 # API controllers
│       ├── Extensions/                  # Extension methods
│       ├── Middleware/                  # Custom middleware
│       └── Program.cs                   # Entry point
│
├── scripts/                             # Helper scripts
│   ├── create-migration.ps1             # PowerShell migration script
│   ├── create-migration.sh              # Bash migration script
│   ├── update-database.ps1              # PowerShell update script
│   └── update-database.sh               # Bash update script
│
├── docker-compose.yml                   # Docker Compose configuration
├── PMC-Commands.txt                     # Package Manager Console commands
└── README.md                            # This file
```

## 🌱 Seed Data

When the application starts for the first time, it automatically seeds the database with:

- **3 Users** (1 Admin, 2 Agents)
- **7 Categories** (with hierarchical structure)
- **10 Brands** (Apple, Samsung, Lenovo, etc.)
- **5 Sample Products** (iPhone, Samsung Galaxy, MacBook, etc.)

## 🔧 Configuration

### JWT Settings

Configure JWT in `appsettings.json`:

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGenerationMustBeAtLeast32CharactersLong!",
    "Issuer": "ElectroShop",
    "Audience": "ElectroShop",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 30
  }
}
```

### Serilog Configuration

Logging is configured in `appsettings.json`:

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

## 🐳 Docker

### Using Docker Compose

Start PostgreSQL:

```bash
docker-compose up -d
```

Stop PostgreSQL:

```bash
docker-compose down
```

### Dockerfile

The project includes a multi-stage Dockerfile for production deployments.

## 📝 Development

### Creating New Migrations

```powershell
Add-Migration MigrationName -Project ElectroShop.Persistence -StartupProject ElectroShop.WebApi
```

### Updating Database

```powershell
Update-Database -Project ElectroShop.Persistence -StartupProject ElectroShop.WebApi
```

### Running Tests

```bash
dotnet test
```

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👤 Author

**Your Name**
- GitHub: [@yourusername](https://github.com/yourusername)
- Email: your.email@example.com

## 🙏 Acknowledgments

- Clean Architecture principles by Robert C. Martin
- Domain-Driven Design by Eric Evans
- MediatR library for CQRS pattern
- ASP.NET Core team for the excellent framework

---

⭐ If you found this project helpful, please consider giving it a star!
