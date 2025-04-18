namespace Backend.Infrastructure.Entities;


public class UserBlockedEntity
{
    public Guid Id { get; set; }
    public Guid BlockerId { get; set; }
    public Guid BlockedId { get; set; }
    
    public virtual UserEntity Blocker { get; set; } = null!;
    public virtual UserEntity Blocked { get; set; } = null!;
}