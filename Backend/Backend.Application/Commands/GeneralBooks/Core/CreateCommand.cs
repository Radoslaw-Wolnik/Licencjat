using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Core;

public sealed record CreateGeneralBookCommand(
    string Title,
    string Author,
    DateOnly Published,
    string OryginalLanguage,
    string CoverFileName
    ) : IRequest<Result<(Guid, string)>>;