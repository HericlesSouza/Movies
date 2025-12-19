using Microsoft.EntityFrameworkCore;

using Movies.Application.Abstractions.Persistence;
using Movies.Application.Common.Enum;
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
        _context.Movies.Remove(movie);
        return Task.CompletedTask;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Movies.FirstOrDefaultAsync(m => m.Id == id, ct);
    }

    public Task<Movie> UpdateAsync(Movie movie, CancellationToken ct = default)
    {
        _context.Movies.Update(movie);
        return Task.FromResult(movie);
    }

    public async Task<(IReadOnlyList<Movie> Items, int TotalCount)> ListAsync(
        int page,
        int pageSize,
        string? search,
        string? sortBy,
        SortDirection sortDirection,
        CancellationToken ct = default)
    {
        var query = _context.Movies
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(m => m.Title.Contains(search, StringComparison.OrdinalIgnoreCase));

        var totalCount = await query.CountAsync(ct);

        query = (sortBy?.ToLower(), sortDirection) switch
        {
            ("title", SortDirection.Asc) => query.OrderBy(m => m.Title),
            ("title", SortDirection.Desc) => query.OrderByDescending(m => m.Title),

            ("durationinminutes", SortDirection.Asc) => query.OrderBy(m => m.DurationInMinutes),
            ("durationinminutes", SortDirection.Desc) => query.OrderByDescending(m => m.DurationInMinutes),

            ("price", SortDirection.Asc) => query.OrderBy(m => m.Price),
            ("price", SortDirection.Desc) => query.OrderByDescending(m => m.Price),

            ("createdat", SortDirection.Asc) => query.OrderBy(m => m.CreatedAt),
            ("createdat", SortDirection.Desc) => query.OrderByDescending(m => m.CreatedAt),

            (null, SortDirection.Desc) => query.OrderByDescending(m => m.CreatedAt),

            _ => query.OrderBy(m => m.CreatedAt)
        };

        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 10;

        var skip = (page - 1) * pageSize;

        var items = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
