using Backend.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using Backend.Infrastructure.Data.Attributes;
using Backend.Infrastructure.Views;
using Backend.Domain.Common;
using System.Linq;

namespace Backend.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // Root aggregates - public
    public DbSet<GeneralBookEntity> GeneralBooks { get; set; }
    public DbSet<UserBookEntity> UserBooks { get; set; }
    public DbSet<SwapEntity> Swaps { get; set; }
    
    // Child entities - internal for controlled access
    internal DbSet<ReviewEntity> Reviews { get; set; }
    internal DbSet<BookmarkEntity> Bookmarks { get; set; }
    internal DbSet<SocialMediaLinkEntity> SocialMediaLinks { get; set; }
    internal DbSet<UserFollowingEntity> UserFollowings { get; set; }
    internal DbSet<UserBlockedEntity> UserBlockeds { get; set; }
    internal DbSet<UserWishlistEntity> UserWishlists { get; set; }
    internal DbSet<SubSwapEntity> SubSwaps { get; set; }
    internal DbSet<MeetupEntity> Meetups { get; set; }
    internal DbSet<FeedbackEntity> Feedbacks { get; set; }
    internal DbSet<IssueEntity> Issues { get; set; }
    internal DbSet<TimelineEntity> Timelines { get; set; }

    // Views
    public DbSet<GeneralBookWithAverageRating> GeneralBooksWithAverageRatings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        ConfigureShadowProperties(builder);
        ConfigureDataTypesAndConstraints(builder);
        ConfigureEntityRelationships(builder);
    }

    private void ConfigureEntityRelationships(ModelBuilder builder)
    {
        // User Entity Relationships
        builder.Entity<UserEntity>(b =>
        {
            // Social Media Links (1:many)
            b.HasMany(u => u.SocialMediaLinks)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // User Books
            b.HasMany(u => u.UserBooks)
                .WithOne(ub => ub.User)
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Reviews
            b.HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Followers/Following
            b.HasMany(u => u.Followers)
                .WithOne(f => f.Followed)
                .HasForeignKey(f => f.FollowedId)
                .OnDelete(DeleteBehavior.Restrict);
            
            b.HasMany(u => u.Following)
                .WithOne(f => f.Follower)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Blocked Users
            b.HasMany(u => u.BlockedUsers)
                .WithOne(ub => ub.Blocker)
                .HasForeignKey(ub => ub.BlockerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Wishlist
            b.HasMany(u => u.Wishlist)
                .WithOne(w => w.User)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // GeneralBook Relationships
        builder.Entity<GeneralBookEntity>(b =>
        {
            b.HasMany(g => g.Reviews)
                .WithOne(r => r.Book)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);
            
            b.HasMany(g => g.UserBooks)
                .WithOne(ub => ub.Book)
                .HasForeignKey(ub => ub.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserBook Relationships
        builder.Entity<UserBookEntity>(b =>
        {
            b.HasMany(ub => ub.Bookmarks)
                .WithOne(bk => bk.UserBook)
                .HasForeignKey(bk => bk.UserBookId)
                .OnDelete(DeleteBehavior.Restrict);
            
            b.HasMany(ub => ub.SubSwaps)
                .WithOne(sub => sub.UserBookReading)
                .HasForeignKey(sub => sub.UserBookReadingId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Swap Relationships
        builder.Entity<SwapEntity>(b =>
        {
            b.HasOne(s => s.SubSwapRequesting)
                .WithOne()
                .HasForeignKey<SwapEntity>(s => s.SubSwapRequestingId)
                .OnDelete(DeleteBehavior.Restrict);
            
            b.HasOne(s => s.SubSwapAccepting)
                .WithOne()
                .HasForeignKey<SwapEntity>(s => s.SubSwapAcceptingId)
                .OnDelete(DeleteBehavior.Restrict);
            
            b.HasMany(s => s.SubSwaps)
                .WithOne(sub => sub.Swap)
                .HasForeignKey(sub => sub.SwapId)
                .OnDelete(DeleteBehavior.Cascade);
            
            b.HasMany(s => s.Meetups)
                .WithOne(m => m.Swap)
                .HasForeignKey(m => m.SwapId);
            
            b.HasMany(s => s.TimelineUpdates)
                .WithOne(t => t.Swap)
                .HasForeignKey(t => t.SwapId);
        });

        // SubSwap Relationships
        builder.Entity<SubSwapEntity>(b =>
        {
            b.HasOne(s => s.Feedback)
                .WithOne(f => f.SubSwap)
                .HasForeignKey<FeedbackEntity>(f => f.SubSwapId);
            
            b.HasOne(s => s.Issue)
                .WithOne(i => i.SubSwap)
                .HasForeignKey<IssueEntity>(i => i.SubSwapId);
        });

        // Views
        builder.Entity<GeneralBookWithAverageRating>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("GeneralBooksWithAverageRatings");
        });
    }

    private void ConfigureShadowProperties(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            // CreatedAt shadow property for all entities
            builder.Entity(entityType.ClrType)
                .Property<DateTime>("CreatedAtTime");

            // Add UpdatedAt only for entities with [HasUpdatedAt] attribute
            if (entityType.ClrType.GetCustomAttribute<HasUpdatedAtAttribute>() != null)
            {
                builder.Entity(entityType.ClrType)
                    .Property<DateTime>("UpdatedAtTime");
            }
        }
    }

    public override int SaveChanges()
    {
        SetAuditTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        SetAuditTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Set CreatedAt for new entities
            if (entry.State == EntityState.Added)
            {
                entry.Property("CreatedAtTime").CurrentValue = DateTime.UtcNow;
            }

            // Set UpdatedAt if property exists
            if (entry.Metadata.FindProperty("UpdatedAtTime") != null && 
                entry.State == EntityState.Modified)
            {
                entry.Property("UpdatedAtTime").CurrentValue = DateTime.UtcNow;
            }
        }
    }

    private void ConfigureDataTypesAndConstraints(ModelBuilder builder)
    {
        // Coordinates precision
        builder.Entity<MeetupEntity>()
            .Property(m => m.Location_X)
            .HasColumnType("decimal(10,7)");

        builder.Entity<MeetupEntity>()
            .Property(m => m.Location_Y)
            .HasColumnType("decimal(10,7)");

        // Reputation column type
        builder.Entity<UserEntity>()
            .Property(u => u.Reputation)
            .HasColumnType("decimal(4,3)");

        // Check constraints via ToTable(...)
        builder.Entity<UserEntity>()
            .ToTable(tb => tb.HasCheckConstraint(
                "CK_Reputation_Range",
                "\"Reputation\" >= 1 AND \"Reputation\" <= 5"));

        builder.Entity<FeedbackEntity>()
            .ToTable(tb => tb.HasCheckConstraint(
                "CK_Feedback_Stars",
                "\"Stars\" >= 1 AND \"Stars\" <= 5"));

        builder.Entity<ReviewEntity>()
            .ToTable(tb => tb.HasCheckConstraint(
                "CK_Review_Stars",
                "\"Rating\" >= 1 AND \"Rating\" <= 10"));
    }

}