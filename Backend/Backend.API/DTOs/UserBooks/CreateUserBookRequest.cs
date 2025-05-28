using System.ComponentModel.DataAnnotations;
using Backend.Domain.Enums;

namespace Backend.API.DTOs.UserBooks;

public sealed record CreateUserBookRequest(
    [Required] Guid BookId,
    [Required] BookStatus Status,
    [Required] BookState State,
    [Required] string Language,
    [Required] int PageCount,
    [Required] string CoverFileName
);