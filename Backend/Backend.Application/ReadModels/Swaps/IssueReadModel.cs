namespace Backend.Application.ReadModels.Swaps;

public sealed record IssueReadModel(
    Guid Id,
    Guid SwapId,
    string Description,
    DateTime ReportedAt,
    string? ResolutionDetails
);