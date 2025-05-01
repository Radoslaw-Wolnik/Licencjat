using System.ComponentModel.DataAnnotations;
using Backend.Domain.Enums;

namespace Backend.Infrastructure.Entities;

public class UserBookEntity
{
    public Guid Id { get; set; }
    
    // info
    [Required, MaxLength(5)]
    public string Language { get; set; } = null!;
    [Required]
    public int PageCount { get; set; }
    [Required]
    public string CoverPhoto { get; set; } = null!;

    // status & state
    public BookStatus Status { get; set; }
    public BookState State { get; set; }

    // references
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    [Required]
    public virtual UserEntity User { get; set; } = null!;
    [Required]
    public virtual GeneralBookEntity Book { get; set; } = null!;

    public virtual ICollection<SubSwapEntity> SubSwaps { get; set; } = [];
    public virtual ICollection<BookmarkEntity> Bookmarks { get; set; } = [];
}