using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Wishlist;

public class AddWishlistBookCommandHandler
    : IRequestHandler<AddWishlistBookCommand, Result>
{
    private readonly IWriteUserRepository _userRepo;
    public AddWishlistBookCommandHandler(
        IWriteUserRepository userRepo
    ) {
        _userRepo = userRepo;
    }

    public async Task<Result> Handle(
        AddWishlistBookCommand request,
        CancellationToken cancellationToken
    ) {
        return await _userRepo.AddWishlistBookAsync(request.UserId, request.WishlistBookId, cancellationToken);
    }
}
