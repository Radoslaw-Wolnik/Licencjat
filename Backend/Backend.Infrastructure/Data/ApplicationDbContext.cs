// Backend.Infrastructure/Data/ApplicationDbContext.cs
using Backend.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace Backend.Infrastructure.Data;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    
    public DbSet<Address> Addresses { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Ensure Identity tables are correctly mapped
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers");
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.BirthDate).IsRequired();
            entity.Property(e => e.Reputation).HasDefaultValue(0);
            
            // One-to-One relationship with Address (optional)
            entity.HasOne(a => a.Address)
                .WithOne(a => a.User) // Each address belongs to one user
                .HasForeignKey<Address>(a => a.UserId) // Address table stores UserId as FK
                .OnDelete(DeleteBehavior.Cascade); // If User is deleted, delete Address
        });
        
        // Address entity configuration
        builder.Entity<Address>(entity =>
        {
            entity.ToTable("Addresses");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Street).IsRequired();
            entity.Property(a => a.City).IsRequired();
        });
    }
}