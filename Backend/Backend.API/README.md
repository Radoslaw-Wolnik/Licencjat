# Presentation Layer - API Gateway
**Handles HTTP requests, authentication, and serves as the system's entry point**
<!-- 
![API Layer Diagram](https://miro.medium.com/v2/resize:fit:720/format:webp/1*66isW6uUtN_dd6Y6pqKUHA.png)  
*The outermost layer where HTTP meets business logic*
-->

## 🌐 Responsibility
- **HTTP Handling**: Manage incoming requests and outgoing responses
- **Authentication/Authorization**: Secure endpoints with JWT
- **Validation**: Validate incoming DTOs using FluentValidation
- **Error Handling**: Convert domain errors to standardized HTTP responses
- **Documentation**: Provide OpenAPI/Swagger documentation
- **Dependency Registration**: Configure services and middleware

**Dependency Rule**:  
Depends on **Application Layer** for use cases, no direct access to Domain or Infrastructure.

---

## ⚙️ Key Components

### 1. Controller Structure
Controllers handle HTTP requests and delegate work to MediatR handlers:

```csharp
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public AuthController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = _mapper.Map<RegisterCommand>(request);
        var result = await _sender.Send(command);

        return result.Match(
            userId => CreatedAtAction(nameof(Register), new { userId }),
            errors => errors.ToProblemDetailsResult()
        );
    }
}
```

### 2. DTOs and Validation
Request/Response objects with FluentValidation:

```csharp
// Request DTO
public sealed record RegisterRequest(
    string Email,
    string Username,
    string Password,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string City,
    string Country);

// FluentValidation
public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Username).MinimumLength(3);
        RuleFor(x => x.Password).MinimumLength(8);
        RuleFor(x => x.BirthDate)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-13)))
            .WithMessage("Must be at least 13 years old");
    }
}
```

### 3. Error Handling
Consistent error responses using ProblemDetails:

```csharp
// Result extension
public static IActionResult ToProblemDetailsResult(this List<IError> errors)
{
    var problemDetails = new ProblemDetails
    {
        Type = "https://httpstatuses.io/400",
        Title = "Request processing error",
        Status = StatusCodes.Status400BadRequest
    };
    
    // ... error mapping logic ...
    
    return new ObjectResult(problemDetails)
    {
        StatusCode = problemDetails.Status
    };
}

// Controller usage
return result.Match(
    onSuccess: userId => CreatedAtAction(...),
    onFailure: errors => errors.ToProblemDetailsResult()
);
```

### 4. Authentication & Authorization
Authentication is handled via HTTP-only cookies. After a successful login:
1. Server sets two secure, HTTP-only cookies:
   - `access_token`: Short-lived access token (default 60 minutes)
   - `refresh_token`: Long-lived refresh token (default 7 days)
2. Client includes these cookies in subsequent requests
3. Server automatically refreshes access token when needed


```csharp
// ========== SECURITY CONFIGURATION ========== //
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.LoginPath = "/api/auth/login";
    options.AccessDeniedPath = "/api/auth/forbidden";
    
    // API-friendly responses
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});
```

### 5. AutoMapper Profiles
DTO to Command/Query mapping:

```csharp
public sealed class AuthCommandProfile : Profile
{
    public AuthCommandProfile()
    {
        CreateMap<RegisterRequest, RegisterCommand>();
        CreateMap<LoginRequest, LoginCommand>();
        CreateMap<ForgotPasswordRequest, ForgotCommand>();
    }
}
```

---

## 🚀 Performance Features
1. **Async Processing**: All handlers are async
2. **Response Compression**:
```csharp
app.UseResponseCompression(options => 
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
});
```
3. **Pagination**: Used in all list endpoints
```csharp
public record ListUserBooksQuerry(
    Guid GeneralBookId,
    SortUserBookBy SortBy = SortUserBookBy.Title,
    bool Descending = false,
    int Offset = 0,
    int Limit = 20);
```

---

## 📊 Monitoring & Logging
```csharp
builder.Logging
    .AddConsole()
    .AddApplicationInsights();

builder.Services.AddApplicationInsightsTelemetry();

app.MapHealthChecks("/health");
app.MapPrometheusScrapingEndpoint();
```

---

## 🔍 Swagger Documentation
![Swagger UI Example](https://miro.medium.com/v2/resize:fit:1200/1*_5f4e5bYF0Kp4d6xJNkZMQ.png)

Access at: `https://localhost:5001/swagger`

---

## ✅ Key Implementation Decisions
1. **MediatR Pattern**: All business logic accessed through commands/queries
2. **Result Pattern**: Unified error handling across layers
3. **AutoMapper**: Clean DTO to Command/Query mapping
4. **FluentValidation**: Centralized request validation
5. **JWT Authentication**: Stateless authentication with refresh token support
6. **Rich Error Handling**: Detailed error responses with ProblemDetails
7. **Pagination**: Consistent pagination pattern across all list endpoints

---

## 📜 Next Steps
- [Domain Layer](../Backend.Domain/README.md)
- [Application Layer](../Backend.Application/README.md)
- [Infrastructure Layer](../Infrastructure/README.md)

> "The presentation layer is the face of your application - it should be welcoming to consumers while rigorously protecting the system within."