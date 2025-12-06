namespace Movies.Application.DTOs;

public record MovieCreateDto(
    string Title,
    string? Description,
    int DurationInMinues,
    decimal Price
    );
