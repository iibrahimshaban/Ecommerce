# Ecommerce API

A layered .NET 9 ecommerce backend built with ASP.NET Core, Entity Framework Core, ASP.NET Core Identity, JWT authentication, and Hangfire background jobs. The solution includes the API layer, application services, domain entities, infrastructure/data access, and shared utilities.

## Overview

This project implements a complete ecommerce backend focused on:

- User authentication and authorization
- Product catalog management
- Category-based product browsing
- Cart operations for members
- Order checkout and cancellation flows
- Admin management for users and products
- Background notifications for newly published products
- Email-based account workflows such as registration confirmation and password reset

## Tech Stack

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core + SQL Server
- ASP.NET Core Identity
- JWT bearer authentication
- FluentValidation
- Mapster for mapping
- Serilog for structured logging
- Hangfire for scheduled/background jobs
- Swagger / OpenAPI for API documentation
- MailKit for email sending

## Solution Structure

```text
Ecommerce/
├── Ecommerce.Api/                  # API host and controllers
├── Ecommerce.Application/          # Application services, contracts, validators, mappings
├── Ecommerce.Core/                 # Domain entities and core business logic
├── Ecommerce.Infrastructure/       # EF Core, repositories, identity, unit of work, services
├── Ecommerce.Shared/               # Shared auth constants and reusable helpers
├── Ecommerce.sln                  # Solution file
├── README.md                      # Project documentation
└── .gitignore
```

## Main Architectural Layers

### 1. Ecommerce.Api

This project is the entry point for the application. It configures:

- dependency injection
- EF Core SQL Server connection
- Identity setup
- JWT authentication
- Swagger/OpenAPI
- Hangfire dashboard
- Serilog request logging
- controller routing

### 2. Ecommerce.Application

This layer contains the business logic and contracts used by the API:

- auth and user services
- product, cart, category, and order services
- request/response DTOs
- validation rules
- mapping configuration
- result wrappers and error definitions

### 3. Ecommerce.Core

This layer holds the domain entities and application constants such as:

- Product
- Category
- Cart / CartItem
- Order / OrderItem
- User-related and refresh-token data
- order status and cart status constants

### 4. Ecommerce.Infrastructure

This layer implements persistence and infrastructure concerns:

- ApplicationDbContext
- Identity setup and role seeding
- repositories
- unit of work pattern
- email sender integration
- Hangfire SQL Server storage

### 5. Ecommerce.Shared

Shared reusable components such as:

- authorization constants and default roles
- general helper logic
- common extensions

## Features

### Authentication and Authorization

The API uses ASP.NET Core Identity and JWT bearer tokens.

Built-in features include:

- login
- signup / registration
- refresh token flow
- revoke refresh token
- email confirmation
- resend confirmation email
- forgot password
- reset password
- role-based access control

Default roles:

- Admin
- Member

The JWT configuration is defined in appsettings.json and includes:

- Key
- Issuer
- Audience
- Expiry minutes

### Product Catalog

The product service supports:

- paginated product listing
- product search and filtering
- category and price filters
- status-based visibility control
- publishing and unpublishing products
- sending notification emails when new items are published

### Carts and Orders

Members can:

- view their cart
- add items
- update quantities
- remove items
- clear cart
- checkout orders
- view their order history
- cancel order requests

Admins can:

- retrieve order status options
- update order status

### User Management

Admins can manage users by:

- listing users
- fetching by ID or email
- creating users
- updating user information
- toggling account status
- unlocking accounts

Authenticated users can also:

- view their own profile
- update profile details
- change their password

## API Documentation and Monitoring

### Swagger / OpenAPI

The API is configured to expose OpenAPI metadata in development and to use Swagger UI. The UI is available through the standard Swagger endpoint for the project.

### Hangfire Dashboard

The application exposes the Hangfire dashboard at:

- /jobs

The dashboard credentials are defined in appsettings.json under HangFireSettings and are used for basic authentication.

## Database and Persistence

The project uses SQL Server with EF Core.

