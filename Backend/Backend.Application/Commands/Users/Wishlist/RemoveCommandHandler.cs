using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Wishlist;

public class RemoveWishlistBookCommandHandler
    : IRequestHandler<RemoveWishlistBookCommand, Result>
{
    private readonly IWriteUserRepository _userRepo;
    public RemoveWishlistBookCommandHandler(
        IWriteUserRepository userRepo
    ) {
        _userRepo = userRepo;
    }

    public async Task<Result> Handle(
        RemoveWishlistBookCommand request,
        CancellationToken cancellationToken
    ) {
        return await _userRepo.RemoveWishlistBookAsync(request.UserId, request.WishlistBookId, cancellationToken);
    }
}
