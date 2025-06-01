using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using Swashbuckle.AspNetCore.Annotations;
using MediatR;
using AutoMapper.Extensions.ExpressionMapping;
using FluentValidation.AspNetCore;
using FluentValidation;
using Minio;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

using Backend.API.Middleware;

using Backend.Application.Interfaces;
using Backend.Application.Behaviors;

using Backend.Infrastructure.Data;
using Backend.Infrastructure.Data.Seeders;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Mapping;

// validators
// using Backend.API.Validators;

// commands
using Backend.Application.Commands.Auth;

// repositories
using Backend.Application.Interfaces.Repositories;
using Backend.Infrastructure.Repositories;

// services
using Backend.Application.Interfaces.DbReads;
using Backend.Infrastructure.Services.DbReads;
using Backend.Infrastructure.Services;

using Backend.Infrastructure.Configuration;
using Backend.Infrastructure.BackgroundTasks;
using Backend.Domain.Events;
using Backend.Infrastructure.Services.Queries;
using Backend.Application.Interfaces.Queries;
using Backend.API.Mapping;
using Backend.Application.Querries.GeneralBooks;
using Backend.API.Validators.Auth;


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

// ========== MINIO IMAGE SERVICE ========== //
builder.Services.Configure<MinioSettings>(
    builder.Configuration.GetSection("Minio"));

builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var opts = sp.GetRequiredService<IOptions<MinioSettings>>().Value;
    return new MinioClient()
        .WithEndpoint(opts.Endpoint)
        .WithCredentials(opts.AccessKey, opts.SecretKey)
        .Build();
})
.AddTransient<IImageResizerService, ImageResizerService>() // adds imageresizing service
.AddTransient<IImageStorageService, MinioImageStorageService>(); // adds image storage service

// Thumbanil background worker
var channel = Channel.CreateUnbounded<ThumbnailRequest>();
builder.Services.AddSingleton(channel);
builder.Services.AddHostedService<ThumbnailBackgroundService>();

// ========== INFRASTRUCTURE SERVICES ========== //

// setting the userContext
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, HttpUserContext>();

// repositories
builder.Services.AddScoped<IWriteGeneralBookRepository, WriteGeneralBookRepository>();
builder.Services.AddScoped<IWriteSwapRepository, WriteSwapRepository>();
builder.Services.AddScoped<IWriteUserBookRepository, WriteUserBookRepository>();
builder.Services.AddScoped<IWriteUserRepository, WriteUserRepository>();

// read services
builder.Services.AddTransient<IGeneralBookReadService, GeneralBookReadService>();
builder.Services.AddTransient<ISwapReadService, SwapReadService>();
builder.Services.AddTransient<IUserBookReadService, UserBookReadService>();
builder.Services.AddTransient<IUserReadService, UserReadService>();

// query services
builder.Services.AddTransient<IGeneralBookQueryService, GeneralBookQueryService>();
builder.Services.AddTransient<ISwapQueryService, SwapQueryService>();
builder.Services.AddTransient<IUserBookQueryService, UserBookQueryService>();
builder.Services.AddTransient<IUserQueryService, UserQueryService>();

// services
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ISignInService, SignInService>();
// miniiio and image resizer declared above
// user context at the beginning of this section


// mappers
builder.Services.AddAutoMapper(typeof(BookmarkProfile));
builder.Services.AddAutoMapper(typeof(FeedbackProfile));
builder.Services.AddAutoMapper(typeof(GeneralBookProfile));
builder.Services.AddAutoMapper(typeof(IssueProfile));
builder.Services.AddAutoMapper(typeof(MeetupProfile));
builder.Services.AddAutoMapper(typeof(ReviewProfile));
builder.Services.AddAutoMapper(typeof(SocialMediaProfile));
builder.Services.AddAutoMapper(typeof(SubSwapProfile));
builder.Services.AddAutoMapper(typeof(SwapProfile));
builder.Services.AddAutoMapper(typeof(TimelineProfile));
builder.Services.AddAutoMapper(typeof(UserBookProfile));
builder.Services.AddAutoMapper(typeof(UserProfile)); // cfg => {cfg.AddExpressionMapping();},
// the read models mappers
builder.Services.AddAutoMapper(typeof(GeneralBookReadModelProfile));
builder.Services.AddAutoMapper(typeof(SwapReadModelProfile));
builder.Services.AddAutoMapper(typeof(UserBookReadModelProfile));
builder.Services.AddAutoMapper(typeof(UserReadModelProfile));
// the automappers in Presentation - Api layer - mapping to dtos
builder.Services.AddAutoMapper(typeof(AuthCommandProfile));
builder.Services.AddAutoMapper(typeof(GeneralBookCommandProfile));
builder.Services.AddAutoMapper(typeof(SwapCommandProfile));
builder.Services.AddAutoMapper(typeof(UserBookCommandProfile));
builder.Services.AddAutoMapper(typeof(UserCommandProfile));

// im not sure if this is an automapper - its a converter
builder.Services.AddAutoMapper(typeof(TimelineStatusToSwapStatusConverter));

// AutoMapper - expression mapping added to specyfic mappers
builder.Services.AddAutoMapper(cfg => 
{
    cfg.AddExpressionMapping();
    cfg.AddMaps(
        typeof(UserProfile));
});

// health check for db
builder.Services.AddScoped<DatabaseHealthCheck>();


// ========== MEDIATR & VALIDATION ========== //

// MediatR commands & queries
// This will scan the assembly that contains LoginCommand
// and register ALL of your IRequestHandler<,> + INotificationHandler<> types.
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
    cfg.RegisterServicesFromAssemblies(typeof(GetGeneralBookByIdQuerry).Assembly);
});

// FluentValidation - should scan and find all other validators
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
// builder.Services.AddValidatorsFromAssemblyContaining<CreateGeneralBookRequestValidator>();
// builder.Services.AddValidatorsFromAssemblyContaining<CreateSwapRequestValidator>();
// builder.Services.AddValidatorsFromAssemblyContaining<CreateUserBookRequestValidator>();
// builder.Services.AddValidatorsFromAssemblyContaining<AddSocialMediaRequestValidator>();

// - optional - command validation pipeline
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