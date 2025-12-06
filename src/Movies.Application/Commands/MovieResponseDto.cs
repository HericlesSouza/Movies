namespace Movies.Application.DTOs;

public record MovieResponseDto(
    Guid Id,
    string Title,
    string? Description,
    int DurationInMinutes,
    decimal Price,
    DateTime CreatedAt,
    DateTime UpdatedAt
    );
