using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Wishlist;

public sealed record AddWishlistBookCommand(
    Guid UserId,
    Guid WishlistBookId
    ) : IRequest<Result>;