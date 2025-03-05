using Backend.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Entities;
using Microsoft.AspNetCore.HttpOverrides;
using Backend.API.Middleware;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);

// ========== SERVICE CONFIGURATION ========== //

// [1] Add Controller Support
builder.Services.AddControllers();

// [2] Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database_health_check"); 

// [2] Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// [3] Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    // Password requirements
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure HTTPS based on environment
if (!builder.Environment.IsDevelopment())
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });
}

// [4] Cookie Authentication Settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true; // Prevent XSS
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true; // Renew cookie on activity
    options.LoginPath = "/api/auth/login"; // Custom auth endpoints
    options.AccessDeniedPath = "/api/auth/forbidden";
});

// [5] Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

// ========== APP BUILD & MIDDLEWARE ========== //
var app = builder.Build();

// [6] Database Migration
/*
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (app.Environment.IsDevelopment())
    {
        dbContext.Database.EnsureCreated(); // Ensure the database schema is created in dev
    }
    else
    {
        dbContext.Database.Migrate(); // Apply migrations in production
    }
}
*/

// [7] Development Tools
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection(); // in dev no https
}
else
{
    app.UseForwardedHeaders();
    app.UseHsts();
}

// [8] Security Middleware
// app.UseHttpsRedirection(); // Force HTTPS
app.UseCookiePolicy(); // Enforce cookie rules

// [9] Auth Middleware
app.UseAuthentication(); // Identity system
app.UseAuthorization(); // [Authorize] attribute

// [10] Endpoint Mapping
app.MapControllers(); // Discover controllers
app.MapHealthChecks("/health");

app.Run();