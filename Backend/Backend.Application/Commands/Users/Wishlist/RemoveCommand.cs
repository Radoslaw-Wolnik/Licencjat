using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Wishlist;

public sealed record RemoveCommand(
    Guid WishlistLinkId
    ) : IRequest<Result>;