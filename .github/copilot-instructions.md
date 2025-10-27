## Quick context — what this repo is

- This is a small ASP.NET Core solution (API.sln) split into clear layers: `API/` (web layer + controllers + middleware), `Application/` (services, DTOs, interfaces), `Core/` (domain entities, exceptions, repository interfaces), `Infrastructure/` (EF Core DbContext, repository implementations, migrations), and `Repositories/` (implementation namespace lives under `Infrastructure.Repositories`).

## High-level architecture notes

- Web/API: `API/Program.cs` wires up DI (services+repositories), Swagger, Identity, and middleware. See `API/Middlewares/ExceptionHandlingMiddleware.cs` for global exception handling.
- Persistence: `Infrastructure/ApplicationDbContext.cs` inherits `IdentityDbContext<User>` and applies global query filters for soft delete (`IsDeleted`). Migrations are in `Infrastructure/Migrations/`.
- Domain & DTOs: Domain entities are in `Core/Entities` (e.g., `Category.cs`); DTOs live under `Application/DTOs/Category` and services in `Application/Services` (e.g., `CategoryService.cs`).
- Repos & services: Repositories implement `Core.IRepositories` under `Infrastructure.Repositories` and are registered in `API/Program.cs` (e.g., `ICategoryRepository` -> `CategoryRepository`, `ICategoryService` -> `CategoryService`).

## Important conventions and patterns (project-specific)

- Soft deletes: Entities have `IsDeleted` and `ApplicationDbContext.OnModelCreating` applies `HasQueryFilter(...)` for `Product` and `Category`. Repositories implement delete by setting `IsDeleted = true` (see `Infrastructure/Repositories/CategoryRepository.cs`).
- Exceptions -> middleware: Services throw domain exceptions from `Core/Exceptions` (e.g. `NotFoundException`, `BadRequestException`, `DomainException`) and `API/Middlewares/ExceptionHandlingMiddleware.cs` maps them to HTTP status codes and JSON payloads — do not swallow these in controllers.
- DTO naming: Use `CategoryDTO`, `CategoryListDTO`, `CategoryDetailDTO`, `CategoryUpdateDTO` (see `Application/DTOs/Category`). Services map entities to DTOs manually in `Application.Services`.
- Async-first: Repository/service methods are asynchronous (`Task`, `Task<T>`). Controllers call service async methods and return appropriate ActionResult types (see `API/Controllers/CategoryController.cs`).
- Identity: User management uses ASP.NET Core Identity. `ApplicationDbContext` extends `IdentityDbContext<User>` and Identity is configured in `Program.cs`.

## Common developer workflows (commands you can run)

- Build whole solution: `dotnet build API.sln`
- Run the API project (development): `dotnet run --project API\API.csproj` or `cd API; dotnet run`
- EF Core migrations (project separation):
  - Add migration: `dotnet ef migrations add <Name> --project Infrastructure --startup-project API`
  - Apply migration: `dotnet ef database update --project Infrastructure --startup-project API`
  (Reason: DbContext lives in `Infrastructure`, but the startup application with configuration is `API`.)

## Files to check when changing behavior

- DI and startup config: `API/Program.cs` — add service/repo registrations here.
- Global exceptions: `API/Middlewares/ExceptionHandlingMiddleware.cs` — how exceptions map to HTTP responses.
- EF configuration & query filters: `Infrastructure/ApplicationDbContext.cs`.
- Repository patterns: `Infrastructure/Repositories/*Repository.cs` (CRUD and soft-delete behavior).
- Service layer mapping & validation: `Application/Services/*Service.cs` — services should throw `Core.Exceptions` where appropriate.

## Example patterns to follow (copyable)

- Register a new repository & service in `API/Program.cs`:

  builder.Services.AddScoped<INewRepository, NewRepository>();
  builder.Services.AddScoped<INewService, NewService>();

- Soft-delete in repository (follow `CategoryRepository.DeleteAsync`):

  category.IsDeleted = true;
  _context.Categories.Update(category);
  await _context.SaveChangesAsync();

## Notes & gotchas

- No unit tests present in the workspace — add tests under a dedicated test project if needed.
- Connection string is in `API/appsettings.json` under `ConnectionStrings:DefaultConnection`.
- When using EF CLI, always pass `--startup-project API` so the app's configuration (connection string, environment) is used.

If any of these details are incomplete or you'd like runnable snippets (e.g., how to scaffold a new controller+service+repo), tell me which area to expand and I'll update/extend these instructions.
