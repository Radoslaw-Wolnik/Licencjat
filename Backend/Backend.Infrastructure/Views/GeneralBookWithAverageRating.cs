namespace Backend.Infrastructure.Views;

public class GeneralBookWithAverageRating
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public decimal? AverageRating { get; set; } // decimal to match PostgreSQL's numeric type
}