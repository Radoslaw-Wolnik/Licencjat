using System.Linq.Expressions;
using Backend.Application.DTOs;
using Backend.Application.DTOs.Auth;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using FluentResults;

namespace Backend.Infrastructure.Repositories.Users;

public interface IUserRepository :
    ICoreUserRepository,
    IAuthUserRepository,
    
    IUserWishlistRepository,
    IUserFollowingRepository,
    IUserBlockedRepository,

    IUserSocialMediaRepository,
    IUserBooksRepository
{ }


public class UserRepository : IUserRepository
{
    private readonly ICoreUserRepository _core;
    private readonly IAuthUserRepository _auth;
    private readonly IUserWishlistRepository _wishlist;
    private readonly IUserFollowingRepository _following;
    private readonly IUserBlockedRepository _blocked;
    private readonly IUserSocialMediaRepository _socialmedia;
    private readonly IUserBooksRepository _userBooks;

    public UserRepository(
      ICoreUserRepository core,
      IAuthUserRepository auth,
      IUserWishlistRepository wishlist,
      IUserFollowingRepository following,
      IUserBlockedRepository blocked,
      IUserSocialMediaRepository socialmedia,
      IUserBooksRepository userBooks
    ) {
      _core = core;
      _auth = auth;
      _wishlist = wishlist;
      _following = following;
      _blocked = blocked;
      _socialmedia = socialmedia;
      _userBooks = userBooks;
    }

    // --- ICoreUserRepository --- 
    public Task<Result<bool>> ExistsAsync( Expression<Func<UserProjection, bool>> predicate) 
        => _core.ExistsAsync(predicate);
    public Task<Result<Guid>> AddAsync(User u)
        => _core.AddAsync(u);
    public Task<Result<User>> GetByAsync(Expression<Func<UserProjection, bool>> predicate)
        => _core.GetByAsync(predicate);
    public Task<Result<User>> GetByIdAsync(Guid id) 
        => _core.GetByIdAsync(id);
    public Task<Result<User>> GetUserWithIncludes(Guid userId, 
        params Expression<Func<UserProjection, object>>[] includes)
        => _core.GetUserWithIncludes(userId, includes);
    public Task<Result> UpdateAsync(User u) 
        => _core.UpdateAsync(u);
    public Task<Result> DeleteAsync(Guid id) 
        => _core.DeleteAsync(id);
    

    // --- IAuthUserRepository ---
    public Task<Result<LoginUserInfo>> GetLoginInfoAsync(Expression<Func<UserProjection,bool>> predicate)
        => _auth.GetLoginInfoAsync(predicate);


    // --- IUserWishlistRepository ---
    public Task<Result<IReadOnlyCollection<Guid>>> GetWishlistAsync(Guid u)
        => _wishlist.GetWishlistAsync(u);
    public Task<Result> AddToWishlistAsync(Guid u, Guid b)
        => _wishlist.AddToWishlistAsync(u, b);
    public Task<Result> RemoveFromWishlistAsync(Guid u, Guid b)
        => _wishlist.RemoveFromWishlistAsync(u, b);
    public Task<Result<bool>> WishlistContainsAsync(Guid u, Guid b)
        => _wishlist.WishlistContainsAsync(u, b);
    

    /// --- IUSerFollowingRepository ---
    public Task<Result<IReadOnlyCollection<Guid>>> GetFollowingAsync(Guid userId)
        => _following.GetFollowingAsync(userId);

    public Task<Result> AddToFollowingAsync(Guid userId, Guid newFollowingId)
        => _following.AddToFollowingAsync(userId, newFollowingId);
    public Task<Result> RemoveFromFollowingAsync(Guid userId, Guid unfollowingId)
        => _following.RemoveFromFollowingAsync(userId, unfollowingId);
    
    public Task<Result<bool>> FollowingContainsAsync(Guid userId, Guid candidateId)
        => _following.FollowingContainsAsync(userId, candidateId);


    /// --- IUserBlockedRepository ---
    public Task<Result<IReadOnlyCollection<Guid>>> GetBlockedAsync(Guid userId)
        => _blocked.GetBlockedAsync(userId);

    public Task<Result> AddToBlockedAsync(Guid userId, Guid newBlockedId)
        => _blocked.AddToBlockedAsync(userId, newBlockedId);
    public Task<Result> RemoveFromBlockedAsync(Guid userId, Guid unblockedId)
        => _blocked.RemoveFromBlockedAsync(userId, unblockedId);
    
    public Task<Result<bool>> BlockedContainsAsync(Guid userId, Guid candidateId)
        => _blocked.BlockedContainsAsync(userId, candidateId);


    // --- IUserSocialMediaRepository ---
    public Task<Result<IReadOnlyCollection<SocialMediaLink>>> GetSocialMediaAsync(Guid u)
        => _socialmedia.GetSocialMediaAsync(u);
    public Task<Result> AddSocialMediaAsync(Guid u, SocialMediaLink l)
        => _socialmedia.AddSocialMediaAsync(u, l);
    public Task<Result> UpdateSocialMediaAsync(Guid u, SocialMediaLink l)
        => _socialmedia.UpdateSocialMediaAsync(u, l);
    public Task<Result> RemoveSocialMediaAsync(Guid u, Guid id)
        => _socialmedia.RemoveSocialMediaAsync(u, id);
    public Task<Result<bool>> SocialMediaContainsAsync(Guid u, Guid b)
        => _socialmedia.SocialMediaContainsAsync(u, b);


    // --- IUserBookRepository ---
    public Task<Result<IReadOnlyCollection<UserBook>>> GetUserBooksAsync(Guid u)
        => _userBooks.GetUserBooksAsync(u);
    public Task<Result> AddUserBookAsync(Guid u, UserBook b)
        => _userBooks.AddUserBookAsync(u, b);
    public Task<Result> UpdateUserBookAsync(Guid u, UserBook b)
        => _userBooks.UpdateUserBookAsync(u, b);
    public Task<Result> RemoveUserBookAsync(Guid u, Guid b)
        => _userBooks.RemoveUserBookAsync(u, b);
    public Task<Result<bool>> UserBookContainsAsync(Guid u, Guid b)
        => _userBooks.UserBookContainsAsync(u, b);
}
