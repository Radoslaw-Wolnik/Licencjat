# Backend API (Clean Architecture)

## Project Structure
``` bash
📂 Backend
├── 📄 docker-compose.yml # Docker compose configuration
├── 📄 Backend.sln # Solution file
├── 📂 Backend.API # Presentation Layer (Web API)
│   ├── 📂 Controllers # API endpoints
│   ├── 📄 Program.cs # Entry point & middleware config
│   ├── 📄 appsettings.json # Configuration values
│   └── 📄 Dockerfile # Docker build instructions
├── 📂 Backend.Application # Application Layer
│   ├── 📂 Services # Business logic implementations
│   ├── 📂 DTOs # Data Transfer Objects
│   └── 📂 Interfaces # Service contracts
├── 📂 Backend.Infrastructure # Infrastructure Layer
│   ├── 📂 Data # Database configuration
│   │    └── ApplicationDbContext.cs
│   └── 📂 Repositories # Data access implementations
└── 📂 Backend.Domain # Domain Layer
    ├── 📂 Entities # Core business entities
    └── 📂 ValueObjects # Domain value objects
```


## Layer Responsibilities

### 🎯 Domain Layer
- **Pure business logic** - no dependencies!
- Contains:
  - Entities (e.g., `User`, `Order`)
  - Value Objects
  - Domain Events
  - Enums
- **Zero dependencies** on other projects

### ⚙️ Application Layer
- Orchestrates business workflows
- Contains:
  - CQRS handlers
  - Validation logic
  - Service interfaces
  - DTOs
- **Depends on:** Domain Layer

### 🛠️ Infrastructure Layer
- Implements technical details:
  - Database access (EF Core)
  - File storage
  - External services
  - Repositories
- **Depends on:** Application + Domain

### 🌐 API Layer
- Handles HTTP requests/responses
- Contains:
  - Controllers
  - Middleware
  - Auth configuration
- **Depends on:** Application + Infrastructure

---

## NuGet Packages
- **API**: Web-related packages (JWT, Swagger)
- **Infrastructure**: EF Core + PostgreSQL
- **Application/Domain**: Typically no packages needed (pure business logic)
- Optional for Application:
  ```bash
  dotnet add package MediatR          # For CQRS
  dotnet add package AutoMapper       # For object mapping
  ```

## First time setup project
``` bash
# From the solution root

# 1. Restore dependencies
dotnet restore

# 2. Create initial migration
dotnet ef migrations add InitialCreate --project Backend.Infrastructure
dotnet ef migrations script --project Backend.Infrastructure --output migrations.sql

# 3. Start containers
docker-compose up --build

# 4. Test API
curl https://localhost:5000/swagger
```

## Testing Authentication
``` bash
# Login example
curl -X POST "https://localhost:5001/api/auth/login" \
     -H "Content-Type: application/json" \
     -d '{"email":"user@example.com","password":"Password123!"}'

# Response will set auth cookies automatically
```

to remove migrations use: 
`ef migrations remove`