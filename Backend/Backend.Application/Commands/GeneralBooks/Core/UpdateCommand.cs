using Backend.Domain.Entities;
using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Core;

public sealed record UpdateGeneralBookCommand(
    Guid BookId,
    string? Title,
    string? Author,
    DateOnly? Published,
    string? OryginalLanguage,
    IEnumerable<BookGenre>? NewBookGenres
    ) : IRequest<Result<GeneralBook>>;