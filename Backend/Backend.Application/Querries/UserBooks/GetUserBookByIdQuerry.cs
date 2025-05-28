using Backend.Application.ReadModels.UserBooks;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.UserBooks;

public sealed record GetUserBookByIdQuerry(
    Guid UserBookId
    ) : IRequest<Result<UserBookDetailsReadModel?>>;