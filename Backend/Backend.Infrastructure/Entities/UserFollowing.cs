namespace Backend.Infrastructure.Entities;


public class UserFollowing
{
    public Guid FollowerId { get; set; }
    public Guid FollowedId { get; set; }
    
    public virtual ApplicationUser Follower { get; set; } = null!;
    public virtual ApplicationUser Followed { get; set; } = null!;

}
