# Contacts Manager - Enterprise Contacts Management System

[![.NET](https://img.shields.io/badge/.NET-7.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-7.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-6.0-512BD4?logo=dotnet)](https://docs.microsoft.com/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A comprehensive, enterprise-grade contacts management system built with **Clean Architecture** principles, featuring robust authentication, role-based access control, advanced search capabilities, and multi-format data export functionality.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [Testing](#testing)
- [API Documentation](#api-documentation)
- [Security](#security)
- [Logging](#logging)
- [Deployment](#deployment)
- [Contributing](#contributing)
- [License](#license)

## 🎯 Overview

Contacts Manager is a full-featured enterprise contacts management application designed to help organizations efficiently manage their contact database. The system provides a secure, scalable, and maintainable solution for storing, searching, and managing contact information with comprehensive CRUD operations, advanced filtering, and data export capabilities.

### Key Highlights

- **Clean Architecture**: Separation of concerns with clear layer boundaries
- **Security First**: ASP.NET Core Identity with role-based authorization
- **Comprehensive Testing**: Unit, integration, and controller tests with high coverage
- **Production Ready**: Structured logging, exception handling, and error management
- **Data Export**: Support for CSV, Excel, and PDF export formats
- **Modern UI**: Responsive ASP.NET Core MVC views with intuitive user experience

## ✨ Features

### Core Functionality

- **Contact Management**
  - Create, read, update, and delete contacts
  - Advanced search and filtering by multiple criteria
  - Multi-column sorting with ascending/descending order
  - Bulk operations support

- **Country Management**
  - Country reference data management
  - Excel-based country import functionality
  - Country-based contact filtering

- **User Authentication & Authorization**
  - Secure user registration and login
  - Role-based access control (Admin/User roles)
  - Session management with persistent login
  - Protected routes and actions

- **Data Export**
  - **CSV Export**: Export contacts to comma-separated values format
  - **Excel Export**: Generate formatted Excel workbooks with EPPlus
  - **PDF Export**: Create professional PDF reports with Rotativa

- **Search & Filter**
  - Real-time search across multiple fields (Name, Email, Date of Birth, Gender, Country, Address)
  - Dynamic filtering with case-insensitive matching
  - Combined search and sort operations

### Technical Features

- **Structured Logging**: Serilog integration with multiple sinks (Console, File, SQL Server, Seq)
- **Exception Handling**: Global exception handling middleware
- **Action Filters**: Request/response logging and validation
- **Anti-Forgery Protection**: CSRF token validation on state-changing operations
- **HTTP Logging**: Comprehensive request/response logging
- **Database Migrations**: Automatic migration application on startup
- **Dependency Injection**: Full DI container configuration

## 🏗️ Architecture

The application follows **Clean Architecture** principles, ensuring separation of concerns and maintainability:

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│              (ContactsManager.Web)                       │
│  Controllers, Views, Filters, Middleware, Startup       │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│                    Application Layer                     │
│              (ContactsManager.Core)                      │
│  Services, DTOs, Service Contracts, Domain Entities     │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│                  Infrastructure Layer                    │
│          (ContactsManager.Infrastructure)              │
│  DbContext, Repositories, Migrations                    │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│                      Data Layer                          │
│                  SQL Server Database                     │
└─────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

- **Web Layer**: Handles HTTP requests, MVC views, authentication, and presentation logic
- **Core Layer**: Contains business logic, domain entities, DTOs, and service contracts
- **Infrastructure Layer**: Manages data access, Entity Framework configuration, and repository implementations

## 🛠️ Technology Stack

### Core Technologies

- **.NET 7.0**: Modern, high-performance framework
- **ASP.NET Core MVC**: Web application framework
- **Entity Framework Core 6.0.9**: ORM for database operations
- **SQL Server**: Relational database management system
- **ASP.NET Core Identity**: Authentication and authorization framework

### Key Libraries & Packages

- **Serilog 2.12.0**: Structured logging framework
  - Serilog.Sinks.Console
  - Serilog.Sinks.File
  - Serilog.Sinks.MSSqlServer
  - Serilog.Sinks.Seq
- **EPPlus 6.0.8**: Excel file generation
- **CsvHelper 28.0.1**: CSV file generation
- **Rotativa.AspNetCore 1.2.0**: PDF generation from views
- **AutoFixture 4.17.0**: Test data generation
- **FluentAssertions 6.7.0**: Fluent test assertions
- **Moq 4.18.2**: Mocking framework for unit tests
- **xUnit 2.4.2**: Testing framework

## 📦 Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) or later
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (2019 or later) or [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended) or [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/downloads) for version control

### Optional Tools

- [Seq](https://datalust.co/seq) - For centralized log viewing (optional, for development)
- [IIS Express](https://www.iis.net/downloads/microsoft/iis-express) - Included with Visual Studio

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/ContactsManager-App.git
cd ContactsManager-App
```

### 2. Configure Database Connection

Open `ContactsManagerCleanArchitectureSolution/ContactsManager.Web/appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost\\SQLEXPRESS;Database=ContactsDatabase;Trusted_Connection=True;"
  }
}
```

**Note**: Adjust the server name and database name according to your SQL Server configuration.

### 3. Restore Dependencies

```bash
cd ContactsManagerCleanArchitectureSolution
dotnet restore
```

### 4. Run Database Migrations

The application automatically applies migrations on startup. Alternatively, you can run migrations manually:

```bash
cd ContactsManager.Web
dotnet ef database update --project ../ContactsManager.Infrastructure
```

### 5. Run the Application

#### Using .NET CLI

```bash
cd ContactsManagerCleanArchitectureSolution/ContactsManager.Web
dotnet run
```

#### Using Visual Studio

1. Open `ContactsManagerCleanArchitectureSolution.sln`
2. Set `ContactsManager.Web` as the startup project
3. Press `F5` or click "Run"

The application will be available at:
- **HTTPS**: `https://localhost:5286`
- **HTTP**: `http://localhost:5286`

### 6. Initial Setup

1. Navigate to the application URL
2. Click "Register" to create your first user account
3. Choose either "Admin" or "User" role during registration
4. Log in with your credentials

## 📁 Project Structure

```
ContactsManagerCleanArchitectureSolution/
│
├── ContactsManager.Web/                    # Presentation Layer
│   ├── Controllers/                         # MVC Controllers
│   │   ├── AccountController.cs            # Authentication
│   │   ├── PersonsController.cs            # Contact CRUD operations
│   │   ├── CountriesController.cs          # Country management
│   │   └── HomeController.cs               # Home page
│   ├── Views/                              # Razor views
│   ├── Filters/                            # Action filters
│   ├── Middleware/                         # Custom middleware
│   ├── StartupExtensions/                  # Service configuration
│   └── Program.cs                          # Application entry point
│
├── ContactsManager.Core/                    # Application Layer
│   ├── Domain/                             # Domain entities
│   │   ├── Entities/                       # Person, Country
│   │   ├── IdentityEntities/               # ApplicationUser, ApplicationRole
│   │   └── RepositoryContracts/            # Repository interfaces
│   ├── DTO/                                # Data Transfer Objects
│   ├── Enums/                              # Enumerations
│   ├── Exceptions/                         # Custom exceptions
│   ├── Helpers/                            # Helper classes
│   ├── ServiceContracts/                  # Service interfaces
│   └── Services/                           # Business logic implementation
│
├── ContactsManager.Infrastructure/          # Infrastructure Layer
│   ├── DbContext/                          # Entity Framework context
│   ├── Migrations/                         # Database migrations
│   └── Repositories/                      # Repository implementations
│
├── ContactsManager.ServiceTests/            # Service layer unit tests
├── ContactsManager.ControllerTests/        # Controller unit tests
└── ContactsManager.IntegrationTests/      # Integration tests
```

## ⚙️ Configuration

### Application Settings

Key configuration options in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Your connection string here"
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { 
        "Name": "File",
        "Args": {
          "path": "logs/log.txt",
          "rollingInterval": "Hour"
        }
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://localhost:5341/"
        }
      }
    ]
  }
}
```

### Environment-Specific Settings

- **Development**: `appsettings.Development.json`
- **Production**: Configure via environment variables or Azure App Settings

### Identity Configuration

Password requirements are configured in `ConfigureServiceExtension.cs`:

- Minimum length: 5 characters
- Require lowercase: Yes
- Require uppercase: No
- Require digit: No
- Require non-alphanumeric: No
- Required unique chars: 2

**Note**: Adjust these settings according to your security requirements.

## 🧪 Testing

The project includes comprehensive test coverage across multiple layers:

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test ContactsManagerCleanArchitectureSolution/ContactsManager.ServiceTests

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Test Projects

1. **ContactsManager.ServiceTests**
   - Unit tests for service layer
   - Uses Moq for repository mocking
   - AutoFixture for test data generation
   - FluentAssertions for readable assertions

2. **ContactsManager.ControllerTests**
   - Controller action testing
   - View result validation
   - Model binding verification

3. **ContactsManager.IntegrationTests**
   - End-to-end integration tests
   - Uses in-memory database
   - Custom WebApplicationFactory for test hosting

### Test Coverage

The test suite covers:
- ✅ CRUD operations
- ✅ Search and filtering
- ✅ Sorting functionality
- ✅ Validation logic
- ✅ Exception handling
- ✅ Edge cases and error scenarios

## 📚 API Documentation

### Contact Management Endpoints

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/persons/index` | List all contacts with search/sort | Authenticated |
| GET | `/persons/create` | Display create contact form | Authenticated |
| POST | `/persons/create` | Create new contact | Authenticated |
| GET | `/persons/edit/{id}` | Display edit contact form | Authenticated |
| POST | `/persons/edit/{id}` | Update contact | Authenticated |
| GET | `/persons/delete/{id}` | Display delete confirmation | Authenticated |
| POST | `/persons/delete/{id}` | Delete contact | Authenticated |
| GET | `/persons/PersonsPDF` | Export contacts as PDF | Authenticated |
| GET | `/persons/PersonsCSV` | Export contacts as CSV | Authenticated |
| GET | `/persons/PersonsExcel` | Export contacts as Excel | Authenticated |

### Authentication Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/Account/Register` | Display registration form |
| POST | `/Account/Register` | Register new user |
| GET | `/Account/Login` | Display login form |
| POST | `/Account/Login` | Authenticate user |
| POST | `/Account/Logout` | Sign out user |
| GET | `/Account/IsEmailAlreadyRegistered` | Check email availability |

### Country Management Endpoints

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/countries/UploadFromExcel` | Display upload form | Authenticated |
| POST | `/countries/UploadFromExcel` | Import countries from Excel | Authenticated |

## 🔒 Security

### Authentication & Authorization

- **ASP.NET Core Identity**: Secure user management
- **Role-Based Access Control**: Admin and User roles
- **Cookie Authentication**: Persistent login sessions
- **Password Hashing**: Secure password storage using Identity's password hasher

### Security Features

- **Anti-Forgery Tokens**: CSRF protection on state-changing operations
- **HTTPS Enforcement**: HSTS headers in production
- **Input Validation**: Model validation on all user inputs
- **SQL Injection Protection**: Parameterized queries via Entity Framework
- **XSS Protection**: Razor view engine automatic encoding

### Security Best Practices

- All routes require authentication by default (fallback policy)
- Sensitive operations protected with authorization policies
- Exception details hidden in production environment
- Connection strings stored securely (use User Secrets in development)

## 📊 Logging

The application uses **Serilog** for structured logging with multiple output sinks:

### Log Sinks

1. **Console**: Real-time log output during development
2. **File**: Rolling file logs in `logs/log.txt`
3. **SQL Server**: Structured logs in database (configurable)
4. **Seq**: Centralized log aggregation (optional, for development)

### Log Levels

- **Information**: General application flow
- **Warning**: Potential issues
- **Error**: Exceptions and errors
- **Debug**: Detailed diagnostic information

### Logging Configuration

Logs are configured in `appsettings.json` and can be adjusted per environment. The application logs:

- HTTP requests and responses
- Service method invocations
- Database operations
- Exceptions and errors
- Performance metrics (via SerilogTimings)

## 🚢 Deployment

### Prerequisites for Production

1. **SQL Server Database**: Set up production database
2. **Connection String**: Configure secure connection string
3. **HTTPS Certificate**: SSL/TLS certificate for HTTPS
4. **Environment Variables**: Set production environment variables

### Deployment Options

#### IIS Deployment

1. Publish the application:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. Configure IIS:
   - Create application pool targeting .NET 7.0
   - Create website pointing to publish folder
   - Configure connection strings in `web.config` or environment variables

#### Azure App Service

1. Create Azure App Service
2. Configure connection string in App Settings
3. Deploy via Visual Studio or Azure DevOps

#### Docker (Future Enhancement)

Docker support can be added for containerized deployments.

## 🤝 Contributing

Contributions are welcome! Please follow these guidelines:

1. **Fork the repository**
2. **Create a feature branch** (`git checkout -b feature/AmazingFeature`)
3. **Commit your changes** (`git commit -m 'Add some AmazingFeature'`)
4. **Push to the branch** (`git push origin feature/AmazingFeature`)
5. **Open a Pull Request**

### Development Guidelines

- Follow Clean Architecture principles
- Write unit tests for new features
- Update documentation as needed
- Follow C# coding conventions
- Ensure all tests pass before submitting PR

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
- Uses [Serilog](https://serilog.net/) for structured logging
- PDF generation powered by [Rotativa](https://github.com/webgio/Rotativa.AspNetCore)
- Excel generation with [EPPlus](https://github.com/EPPlusSoftware/EPPlus)

## 📞 Support

For issues, questions, or contributions, please open an issue on the GitHub repository.

---

**Built with ❤️ using .NET 7.0 and Clean Architecture**

