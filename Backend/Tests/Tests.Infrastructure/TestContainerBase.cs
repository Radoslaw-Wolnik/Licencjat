using Microsoft.EntityFrameworkCore;
using Respawn;
using Npgsql;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;

using Backend.Infrastructure.Data;
using DotNet.Testcontainers.Builders;

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

        // _minioContainer = new MinioBuilder()
        //    .WithImage("minio/minio")
        //    .WithCommand("server /data")
        //    .Build();

        _minioContainer = new MinioBuilder()
            .WithImage("minio/minio:latest") // minio:latest
            .WithCommand("/dataminio")
            .WithEnvironment("MINIO_ROOT_USER", "minioadmin")
            .WithEnvironment("MINIO_ROOT_PASSWORD", "minioadmin")
            .WithPortBinding(9000, true)
            .WithVolumeMount("minio-test-data", "/dataminio") // docker volume create minio-test-data
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(9000) // Wait for port
                .UntilMessageIsLogged("API:")) // Wait for startup log
                // .WithStartupTimeout(TimeSpan.FromMinutes(2)))
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

        // for initialisation specyfic for the child tests
        await OnTestInitializedAsync();
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
    
    protected virtual Task OnTestInitializedAsync() => Task.CompletedTask;
}