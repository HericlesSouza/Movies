using MediatR;

using Movies.Application.Common.Enum;
using Movies.Application.DTOs;

namespace Movies.Application.Movies.Queries.ListMovies;

public record ListMoviesQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    string? SortBy = null,
    SortDirection SortDirection = SortDirection.Asc
) : IRequest<PagedResult<MovieDto>>;
