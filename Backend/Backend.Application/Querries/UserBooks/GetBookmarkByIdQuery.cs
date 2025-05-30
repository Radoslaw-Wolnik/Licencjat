using Backend.Application.ReadModels.UserBooks;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.UserBooks;

public sealed record GetBookmarkByIdQuery(
    Guid BookmarkId
) : IRequest<Result<BookmarkReadModel>>;