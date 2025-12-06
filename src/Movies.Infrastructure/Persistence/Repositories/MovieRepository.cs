using Microsoft.EntityFrameworkCore;

using Movies.Application.Abstractions.Persistence;
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

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Movies.FirstOrDefaultAsync(m => m.Id == id, ct);
    }

    //public Task<List<Movie>> ListAsync(MoviesListQuery query, CancellationToken ct = default)
    //{
    //    throw new NotImplementedException();
    //}

    public Task<Movie> UpdateAsync(Movie movie, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
