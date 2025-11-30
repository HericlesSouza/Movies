namespace Movies.Domain.Entities;

public class Movie
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public int DurationInMinutes { get; private set; }
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; } 
    public DateTime UpdatedAt { get; private set; } 

    private Movie() { }

    public Movie(string title, string? description, int duration, decimal price)
    {
        SetTitle(title);
        SetDescription(description);
        SetDurationInMinutes(duration);
        SetPrice(price);

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.");

        if (title.Length > 50)
            throw new ArgumentException("Title cannot be longer than 50 characters.");

        Title = title;
    }

    private void SetDescription(string? description)
    {
        Description = description;
    }

    private void SetDurationInMinutes(int duration)
    {
        if (duration <= 0)
            throw new ArgumentException("The duration must be greater than zero.");

        DurationInMinutes = duration;
    }

    private void SetPrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");

        Price = price;
    }
}