Connection strings in appsettings.json:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=...;Database=Ecommerce;...",
  "HangfireConnection": "Server=...;Database=EcommerceJobs;..."
}
```

The solution also includes EF Core migrations under the Infrastructure project, which means the database can be created and updated using standard .NET EF tooling.

## Configuration

The main configuration file is:

- Ecommerce.Api/appsettings.json

Important sections include:

- ConnectionStrings
- Serilog
- AllowedHosts
- Jwt
- MailSettings
- HangFireSettings

### Example JWT configuration

```json
"Jwt": {
  "Key": "...",
  "Issuer": "EcommercApp",
  "Audience": "EcommerceUsers",
  "ExpiryMinutes": 30
}
```

### Mail Configuration

The project integrates email sending for registration and notification workflows. Mail settings are configured through MailSettings in appsettings.json.

## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server instance
- Visual Studio 2022 or VS Code with C# support
- Git

### Clone the repository

```bash
git clone <your-repository-url>
cd Ecommerce
```

### Restore dependencies

```bash
dotnet restore
```

### Apply database migrations

```bash
dotnet ef database update --project Ecommerce.Infrastructure --startup-project Ecommerce.Api
```

If the dotnet-ef tool is not installed yet, use:

```bash
dotnet tool install --global dotnet-ef
```

### Run the API

```bash
dotnet run --project Ecommerce.Api
```

The application will start using the configured SQL Server and identity/JWT settings.

## Key Endpoints

### Authentication

| Method | Endpoint                        | Access | Description               |
| ------ | ------------------------------- | ------ | ------------------------- |
| POST   | /Auth/login                     | Public | User login                |
| POST   | /Auth/signup                    | Public | Register a new user       |
| POST   | /Auth/refresh                   | Public | Refresh JWT               |
| POST   | /Auth/revoke-refresh-token      | Public | Revoke token              |
| GET    | /Auth/confirm-email             | Public | Confirm email             |
| POST   | /Auth/resend-confirmation-email | Public | Resend confirmation email |
| POST   | /Auth/forget-password           | Public | Request password reset    |
| POST   | /Auth/reset-password            | Public | Reset password            |

### Products

| Method | Endpoint                         | Access        | Description               |
| ------ | -------------------------------- | ------------- | ------------------------- |
| GET    | /api/Products                    | Public/Member | Get paginated products    |
| GET    | /api/Products/{id}               | Member/Admin  | Get by ID                 |
| POST   | /api/Products                    | Admin         | Create product            |
| PUT    | /api/Products/{id}               | Admin         | Update product            |
| PUT    | /api/Products/{id}/toggle-status | Admin         | Publish/unpublish product |

### Categories

| Method | Endpoint             | Access       | Description        |
| ------ | -------------------- | ------------ | ------------------ |
| GET    | /api/Categories      | Admin        | Get all categories |
| GET    | /api/Categories/{id} | Member/Admin | Get a category     |
| POST   | /api/Categories      | Admin        | Create category    |
| PUT    | /api/Categories/{id} | Admin        | Update category    |

### Cart

| Method | Endpoint                  | Access | Description |
| ------ | ------------------------- | ------ | ----------- |
| GET    | /api/Carts                | Member | Get cart    |
| POST   | /api/Carts/items          | Member | Add item    |
| PUT    | /api/Carts/items/{itemId} | Member | Update item |
| DELETE | /api/Carts/items/{itemId} | Member | Remove item |
| DELETE | /api/Carts                | Member | Clear cart  |

### Orders

| Method | Endpoint                     | Access | Description               |
| ------ | ---------------------------- | ------ | ------------------------- |
| GET    | /api/Orders                  | Member | Get current user's orders |
| GET    | /api/Orders/{orderId}        | Member | Get order details         |
| POST   | /api/Orders/checkout         | Member | Checkout cart             |
| PUT    | /api/Orders/cancel/{orderId} | Member | Cancel order              |
| GET    | /api/Orders/statuses         | Admin  | Get order status values   |
| PUT    | /api/Orders/{id}             | Admin  | Update order status       |

### Users

| Method | Endpoint                          | Access | Description        |
| ------ | --------------------------------- | ------ | ------------------ |
| GET    | /api/Users                        | Admin  | List users         |
| GET    | /api/Users/{id}                   | Admin  | Get user by ID     |
| GET    | /api/Users/email/{email}          | Admin  | Get user by email  |
| POST   | /api/Users                        | Admin  | Create user        |
| PUT    | /api/Users/{UserId}               | Admin  | Update user        |
| PUT    | /api/Users/{UserId}/toggle-status | Admin  | Toggle user status |
| PUT    | /api/Users/{UserId}/unlock        | Admin  | Unlock user        |

### Account

| Method | Endpoint                      | Access             | Description     |
| ------ | ----------------------------- | ------------------ | --------------- |
| GET    | /api/Accounts                 | Authenticated user | Get profile     |
| PUT    | /api/Accounts/info            | Authenticated user | Update profile  |
| PUT    | /api/Accounts/change-password | Authenticated user | Change password |

## Example Login Request

```http
POST /Auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "YourPassword123"
}
```

Example response:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 1800,
  "refreshToken": "abc123...",
  "refreshTokenExpirationDate": "2026-10-17T12:00:00Z"
}
```

## Background Jobs

This project uses Hangfire with SQL Server storage to support scheduled processing. One scheduled job is configured for new product notifications:

- daily email notifications for newly published products

The job scheduler is configured in the startup pipeline and runs as part of the application lifecycle.

## Notes and Considerations

- The project uses SQL Server and a separate Hangfire database connection string.
- Authentication is role-based and uses JWT claims.
- The codebase uses a result-based response pattern across services and controllers, which keeps controller actions consistent and expressive.
- The API uses FluentValidation for request validation before service execution.
- The repository currently appears to be focused on the backend API and infrastructure without a dedicated frontend application.

## Future Improvements

Potential additions to expand this project:

- unit and integration tests
- frontend application integration
- payment gateway integration
- inventory and warehouse management
- analytics dashboard
- caching and performance tuning
- Docker support

## License

This project does not currently declare a license in the repository snapshot. Add a license file if you want to publish the project publicly on GitHub.

## Contributing

Contributions are welcome. A typical workflow would be:

1. Create a feature branch
2. Implement the change
3. Run validation and tests
4. Open a pull request with a clear summary

---

This README was generated based on the current repository structure and implementation details in the solution.
