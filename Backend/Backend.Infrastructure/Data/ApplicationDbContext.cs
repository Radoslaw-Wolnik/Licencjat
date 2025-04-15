// Backend.Infrastructure/Data/ApplicationDbContext.cs
using Backend.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using Backend.Infrastructure.Data.Attributes;

namespace Backend.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    
    private static readonly List<Type> _entitiesWithUpdatedAtTimestamp = new()
    {
        typeof(ApplicationUser),
        typeof(Review),
        typeof(Swap),
        typeof(SubSwap),
        typeof(Meetup)
    };  

    public DbSet<GeneralBook> GeneralBooks { get; set; }
    public DbSet<UserBook> UserBooks { get; set; }
    public DbSet<SocialMediaLink> SocialMediaLinks { get; set; }
    public DbSet<Review> Reviews { get; set; }

    // New DbSets for Swap system
    public DbSet<Swap> Swaps { get; set; }
    public DbSet<SubSwap> SubSwaps { get; set; }
    public DbSet<Meetup> Meetups { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Issue> Issues { get; set; }
    public DbSet<Timeline> Timelines { get; set; }
    
    // Many-to-Many Tables
    public DbSet<UserFollowing> UserFollowings { get; set; }
    public DbSet<UserBlocked> UserBlockeds { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureShadowProperties(builder);

        // Configurate Single tables
        ConfigureApplicationUser(builder);
        ConfigureGeneralBook(builder);
        ConfigureUserBook(builder);
        ConfigureSocialMediaLinks(builder);
        ConfigureReview(builder);
        ConfigureSwap(builder);
        ConfigureSubSwap(builder);
        ConfigureMeetup(builder);
        ConfigureFeedback(builder);
        ConfigureIssue(builder);
        ConfigureTimeline(builder);

        // Many-to-Many Configurations
        ConfigureUserFollowing(builder);
        ConfigureUserBlocked(builder);

        // Constrains
        ConfigureDataTypesAndConstraints(builder);
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

    private void ConfigureApplicationUser(ModelBuilder builder)
    {
        // Configure Wishlist many-to-many
        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Wishlist)
            .WithMany(g => g.WishlistedByUsers)
            .UsingEntity(j => j.ToTable("Wishlist"));

        // Configure FollowedBooks many-to-many
        builder.Entity<ApplicationUser>()
            .HasMany(u => u.FollowedBooks)
            .WithMany(g => g.FollowedByUsers)
            .UsingEntity(j => j.ToTable("BookFollowing"));
    }

    private void ConfigureUserBook(ModelBuilder builder)
    {
        builder.Entity<UserBook>()
            .HasOne(ub => ub.User)
            .WithMany(u => u.UserBooks)
            .HasForeignKey(ub => ub.UserId);

        builder.Entity<UserBook>()
            .HasOne(ub => ub.Book)
            .WithMany(b => b.UserBooks)
            .HasForeignKey(ub => ub.BookId);
    }

    private void ConfigureSocialMediaLinks(ModelBuilder builder)
    {
        builder.Entity<SocialMediaLink>()
            .HasOne(s => s.User)
            .WithMany(u => u.SocialMediaLinks)
            .HasForeignKey(s => s.UserId);
    }

    private void ConfigureGeneralBook(ModelBuilder builder)
    {
        builder.Entity<GeneralBook>()
            .Property(g => g.ReviewAverage)
            .HasComputedColumnSql("(SELECT AVG(CAST([Rating] AS float)) FROM [Reviews] WHERE [BookId] = [Id])")
            .ValueGeneratedOnAddOrUpdate();
    }

    private void ConfigureReview(ModelBuilder builder)
    {
        // Reviews
        builder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId);

        builder.Entity<Review>()
            .HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId);
    }

    private void ConfigureSwap(ModelBuilder builder)
    {
        builder.Entity<Swap>()
            .HasOne(s => s.SubSwapA)
            .WithOne()
            .HasForeignKey<Swap>(s => s.SubSwapAId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Swap>()
            .HasOne(s => s.SubSwapB)
            .WithOne()
            .HasForeignKey<Swap>(s => s.SubSwapBId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Swap>()
            .HasMany(s => s.Meetups)
            .WithOne(m => m.Swap)
            .HasForeignKey(m => m.SwapId);

        builder.Entity<Swap>()
            .HasMany(s => s.TimelineUpdates)
            .WithOne(t => t.Swap)
            .HasForeignKey(t => t.SwapId);
    }

    private void ConfigureSubSwap(ModelBuilder builder)
    {
        builder.Entity<SubSwap>()
            .HasOne(s => s.User)
            .WithMany(u => u.SubSwaps)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SubSwap>()
            .HasOne(s => s.UserBookReading)
            .WithMany(ub => ub.SubSwaps)
            .HasForeignKey(s => s.UserBookReadingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SubSwap>()
            .HasOne(s => s.Feedback)
            .WithOne(f => f.SubSwap)
            .HasForeignKey<Feedback>(f => f.SubSwapId);

        builder.Entity<SubSwap>()
            .HasOne(s => s.Issue)
            .WithOne(i => i.SubSwap)
            .HasForeignKey<Issue>(i => i.SubSwapId);
    }


    private void ConfigureMeetup(ModelBuilder builder)
    {
        builder.Entity<Meetup>()
            .HasOne(m => m.User)
            .WithMany(u => u.Meetups)
            .HasForeignKey(m => m.SuggestedUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureFeedback(ModelBuilder builder)
    {
        builder.Entity<Feedback>()
            .HasOne(f => f.User)
            .WithMany(u => u.SwapsFeedbacks)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureIssue(ModelBuilder builder)
    {
        builder.Entity<Issue>()
            .HasOne(i => i.User)
            .WithMany(u => u.SwapsIssues)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureTimeline(ModelBuilder builder)
    {
        builder.Entity<Timeline>()
            .HasOne(t => t.User)
            .WithMany(u => u.SwapsTimelineupdates)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureUserFollowing(ModelBuilder builder)
    {
        builder.Entity<UserFollowing>()
            .HasKey(uf => new { uf.FollowerId, uf.FollowedId });

        builder.Entity<UserFollowing>()
            .HasOne(uf => uf.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(uf => uf.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<UserFollowing>()
            .HasOne(uf => uf.Followed)
            .WithMany(u => u.Followers)
            .HasForeignKey(uf => uf.FollowedId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureUserBlocked(ModelBuilder builder)
    {
        builder.Entity<UserBlocked>()
            .HasKey(ub => new { ub.BlockerId, ub.BlockedId });

        builder.Entity<UserBlocked>()
            .HasOne(ub => ub.Blocker)
            .WithMany(u => u.BlockedUsers)
            .HasForeignKey(ub => ub.BlockerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<UserBlocked>()
            .HasOne(ub => ub.Blocked)
            .WithMany()
            .HasForeignKey(ub => ub.BlockedId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureDataTypesAndConstraints(ModelBuilder builder)
    {
        // Coordinates precision
        builder.Entity<Meetup>()
            .Property(m => m.Location_X)
            .HasColumnType("decimal(10,7)");

        builder.Entity<Meetup>()
            .Property(m => m.Location_Y)
            .HasColumnType("decimal(10,7)");
        
        // Reputation constraint
        builder.Entity<ApplicationUser>()
            .Property(u => u.Reputation)
            .HasColumnType("decimal(4,3)");

        // Using ToTable for check constraints
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable(t => t.HasCheckConstraint("CK_Reputation_Range", "[Reputation] BETWEEN 0 AND 5"));
        });

        builder.Entity<Feedback>(entity =>
        {
            entity.ToTable(t => t.HasCheckConstraint("CK_Feedback_Stars", "[Stars] BETWEEN 1 AND 10"));
        });

        builder.Entity<Review>(entity =>
        {
            entity.ToTable(t => t.HasCheckConstraint("CK_Review_Stars", "[Stars] BETWEEN 1 AND 10"));
        });
    }
}