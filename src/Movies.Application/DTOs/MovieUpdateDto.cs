namespace Movies.Application.DTOs;

public record MovieUpdateDto(
    string Title,
    string? Description,
    int DurationInMinues,
    decimal Price
    );
