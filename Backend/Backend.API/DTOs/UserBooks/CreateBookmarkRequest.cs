using System.ComponentModel.DataAnnotations;
using Backend.Domain.Enums;

namespace Backend.API.DTOs.UserBooks;

public sealed record CreateBookmarkRequest(
    [Required] BookmarkColours Colour,
    [Required] int Page,
    string? Description
);
