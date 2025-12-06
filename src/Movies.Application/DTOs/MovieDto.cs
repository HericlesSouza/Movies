namespace Movies.Application.DTOs;

public record MovieDto(
    Guid Id,
    string Title,
    string? Description,
    int DurationInMinutes,
    decimal Price,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
