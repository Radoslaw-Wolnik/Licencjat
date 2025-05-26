using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Entities;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.GeneralBooks;

// data class GetBookByIdQuerry(val bookId: UUID)

public sealed record GetGeneralBookByIdQuerry(
    Guid BookId
    ) : IRequest<Result<GeneralBookDetailsReadModel>>;