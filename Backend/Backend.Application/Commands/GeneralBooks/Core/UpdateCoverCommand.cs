using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Core;

public sealed record UpdateGeneralBookCoverCommand(
    Guid BookId,
    string CoverFileName
    ) : IRequest<Result<string>>;