namespace Movies.Application.Commands;

public record MovieUpdateDto(
    string Title,
    string? Description,
    int DurationInMinues,
    decimal Price
    );
