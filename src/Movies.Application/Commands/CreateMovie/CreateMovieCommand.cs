namespace Movies.Application.Commands.CreateMovie;

public record CreateMovieCommand(
    string Title,
    string? Description,
    int DurationInMinues,
    decimal Price
    );
