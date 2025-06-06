using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Swaps;

public sealed record AcceptSwapRequest(
    [Required] Guid SwapId,
    [Required] Guid RequestedBookId
);
