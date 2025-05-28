namespace Backend.API.DTOs.Swaps.Responses;

public sealed record IssueResponse(
    Guid Id,
    Guid SwapId,
    string Description,
    DateTime ReportedAt,
    string? ResolutionDetails
);