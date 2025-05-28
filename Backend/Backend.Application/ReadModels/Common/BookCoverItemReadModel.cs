namespace Backend.Application.ReadModels.Common;

public sealed record BookCoverItemReadModel(
    Guid Id,
    string Title,
    string CoverUrl
);