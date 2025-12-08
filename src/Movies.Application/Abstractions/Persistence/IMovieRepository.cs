using Movies.Application.Common.Enum;
using Movies.Domain.Entities;

namespace Movies.Application.Abstractions.Persistence;

public interface IMovieRepository
{
    public Task<Movie> AddAsync(Movie movie, CancellationToken ct = default);
    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken ct = default);
    public Task<Movie> UpdateAsync(Movie movie, CancellationToken ct = default);
    public Task DeleteAsync(Movie movie, CancellationToken ct = default);

    Task<(IReadOnlyList<Movie> Items, int TotalCount)> ListAsync(
    int page,
    int pageSize,
    string? search,
    string? sortBy,
    SortDirection sortDirection,
    CancellationToken ct = default);
}
