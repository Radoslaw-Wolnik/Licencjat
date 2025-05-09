using Backend.Domain.Common;
using Backend.Domain.Entities;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Core;

public sealed record CreateCommand(
    string Title,
    string Author,
    DateOnly Published,
    string OryginalLanguage,
    string CoverFileName
    ) : IRequest<Result<(Guid, string)>>;