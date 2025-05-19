using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Reviews;

public sealed record DeleteCommand(
    // Guid BookId,
    Guid ReviewId
    ) : IRequest<Result>;