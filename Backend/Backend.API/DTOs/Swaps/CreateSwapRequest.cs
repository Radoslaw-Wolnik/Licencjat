using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Swaps;

public sealed record CreateSwapRequest(
    [Required] Guid UserAcceptingId,
    [Required] Guid RequestedBookId
);
