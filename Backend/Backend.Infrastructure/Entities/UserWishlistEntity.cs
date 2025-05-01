namespace Backend.Infrastructure.Entities;

public class UserWishlistEntity
{
    public Guid UserId { get; set; }
    public Guid GeneralBookId { get; set; }
    
    public virtual UserEntity User { get; set; } = null!;
    public virtual GeneralBookEntity GeneralBook { get; set; } = null!;
}
