using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Swaps;

public sealed record UpdateSwapRequest(
    [Required] Guid SwapId,
    [Range(1, int.MaxValue)] int PageAt
);
