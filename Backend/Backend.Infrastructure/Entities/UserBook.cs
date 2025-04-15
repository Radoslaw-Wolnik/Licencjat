// Backend.Infrastructure/Entities/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Entities;

public class UserBook
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
    [Column(TypeName = "nvarchar(24)")]
    public BookStatus Status { get; set; }
    
    [Column(TypeName = "nvarchar(24)")]
    public BookState State { get; set; }

    // references
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    [Required]
    public virtual ApplicationUser User { get; set; } = null!;
    [Required]
    public virtual GeneralBook Book { get; set; } = null!;

    public virtual ICollection<SubSwap> SubSwaps { get; set; } = [];
}