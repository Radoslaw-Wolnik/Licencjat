using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Backend.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Set the path to the appsettings.json file in the Backend.API project
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), @"..\Backend.API")) // Navigate to the Backend.API folder
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var connectionString = config.GetConnectionString("DefaultConnection"); // Ensure you have this in appsettings.json

            optionsBuilder.UseNpgsql(connectionString); // Use your database provider (Npgsql, SQL Server, etc.)

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
