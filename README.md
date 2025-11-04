# MockTest E-Commerce API

ASP.NET Core e-commerce API with JWT authentication, featuring products, categories, shopping cart, and order management.

## 🚀 Quick Start

### Prerequisites

- .NET 9.0 SDK
- SQL Server or SQL Server Express
- Visual Studio 2022 / VS Code / Rider (optional)

### Running the API

```powershell
# Clone the repository (if applicable)
# Navigate to project directory
cd MockTest

# Update database connection string in API/appsettings.json if needed

# Apply database migrations
dotnet ef database update --project Infrastructure --startup-project API

# Run the API
cd API
dotnet run
```

The API will be available at: `http://localhost:5296/api`

### Test Accounts

The application comes with pre-seeded accounts:

**Admin:**

- Email: `admin@example.com`
- Password: `P@ssw0rd`
- Roles: Admin, User

**Regular User:**

- Email: `user@example.com`
- Password: `P@ssw0rd`
- Roles: User

---

## 📚 Documentation for Frontend Developers

We've created comprehensive documentation to help frontend developers integrate with this API:

### 🌟 [Start Here: Frontend Guide](./FRONTEND_GUIDE.md)

Quick start guide with code examples, common patterns, and troubleshooting.

### 📖 [Complete API Documentation](./API_DOCUMENTATION.md)

Full reference with all endpoints, request/response examples, and authentication details.

### ⚡ [Quick Reference Guide](./API_QUICK_REFERENCE.md)

Handy lookup table for endpoints and common workflows.

### 📦 [Postman Collection](./MockTest_API.postman_collection.json)

Import into Postman or Thunder Client for easy testing.

### 📋 [Documentation Index](./README_API_DOCS.md)

Navigation guide to all documentation files.

---

## 🏗️ Architecture

This project follows a layered architecture pattern:

```
API/                    # Web layer (controllers, middleware, startup)
Application/            # Application layer (services, DTOs, interfaces)
Core/                   # Domain layer (entities, exceptions, repository interfaces)
Infrastructure/         # Infrastructure layer (EF Core, repositories, migrations)
Shared/                 # Shared models (pagination, etc.)
```

### Key Technologies

- **Framework:** ASP.NET Core 9.0
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Core Identity + JWT
- **API Documentation:** Swagger/OpenAPI
- **Database:** SQL Server

---

## 🔑 Features

### Authentication & Authorization

- JWT-based authentication
- Role-based authorization (User, Admin)
- 30-minute token lifetime
- Secure password hashing with Identity

### Products

- CRUD operations
- Pagination and search
- Automatic default image on creation
- Main image in list view, all images in detail view
- Soft delete support

### Categories

- CRUD operations (admin-protected)
- Soft delete support

### Shopping Cart

- One cart per user
- Add, update, remove items
- Automatic cart creation
- Cart cleared after order creation

### Orders

- Create order from cart
- Order history with pagination
- Order status management (admin)
- Payment method selection
- Order status workflow (Pending → Processing → Shipped → Completed)

### Users

- User registration
- User management
- Role assignment

---

## 🛠️ Development

### Project Structure

```
API.sln
├── API/                           # Web API project
│   ├── Controllers/               # API controllers
│   ├── Middlewares/               # Custom middleware (exception handling)
│   ├── Program.cs                 # Application startup & configuration
│   └── appsettings.json           # Configuration (connection strings, JWT)
├── Application/                   # Application layer
│   ├── DTOs/                      # Data transfer objects
│   ├── IServices/                 # Service interfaces
│   └── Services/                  # Service implementations
├── Core/                          # Domain layer
│   ├── Entities/                  # Domain entities
│   ├── Exceptions/                # Domain exceptions
│   └── IRepositories/             # Repository interfaces
├── Infrastructure/                # Infrastructure layer
│   ├── ApplicationDbContext.cs   # EF Core DbContext
│   ├── Migrations/                # EF Core migrations
│   └── Repositories/              # Repository implementations
└── Shared/                        # Shared models
    └── Models/                    # Common models (pagination)
```

### Building the Solution

```powershell
# Build entire solution
dotnet build API.sln

# Build specific project
dotnet build API/API.csproj

# Run tests (if available)
dotnet test
```

### Database Migrations

```powershell
# Add a new migration
dotnet ef migrations add MigrationName --project Infrastructure --startup-project API

# Apply migrations to database
dotnet ef database update --project Infrastructure --startup-project API

# Rollback to specific migration
dotnet ef database update PreviousMigrationName --project Infrastructure --startup-project API
```

### Running with Hot Reload

```powershell
cd API
dotnet watch run
```

---

## 🔒 Configuration

### Connection String

