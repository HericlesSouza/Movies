namespace Movies.Application.Movies.Commands;

public record MovieUpdateDto(
    string Title,
    string? Description,
    int DurationInMinues,
    decimal Price
    );
