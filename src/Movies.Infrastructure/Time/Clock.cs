using Movies.Application.Abstractions.Time;

namespace Movies.Infrastructure.Time;

public class Clock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
