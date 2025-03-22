using Backend.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Entities;
using Microsoft.AspNetCore.HttpOverrides;
using Backend.API.Middleware;
using Swashbuckle.AspNetCore.Annotations;
using Backend.Application.Interfaces;
using Backend.Infrastructure.Services;
using Backend.Infrastructure.Seeders;
using Backend.Application.Validators;
using Backend.Application.Validators.Auth;
using Backend.Application.Behaviors;
using FluentValidation;
using MediatR;
using Backend.Infrastructure.Mapping;
using AutoMapper;
using Backend.Application.Services;
using Backend.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

// ========== CORE SERVICES ========== //
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    options.EnableAnnotations();
});
// ========== HEALTH CHECKS ========== //
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database_health_check");

// ========== DATABASE CONFIGURATION ========== //
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========== IDENTITY & AUTHENTICATION ========== //
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        // Password Policy
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        
        // User Policy
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false; // Change when email service ready
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ========== INFRASTRUCTURE SERVICES ========== //
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<DatabaseHealthCheck>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddAutoMapper(typeof(AuthenticationProfile));
builder.Services.AddAutoMapper(typeof(UserProfile));


// ========== MEDIATR & VALIDATION ========== //   <-- Add here
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(RegisterRequestValidator).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

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

if (!builder.Environment.IsDevelopment())
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });
}

// ========== APP BUILD ========== //
var app = builder.Build();

// ========== ENVIRONMENT CONFIG ========== //
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseForwardedHeaders();
    app.UseHsts();
    app.UseExceptionHandler("/error");
}

// ========== INFRASTRUCTURE SETUP ========== //
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Only migrate if enabled in config
    if (builder.Configuration.GetValue<bool>("EnableMigrations"))
    {
        context.Database.Migrate();
    }
    
    // Always seed roles ?if missing?
    // var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    // await InfrastructureSeeder.SeedRolesAsync(roleManager);
}

// ========== MIDDLEWARE PIPELINE ========== //
app.UseMiddleware<ValidationExceptionMiddleware>();


app.UseHttpsRedirection();
app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();