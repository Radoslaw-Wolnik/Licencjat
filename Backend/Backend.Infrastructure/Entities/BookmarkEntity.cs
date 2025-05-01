using System.ComponentModel.DataAnnotations;
using Backend.Domain.Enums;

namespace Backend.Infrastructure.Entities;

public class BookmarkEntity
{
    public Guid Id { get; set; }

    public BookmarkColours Colour { get; set; }
    public int Page { get; set; }
    public string? Description { get; set; }

    // references FK
    public Guid UserBookId { get; set; }

    [Required]
    public virtual UserBookEntity UserBook { get; set; } = null!;
}
