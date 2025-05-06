using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Core;

public sealed record UpdateCommand(
    Guid BookId,
    string? Title,
    string? Author,
    DateOnly? Published,
    LanguageCode? OryginalLanguage,
    Photo? CoverPhoto,
    List<BookGenre>? BookGenres
    ) : IRequest<Result<GeneralBook>>;