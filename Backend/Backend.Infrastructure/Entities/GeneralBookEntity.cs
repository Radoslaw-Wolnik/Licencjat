using System.ComponentModel.DataAnnotations;
using Backend.Domain.Enums;

namespace Backend.Infrastructure.Entities;

public class GeneralBookEntity
{
    public Guid Id { get; set; }
    // info
    [Required]
    public string Title { get; set; } = null!;
    [Required]
    public string Author { get; set; } = null!;
    [Required]
    public DateOnly Published { get; set; }
    [Required, MaxLength(5)]
    public string Language { get; set; } = null!; // oryginal language when published
    [Required]
    public string CoverPhoto { get; set; } = null!;


    public virtual IList<BookGenre> Genres { get; set; } = [];

    // references
    public virtual ICollection<UserBookEntity> UserBooks { get; set; } = [];
    public virtual ICollection<ReviewEntity> Reviews { get; set; } = [];
    public virtual ICollection<UserEntity> WishlistedByUsers { get; set; } = [];
}
