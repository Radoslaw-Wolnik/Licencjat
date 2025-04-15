namespace Backend.Infrastructure.Entities;


public class UserBlocked
{
    public Guid BlockerId { get; set; }
    public Guid BlockedId { get; set; }
    
    public virtual ApplicationUser Blocker { get; set; } = null!;
    public virtual ApplicationUser Blocked { get; set; } = null!;
}