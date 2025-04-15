namespace Backend.Infrastructure.Entities;


public class UserFollowingEntity
{
    public Guid FollowerId { get; set; }
    public Guid FollowedId { get; set; }
    
    public virtual UserEntity Follower { get; set; } = null!;
    public virtual UserEntity Followed { get; set; } = null!;

}
