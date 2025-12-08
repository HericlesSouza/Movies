using MediatR;

using Movies.Application.Abstractions.Persistence;
using Movies.Application.DTOs;

namespace Movies.Application.Movies.Queries.ListMovies;

public class ListMoviesQueryHandler(IMovieRepository repository) : IRequestHandler<ListMoviesQuery, PagedResult<MovieDto>>
{
    readonly IMovieRepository _repository = repository;

    public async Task<PagedResult<MovieDto>> Handle(ListMoviesQuery request, CancellationToken ct)
    {
        var (movies, TotalCount) = await _repository.ListAsync(
            request.Page,
            request.PageSize,
            request.Search,
            request.SortBy,
            request.SortDirection,
            ct
            );

        var movieDtos = movies.Select(
            movie => new MovieDto(
                movie.Id,
                movie.Title,
                movie.Description,
                movie.DurationInMinutes,
                movie.Price,
                movie.CreatedAt,
                movie.UpdatedAt)
            ).ToList();

        return new PagedResult<MovieDto>(movieDtos, TotalCount, request.Page, request.PageSize);
    }
}
