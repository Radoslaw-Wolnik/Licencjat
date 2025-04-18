using Backend.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Entities;
using Microsoft.AspNetCore.HttpOverrides;
using Backend.API.Middleware;
using Swashbuckle.AspNetCore.Annotations;
using Backend.Application.Interfaces;
using Backend.Infrastructure.Services;
using Backend.Infrastructure.Data.Seeders;
using Backend.Application.Validators;
using Backend.Application.Validators.Auth;
using Backend.Application.Behaviors;
using MediatR;
using Backend.Infrastructure.Mapping;
using AutoMapper;
using Backend.Infrastructure.Repositories;
using AutoMapper.Extensions.ExpressionMapping;
using BAckend.Infrastructure.Services;
using Backend.Application.Features.Auth;
using FluentValidation.AspNetCore;
using FluentValidation;


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
builder.Services.AddIdentity<UserEntity, IdentityRole<Guid>>(options =>
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
// repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<DatabaseHealthCheck>();

// services
builder.Services.AddScoped<ISignInService, SignInService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddTransient<IEmailService, EmailService>();

// mappers
builder.Services.AddAutoMapper(typeof(UserProfile)); // cfg => {cfg.AddExpressionMapping();},
builder.Services.AddAutoMapper(typeof(AuthProfile));


// ========== MEDIATR & VALIDATION ========== //

// MediatR
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly));

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

// Optional command validation pipeline
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// AutoMapper
builder.Services.AddAutoMapper(cfg => 
{
    cfg.AddExpressionMapping();
    cfg.AddMaps(typeof(UserProfile), typeof(AuthProfile));
});

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
    
    // Seed roles only if they are missing - checked in the roles seeder
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await InfrastructureSeeder.SeedRolesAsync(roleManager);
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