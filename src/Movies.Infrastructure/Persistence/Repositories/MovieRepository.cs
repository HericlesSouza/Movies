using Movies.Application.Abstractions.Persistence;
using Movies.Application.DTOs;
using Movies.Domain.Entities;

namespace Movies.Infrastructure.Persistence.Repositories;

public class MovieRepository(AppDbContext context) : IMovieRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Movie> AddAsync(Movie movie, CancellationToken ct = default)
    {
        await _context.Movies.AddAsync(movie, ct);
        return movie;
    }

    public Task DeleteAsync(Movie movie, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Movie>> ListAsync(MoviesListQuery query, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Movie> UpdateAsync(Movie movie, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
