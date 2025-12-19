using MediatR;

using Movies.Application.DTOs;

namespace Movies.Application.Movies.Commands.UpdateMovie;

public record UpdateMovieCommand(
    Guid Id,
    string Title,
    string? Description,
    int DurationInMinutes,
    decimal Price
    ) : IRequest<MovieDto?>;
