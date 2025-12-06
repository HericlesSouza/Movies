using Movies.Application.DTOs;
using Movies.Domain.Entities;

namespace Movies.Application.Abstractions.Persistence;

public interface IMovieRepository
{
    public Task<Movie> AddAsync(Movie movie, CancellationToken ct = default);
    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken ct = default);
    public Task<List<Movie>> ListAsync(MoviesListQuery query, CancellationToken ct = default);
    public Task<Movie> UpdateAsync(Movie movie, CancellationToken ct = default);
    public Task DeleteAsync(Movie movie, CancellationToken ct = default);

}
