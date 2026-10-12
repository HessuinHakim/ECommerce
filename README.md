# ECommerce API

A RESTful e-commerce backend built with **ASP.NET Core Web API (.NET 10)** and **Clean Architecture**. The project provides APIs for product and brand management, shopping carts, order processing, payment integration, promotions, and user identity management.

**Repository:** [HessuinHakim/ECommerce](https://github.com/HessuinHakim/ECommerce)

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Database Migrations](#database-migrations)
- [API Documentation](#api-documentation)
- [Concurrency Handling](#concurrency-handling)
- [Logging and Error Handling](#logging-and-error-handling)
- [Security Notes](#security-notes)
- [Future Improvements](#future-improvements)

## Overview

ECommerce API is a backend project designed to support core e-commerce operations through RESTful endpoints. It separates business logic, domain entities, infrastructure concerns, and API endpoints using Clean Architecture.

The project integrates user authentication and email confirmation, product and category management, shopping cart operations, order processing, Stripe payments, and promotional codes.

## Features

### Authentication and Identity

- User registration and login.
- Email confirmation.
- Forgot-password and password-reset workflows.
- Resend email confirmation.
- Identity-based user management.

### Product and Catalog Management

- Product management endpoints.
- Category management.
- Brand management.
- Product image upload.
- Paginated product queries.

### Shopping Cart and Orders

- Shopping cart operations.
- Order creation and checkout workflow.
- Order processing and status management.
- Administrative order management.

### Payments

- Stripe payment integration.
- Payment webhook handling.

> Configure Stripe credentials and webhook settings locally before testing payment workflows.

### Promotions

- Promotion-code functionality.
- Promotion management endpoints for administrators.
- Promotion activation and usage tracking.

### API Reliability

- Global exception handling middleware.
- Logging.
- Product concurrency handling using a concurrency token.
- Paginated API responses.

## Technology Stack

| Technology            | Purpose                        |
| --------------------- | ------------------------------ |
| C#                    | Primary programming language   |
| .NET 10               | Application platform           |
| ASP.NET Core Web API  | REST API development           |
| Entity Framework Core | Object-relational mapping      |
| SQL Server            | Relational database            |
| ASP.NET Core Identity | User identity management       |
| Stripe                | Payment integration            |
| Scalar                | Interactive API documentation  |
| Clean Architecture    | Separation of responsibilities |

## Architecture

The solution follows Clean Architecture principles to separate the domain, application logic, infrastructure implementations, and API presentation layer.

### Architecture Layers

**ECommerce.Domain**

- Contains core domain entities and business-related models.
- Designed to remain independent of infrastructure implementation details.

**ECommerce.Application**

- Contains application services, interfaces, request and response DTOs, and application-level logic.
- Defines abstractions used by the application.

**ECommerce.Infrastructure**

- Contains database access, Entity Framework Core configurations and migrations, repository implementations, identity-related infrastructure, and external service integrations.

**ECommerce.API**

- Exposes HTTP endpoints through controllers.
- Configures the application pipeline, middleware, and API documentation.

### Dependency Direction

The architecture aims to keep core business logic independent of external technologies. Infrastructure implementations provide the persistence and external integrations required by the application, while the API acts as the entry point.

## Project Structure

The solution is organized into the following projects:

```text
ECommerce/
├── ECommerce.API/
│   ├── Controllers/
│   ├── Middleware/
│   └── Program.cs
├── ECommerce.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
├── ECommerce.Domain/
│   └── Entities/
├── ECommerce.Infrastructure/
│   ├── Configuration/
│   ├── DataAccess/
│   ├── Identity/
│   ├── Migrations/
│   ├── Repositories/
│   └── Services/
└── README.md
```

*The tree highlights the main solution areas. Exact files and folders may vary with the current repository version.*

## Getting Started

### Prerequisites

Install the following tools:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server) or a compatible SQL Server instance
- [Git](https://git-scm.com/)
- An IDE such as [Visual Studio](https://visualstudio.microsoft.com/) or Visual Studio Code

A Stripe account and access to an email service may also be required to test the corresponding integrations.

### 1. Clone the Repository

```bash
git clone https://github.com/HessuinHakim/ECommerce.git
cd ECommerce
```

### 2. Open the Solution

Open the solution file in Visual Studio, or locate it from the repository root:

```bash
dir *.sln
```

If the solution file is in the repository root, restore dependencies and build the solution:

```bash
dotnet restore
dotnet build
```

If the solution file is located elsewhere, run these commands from its directory.

### 3. Configure the Application

Set the database connection string and the required integration settings as described in the Configuration section.

Do not commit credentials, API keys, or other sensitive configuration values.

### 4. Apply Database Migrations

After configuring a valid SQL Server connection, apply the existing Entity Framework Core migrations. See [Database Migrations](#database-migrations).

### 5. Run the API

From the directory containing `ECommerce.API.csproj`, run:

```bash
dotnet run
```

Use the URL printed in the console to access the running API. The actual address depends on the local launch configuration.

## Configuration

The application requires configuration appropriate to the enabled features.

### Database Connection

Configure the `DefaultConnection` connection string using the configuration key expected by the application.

For local development, use ASP.NET Core User Secrets or environment variables rather than storing real credentials in tracked JSON files.

Example User Secrets command:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<YOUR_SQL_SERVER_CONNECTION_STRING>"
```

Run this command from the API project directory, or specify the API project using the `--project` option.

### Stripe Configuration

Provide the Stripe settings expected by the payment service, including the appropriate API key and webhook signing secret.

The exact configuration key names should be confirmed from the application's Stripe service and configuration code.

Use Stripe test credentials for local testing. Never publish live secret keys or webhook signing secrets.

### Email Configuration

Configure the email provider and credentials required for registration confirmation and password-recovery emails.

The provider, configuration key names, and local setup steps depend on the implementation used by the project.

### Administrator Account

Administrator account creation and seeding depend on the project's Identity seeding implementation.

For a local development environment, use a dedicated test administrator account and configure its credentials securely. Do not publish administrator passwords in this repository.

## Database Migrations

The project uses Entity Framework Core migrations to manage database schema changes.

If the repository contains the `ECommerce.Infrastructure` migrations and the API is configured as the startup project, the following commands can be used from Visual Studio's Package Manager Console:

```powershell
Update-Database -Project ECommerce.Infrastructure -StartupProject ECommerce.API
```

To create a new migration after changing the data model:

```powershell
Add-Migration <MigrationName> -Project ECommerce.Infrastructure -StartupProject ECommerce.API
```

Make sure the required Entity Framework Core tooling and database provider packages are installed before running these commands.

## API Documentation

The project uses **Scalar** for interactive API documentation.

After starting the API, open the Scalar route configured in `Program.cs`.

**Scalar URL:** `<SCALAR_URL_TO_BE_CONFIRMED>`

The route is intentionally left as a placeholder because the exact path depends on the application's configuration.

Use the interactive documentation to inspect available endpoints and test supported requests.

## Concurrency Handling

The project includes a concurrency token for product updates.

Concurrency control helps detect conflicting changes when multiple operations attempt to update the same data. The exact behavior depends on how the token is configured and how concurrency exceptions are handled by the application.

## Logging and Error Handling

The project includes logging and global exception-handling middleware to support application diagnostics and consistent error handling.

Log output and retention depend on the application's logging providers and environment configuration.

Sensitive information, authentication credentials, payment data, and secrets should not be written to logs.

## Security Notes

- Never commit passwords, connection strings containing real credentials, Stripe secret keys, or webhook signing secrets.
- Keep local configuration and production secrets outside source control.
- Use separate test and production credentials.
- Do not expose administrator credentials in public documentation.
- Review authorization rules for administrative endpoints before deployment.
- Configure production logging and error responses so internal details are not exposed to API consumers.

## Future Improvements

Potential improvements, subject to the project's current implementation and testing status:

- Automated unit and integration tests.
- Containerized development and deployment.
- CI/CD pipeline integration.
- Deployment documentation and environment-specific configuration.

## License

No license has been specified yet. Add a `LICENSE` file if you intend to define the terms under which others may use, modify, or distribute this project.

---

**Developed by:** [Hussein Abdelhakim](https://github.com/HessuinHakim)

**Project Repository:** [ECommerce API](https://github.com/HessuinHakim/ECommerce)
