using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Enums.SortBy;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.GeneralBooks;

public sealed record ListGeneralBooksQuerry(
    string? Title,        // search by
    string? Author,       // search by
    BookGenre? BookGenre, // search by
    // if none search by then we just list all books
    
    SortGeneralBookBy SortBy,
    bool Descending,
    int Offset,
    int Limit
    ) : IRequest<Result<PaginatedResult<GeneralBookListItem>>>;
