using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Users;

public sealed record WishlistBookRequest(
    [Required] Guid BookId
);
