using Backend.Application.Interfaces;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Infrastructure.Services;

public class SignInServiceIntegrationTests : TestContainersBase  // Remove IClassFixture
{
    private ApplicationDbContext _dbContext = null!;
    private ISignInService _signInService = null!;
    private IServiceProvider _serviceProvider = null!;

    protected override async Task OnTestInitializedAsync()
    {
        await base.OnTestInitializedAsync();

        var services = new ServiceCollection();

        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseNpgsql(_dbContainer.GetConnectionString()));



        // Version without seeding roles
        services.AddIdentityCore<UserEntity>(opts =>
        {
            opts.Password.RequireDigit = false;
            opts.Password.RequiredLength = 4;
            opts.Password.RequireLowercase = false;
            opts.Password.RequireUppercase = false;
            opts.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders()
        .AddSignInManager();  // Explicitly add SignInManager

        services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddCookie(IdentityConstants.ApplicationScheme, o =>
            {
                o.LoginPath = "/account/login";
            });

        /*
        // Using IdentityRole<Guid> to match the actual implementation
        services.AddIdentity<UserEntity, IdentityRole<Guid>>(opts => 
        {
            opts.Password.RequireDigit = false;
            opts.Password.RequiredLength = 4;
            opts.Password.RequireLowercase = false;
            opts.Password.RequireUppercase = false;
            opts.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        });
        */

        services.AddScoped<ISignInService, SignInService>();

        _serviceProvider = services.BuildServiceProvider();

        using var scope = _serviceProvider.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _signInService = scope.ServiceProvider.GetRequiredService<ISignInService>();

        await _dbContext.Database.EnsureCreatedAsync();

        // Seed roles if needed/able to
        // No service for type 'Microsoft.AspNetCore.Identity.RoleManager`1[Microsoft.AspNetCore.Identity.IdentityRole`1[System.Guid]]' has been registered.
        // var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        // await SeedRolesAsync(roleManager);
    }

    private async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        var roles = new[] { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }

    [Fact]
    public async Task RealLogin_ValidUser_ReturnsSuccess()
    {
        // Create a new scope for this test
        using var scope = _serviceProvider.CreateScope();
        var scopedDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var scopedSignInService = scope.ServiceProvider.GetRequiredService<ISignInService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();

        // Arrange
        var user = new UserEntity
        {
            FirstName = "test",
            LastName = "kowalski",
            UserName = "testuser",
            Email = "test@example.com",
            BirthDate = new DateOnly(1990, 1, 1),
            City = "London",
            Country = "uk" };
        await userManager.CreateAsync(user, "Test123!");
        // scopedDbContext.Users.Add(user);
        // await scopedDbContext.SaveChangesAsync();
        // await userManager.AddPasswordAsync(user, "Test123!");

        // Act
        var result = await scopedSignInService.LoginAsync("testuser", "Test123!", false);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value);
    }
}