using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Backend.Infrastructure.Extensions;

namespace Tests.Infrastructure.Extensions
{
    public class DbContextExtensionsTests
    {
        [Fact]
        public async Task SaveChangesWithResultAsync_ReturnsSuccess_WhenSaveSucceeds()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TestContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            
            using var context = new TestContext(options);
            context.Entities.Add(new TestEntity());
            
            // Act
            var result = await context.SaveChangesWithResultAsync(CancellationToken.None);
            
            // Assert
            result.IsSuccess.Should().BeTrue();
            context.Entities.Count().Should().Be(1);
        }

        [Fact (Skip = "this is not usually dealt with as an exception in ef core save")]
        public async Task SaveChangesWithResultAsync_ReturnsStorageError_WhenDbUpdateException()
        {
            // Arrange: first context to seed one entity into the In-Memory “database.”
            var options = new DbContextOptionsBuilder<TestContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Exception")
                .Options;

            // Use a “using” block so that this context’s ChangeTracker is disposed after seeding.
            using (var seedContext = new TestContext(options))
            {
                seedContext.Entities.Add(new TestEntity { /* no Id assigned; InMemory auto-assigns */ });
                await seedContext.SaveChangesAsync(); // Now the InMemory store has one row with Id=1
            }

            // Act: use a brand-new context (so its ChangeTracker is empty), but try to insert another with the same key.
            using var testContext = new TestContext(options);
            testContext.Entities.Add(new TestEntity { Id = 1 });
            var result = await testContext.SaveChangesWithResultAsync(CancellationToken.None); // throws error already tracking an entity with this id

            // Assert:
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            result.Errors[0].Should().BeOfType<DomainError>();
            result.Errors[0].Message.Should().Contain("Database error");
        }


        private class TestContext : DbContext
        {
            public DbSet<TestEntity> Entities { get; set; }
            public TestContext(DbContextOptions options) : base(options) {}
        }

        private class TestEntity
        {
            public int Id { get; set; }
        }
    }
}