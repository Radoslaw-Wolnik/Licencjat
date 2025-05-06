using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Reviews;

public sealed record CreateCommand(
    Guid UserId,
    Guid BookId,
    int Rating,
    string? Comment
    ) : IRequest<Result<Review>>;