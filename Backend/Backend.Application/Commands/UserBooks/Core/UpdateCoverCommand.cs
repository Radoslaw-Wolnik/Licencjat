using FluentResults;
using MediatR;

namespace Backend.Application.Commands.UserBooks.Core;

public sealed record UpdateUserBookCoverCommand(
    Guid UserBookId,
    string CoverFileName
    ) : IRequest<Result<string>>;