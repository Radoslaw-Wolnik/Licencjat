using Backend.Application.ReadModels.GeneralBooks;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.GeneralBooks;

public record GetReviewByIdQuery(
    Guid ReviewId) : IRequest<Result<ReviewReadModel>>;