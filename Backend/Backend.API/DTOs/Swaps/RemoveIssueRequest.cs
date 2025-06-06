using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Swaps;

public sealed record RemoveIssueRequest(
    [Required] Guid SwapId,
    [Required] Guid IssueId,
    [Required] string ResolutionDetails
);