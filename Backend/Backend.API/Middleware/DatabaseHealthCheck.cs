using Microsoft.Extensions.Diagnostics.HealthChecks;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.API.Middleware;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _dbContext;

    public DatabaseHealthCheck(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            return HealthCheckResult.Healthy("Database is online");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is unavailable", ex);
        }
    }
}
