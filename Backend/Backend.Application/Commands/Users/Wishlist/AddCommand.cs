using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.Wishlist;

public sealed record AddCommand(
    Guid BookId,
    Guid UserId
    ) : IRequest<Result>;