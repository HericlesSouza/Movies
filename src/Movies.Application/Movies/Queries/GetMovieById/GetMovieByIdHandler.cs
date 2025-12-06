using MediatR;

using Movies.Application.Abstractions.Persistence;
using Movies.Application.DTOs;

namespace Movies.Application.Movies.Queries.GetMovieById;

public class GetMovieByIdHandler(
    IMovieRepository repository
    ) : IRequestHandler<GetMovieByIdQuery, MovieDto?>
{
    readonly IMovieRepository _repository = repository;

    public async Task<MovieDto?> Handle(GetMovieByIdQuery request, CancellationToken ct)
    {
        var movie = await _repository.GetByIdAsync(request.Id, ct);

        if (movie is null)
        {
            return null;
        }

        return new MovieDto(
            movie.Id,
            movie.Title,
            movie.Description,
            movie.DurationInMinutes,
            movie.Price,
            movie.CreatedAt,
            movie.UpdatedAt
        );
    }
}
