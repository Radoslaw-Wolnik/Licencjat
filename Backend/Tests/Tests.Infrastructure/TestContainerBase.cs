using Microsoft.EntityFrameworkCore;
using Respawn;
using Npgsql;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;

using Backend.Infrastructure.Data;

namespace Tests.Infrastructure;

public abstract class TestContainersBase : IAsyncLifetime
{
    protected readonly PostgreSqlContainer _dbContainer;
    protected readonly MinioContainer _minioContainer;
    private Respawner? _respawner;

    public TestContainersBase()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        _minioContainer = new MinioBuilder()
            .WithImage("minio/minio")
            .WithCommand("server /data")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _minioContainer.StartAsync();

        // Apply migrations
        var context = CreateDbContext();
        await context.Database.MigrateAsync();

        // Create connection manually
        var conn = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await conn.OpenAsync();

        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" }
        });
    }

    protected ApplicationDbContext CreateDbContext()
    {
        return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
            .Options);
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await _minioContainer.DisposeAsync();
    }

    public async Task ResetDatabase()
    {
        if (_respawner == null) return;
        
        var conn = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await conn.OpenAsync();
        await _respawner.ResetAsync(conn);
    }
}