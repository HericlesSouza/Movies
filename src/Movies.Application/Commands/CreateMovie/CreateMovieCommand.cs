using MediatR;

using Movies.Application.DTOs;

namespace Movies.Application.Commands.CreateMovie;

public record CreateMovieCommand(
    string Title,
    string? Description,
    int DurationInMinutes,
    decimal Price
    ) : IRequest<MovieDto>;