Update `API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=MockTest;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### JWT Settings

⚠️ **Important:** Change the JWT key in production!

```json
{
  "Jwt": {
    "Key": "ChangeThisDevelopmentKeyToSomethingSecret",
    "Issuer": "MockTestApi",
    "Audience": "MockTestClient",
    "ExpiresMinutes": 30
  }
}
```

---

## 🧪 Testing the API

### Using Swagger UI

When the API is running, navigate to:

```
http://localhost:5296/swagger
```

### Using Postman

1. Import `MockTest_API.postman_collection.json`
2. Run "Auth > Login" to get a token
3. The token is automatically saved for authenticated requests

### Using cURL

```bash
# Login
curl -X POST http://localhost:5296/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"P@ssw0rd"}'

# Get products
curl http://localhost:5296/api/product?pageNumber=1&pageSize=10

# Get cart (with token)
curl http://localhost:5296/api/cart \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## 📋 API Endpoints Overview

### Public Endpoints

- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and get JWT token
- `GET /api/product` - Get all products (paginated)
- `GET /api/product/{id}` - Get product details
- `GET /api/category` - Get all categories
- `GET /api/category/{id}` - Get category details

### Authenticated Endpoints (User)

- `GET /api/cart` - Get user's cart
- `POST /api/cart/items` - Add item to cart
- `PUT /api/cart/items/{id}` - Update cart item
- `DELETE /api/cart/items/{id}` - Remove cart item
- `DELETE /api/cart/clear` - Clear cart
- `POST /api/order` - Create order from cart
- `GET /api/order` - Get user's orders
- `GET /api/order/{id}` - Get order details

### Admin-Only Endpoints

- `POST /api/category` - Create category
- `PUT /api/category/{id}` - Update category
- `DELETE /api/category/{id}` - Delete category
- `PATCH /api/order/{id}/status` - Update order status
- `GET /api/order/all` - Get all orders

For complete endpoint documentation with examples, see [API_DOCUMENTATION.md](./API_DOCUMENTATION.md).

---

## 🎯 Key Patterns & Conventions

### Soft Deletes

- Products and Categories use soft delete (`IsDeleted` flag)
- Global query filters automatically exclude deleted items
- Repositories implement `DeleteAsync` by setting `IsDeleted = true`

### Exception Handling

- Services throw domain exceptions (`NotFoundException`, `BadRequestException`, etc.)
- `ExceptionHandlingMiddleware` catches and maps to HTTP responses
- Controllers don't handle exceptions (let middleware handle them)

### DTO Naming Convention

- `Create{Entity}DTO` - Input for creation
- `{Entity}ListDTO` - List item representation
- `{Entity}DetailDTO` - Detailed representation
- `Update{Entity}DTO` - Input for updates

### Service Pattern

- Controllers call services
- Services contain business logic
- Services call repositories
- Services map entities to DTOs

### Repository Pattern

- Repositories handle data access
- All methods are async
- Implement `IRepository` interfaces from Core

---

## 🔧 Common Developer Workflows

### Adding a New Feature

1. **Define entities** in `Core/Entities/`
2. **Create DTOs** in `Application/DTOs/{Feature}/`
3. **Define repository interface** in `Core/IRepositories/`
4. **Implement repository** in `Infrastructure/Repositories/`
5. **Define service interface** in `Application/IServices/`
6. **Implement service** in `Application/Services/`
7. **Create controller** in `API/Controllers/`
8. **Register DI** in `API/Program.cs`
9. **Create migration** and update database
10. **Test** with Postman/Swagger

### Debugging

```powershell
# Run with detailed logging
cd API
dotnet run --environment Development

# Check logs for errors
# Use VS Code debugger or Visual Studio debugger
```

---

## 📝 Important Notes

### Security Considerations

- JWT key in `appsettings.json` is for development only
- Change JWT key in production to a secure, random value
- Use HTTPS in production
- Consider implementing refresh tokens for better UX
- Validate all inputs before processing

### Database

- Migrations are in `Infrastructure/Migrations/`
- Always use `--project Infrastructure --startup-project API` with EF commands
- Connection string is in `API/appsettings.json`

### CORS

- Configured in `API/Program.cs`
- Default configuration allows all origins (development only)
- Configure specific origins for production

---

## 🐛 Troubleshooting

### Build Errors

```powershell
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Database Connection Issues

- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database exists (migrations create it)
- Check firewall settings

### Migration Issues

```powershell
# Reset migrations (careful - deletes data!)
dotnet ef database drop --project Infrastructure --startup-project API
dotnet ef database update --project Infrastructure --startup-project API
```

### Port Already in Use

- Change port in `API/Properties/launchSettings.json`
- Or kill the process using the port

---

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [JWT Authentication Guide](https://jwt.io/introduction)

---

## 📄 License

[Specify your license here]

---

## 👥 Contributing

[Add contribution guidelines if applicable]

---

## 📧 Contact

For questions or support, contact the development team.

---

**Last Updated:** October 30, 2025
