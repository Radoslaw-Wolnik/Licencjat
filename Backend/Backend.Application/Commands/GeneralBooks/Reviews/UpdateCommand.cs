using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Reviews;

public sealed record UpdateCommand(
    // Guid BookId,
    Guid ReviewId,
    int? Rating,
    string? Comment
    ) : IRequest<Result<Review>>;