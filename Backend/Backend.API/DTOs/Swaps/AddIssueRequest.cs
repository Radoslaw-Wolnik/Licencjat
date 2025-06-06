using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Swaps;

public sealed record AddIssueRequest(
    [Required] Guid SwapId,
    [Required] string Description
);
