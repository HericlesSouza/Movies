using Movies.Application.Common.Enum;

namespace Movies.Application.DTOs;

public record MoviesListQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    string? SortBy = null,
    SortDirection SortDirection = SortDirection.Asc
);
