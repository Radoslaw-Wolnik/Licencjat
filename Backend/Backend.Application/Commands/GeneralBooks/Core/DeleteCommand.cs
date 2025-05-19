using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Core;

public sealed record DeleteGeneralBookCommand(
    Guid GeneralBookId,
    string PhotoKey
    ) : IRequest<Result>;
