using MediatR;

using Movies.Application.DTOs;

namespace Movies.Application.Movies.Queries.GetMovieById;

public record GetMovieByIdQuery(Guid Id) : IRequest<MovieDto?>;
