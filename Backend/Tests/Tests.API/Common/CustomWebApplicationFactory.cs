// CustomWebApplicationFactory.cs
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost; // insted of using Backend.API;


namespace Tests.API.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        // Add test-specific configuration here
    }
    /*
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace DB with in-memory version
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options => 
            {
                options.UseSqlite("DataSource=:memory:");
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to initialize the database
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.OpenConnection();
            db.Database.EnsureCreated();
        });
    }
    */
}