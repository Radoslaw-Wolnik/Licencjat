using Backend.Domain.Common;
using Backend.Domain.Entities;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Core;

public sealed record CreateCommand(
    string Title,
    string Author,
    DateOnly Published,
    LanguageCode OryginalLanguage,
    Photo CoverPhoto
    ) : IRequest<Result<GeneralBook>>;