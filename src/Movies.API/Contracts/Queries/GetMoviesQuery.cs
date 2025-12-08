using Movies.Application.Common.Enum;

namespace Movies.API.Contracts.Queries;

public record GetMoviesQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    string? SortBy = null,
    SortDirection SortDirection = SortDirection.Asc
);

