using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Wishlist;

public sealed record RemoveWishlistBookCommand(
    Guid UserId,
    Guid WishlistBookId
    ) : IRequest<Result>;