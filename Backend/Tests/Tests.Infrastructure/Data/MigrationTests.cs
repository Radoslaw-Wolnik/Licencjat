using Backend.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Infrastructure;
using Xunit;

namespace Tests.Infrastructure.Data;

// important !!!
// please run docker app for performing this tests


public class MigrationTests : TestContainersBase, IAsyncLifetime
{
    private ApplicationDbContext _context = null!;

    protected override Task OnTestInitializedAsync()
    {
        _context = CreateDbContext();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Database_HasBeenMigrated_ShouldHaveTables()
    {
        // Act
        var tables = await _context.Database
            .SqlQuery<string>($"SELECT tablename FROM pg_tables WHERE schemaname = 'public'")
            .ToListAsync();

        // Assert
        tables.Should().Contain(new[] { "AspNetUsers", "GeneralBooks", "UserBooks", "Swaps" });
    }

    [Fact (Skip = "the view is not used - so its not created")]
    public async Task View_GeneralBooksWithAverageRatings_Exists()
    {
        // Act
        var result = await _context.GeneralBooksWithAverageRatings
            .Take(1)
            .ToListAsync();

        // Assert
        result.Should().NotBeNull();
    }

    public new async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await base.DisposeAsync();
    }
}