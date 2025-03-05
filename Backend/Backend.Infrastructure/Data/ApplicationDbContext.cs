// Backend.Infrastructure/Data/ApplicationDbContext.cs
using Backend.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Backend.Domain.Entities;

namespace Backend.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // Add DbSets for domain entities
    public DbSet<User> DomainUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure domain entities
        builder.Entity<User>(entity => 
        {
            entity.ToTable("DomainUsers");
            entity.HasKey(u => u.Id);
        });
    }
}