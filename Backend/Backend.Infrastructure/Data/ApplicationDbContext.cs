// Backend.Infrastructure/Data/ApplicationDbContext.cs
using Backend.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using Backend.Infrastructure.Data.Attributes;
using Backend.Infrastructure.Views;
using Backend.Domain.Common;

namespace Backend.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<GeneralBookEntity> GeneralBooks { get; set; }
    public DbSet<UserBookEntity> UserBooks { get; set; }
    public DbSet<SocialMediaLinkEntity> SocialMediaLinks { get; set; }
    public DbSet<ReviewEntity> Reviews { get; set; }

    // DbSets for Swaps
    public DbSet<SwapEntity> Swaps { get; set; }
    public DbSet<SubSwapEntity> SubSwaps { get; set; }
    public DbSet<MeetupEntity> Meetups { get; set; }
    public DbSet<FeedbackEntity> Feedbacks { get; set; }
    public DbSet<IssueEntity> Issues { get; set; }
    public DbSet<TimelineEntity> Timelines { get; set; }
    
    // DbSets for Users
    public DbSet<UserFollowingEntity> UserFollowings { get; set; }
    public DbSet<UserBlockedEntity> UserBlockeds { get; set; }
    public DbSet<UserWishlistEntity> UserWishlists { get; set; }

    // DbSets for UserBooks
    public DbSet<BookmarkEntity> Bookmarks { get; set; }

    // views
    public DbSet<GeneralBookWithAverageRating> GeneralBooksWithAverageRatings { get; set; }
    

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureShadowProperties(builder);
        ConfigureDataTypesAndConstraints(builder);

        // ApplicationUser and Owned collections
        builder.Entity<UserEntity>(b =>
        {
            b.OwnsMany(u => u.SocialMediaLinks, sm =>
            {
                sm.WithOwner().HasForeignKey(s => s.UserId);
                sm.Property<Guid>("Id");
                sm.HasKey("Id");
            });
            b.HasMany(u => u.UserBooks)
             .WithOne(ub => ub.User)
             .HasForeignKey(ub => ub.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(u => u.Reviews)
             .WithOne(r => r.User)
             .HasForeignKey(r => r.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(u => u.Followers)
             .WithOne(f => f.Followed)
             .HasForeignKey(f => f.FollowedId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(u => u.Following)
             .WithOne(f => f.Follower)
             .HasForeignKey(f => f.FollowerId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(u => u.BlockedUsers)
             .WithOne(ub => ub.Blocker)
             .HasForeignKey(ub => ub.BlockerId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(u => u.SubSwaps)
             .WithOne(s => s.User)
             .HasForeignKey(s => s.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(u => u.SwapsFeedbacks)
             .WithOne(f => f.User)
             .HasForeignKey(f => f.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(u => u.SwapsIssues)
             .WithOne(i => i.User)
             .HasForeignKey(i => i.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(u => u.SwapsTimelineupdates)
             .WithOne(t => t.User)
             .HasForeignKey(t => t.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(u => u.Meetups)
             .WithOne(m => m.User)
             .HasForeignKey(m => m.SuggestedUserId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(u => u.Wishlist)
             .WithOne(w => w.User)
             .HasForeignKey(w => w.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // GeneralBook & Reviews
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

        // UserBook -> Bookmarks, SubSwaps
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

        // Swap and related
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

        // SubSwap -> Feedback & Issue
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
                .Property<DateTime>("CreatedAt")
                .HasDefaultValueSql("NOW()");

            // Add UpdatedAt ONLY if the entity has the [HasUpdatedAt] attribute
            if (entityType.ClrType.GetCustomAttribute<HasUpdatedAtAttribute>() != null)
                {
                    builder.Entity(entityType.ClrType)
                        .Property<DateTime>("UpdatedAt");
                }
        }
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Always set CreatedAt for new entities
            if (entry.State == EntityState.Added)
            {
                entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
            }

            // Dynamically check if entity has UpdatedAt property
            // var updatedAtProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");
            if (entry.Metadata.FindProperty("UpdatedAt") != null)
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        }

        return base.SaveChanges();
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
        
        // Reputation constraint
        builder.Entity<UserEntity>()
            .Property(u => u.Reputation)
            .HasColumnType("decimal(4,3)");

        // Using ToTable for check constraints
        builder.Entity<UserEntity>(entity =>
        {
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Reputation_Range", 
                "\"Reputation\" BETWEEN 1 AND 5" // PostgreSQL-compatible syntax
            ));
        });

        builder.Entity<FeedbackEntity>(entity =>
        {
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Feedback_Stars", 
                "\"Stars\" BETWEEN 1 AND 5"
            ));
        });

        builder.Entity<ReviewEntity>(entity =>
        {
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Review_Stars", 
                "\"Rating\" BETWEEN 1 AND 10"
            ));
        });
    }
}