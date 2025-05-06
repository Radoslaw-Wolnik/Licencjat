using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Core;

public sealed record DeleteCommand(
    Guid GeneralBookId
    ) : IRequest<Result>;