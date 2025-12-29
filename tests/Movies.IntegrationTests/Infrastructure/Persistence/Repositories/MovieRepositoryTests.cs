using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Movies.Application.Abstractions.Persistence;
using Movies.Application.Common.Enum;
using Movies.Application.Movies.Queries.ListMovies;
using Movies.Domain.Entities;
using Movies.Infrastructure.Persistence;

namespace Movies.IntegrationTests.Infrastructure.Persistence.Repositories;

public class MovieRepositoryTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IServiceScope _scope;
    private readonly IMovieRepository _repository;
    private readonly AppDbContext _context;

    public MovieRepositoryTests(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        _repository = _scope.ServiceProvider.GetRequiredService<IMovieRepository>();
        _context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    // 2. Este método roda AUTOMATICAMENTE antes de CADA teste (e suporta await!)
    public async Task InitializeAsync()
    {
        // Aqui é o lugar seguro para limpar o banco
        await _context.Movies.ExecuteDeleteAsync();
    }

    public async Task DisposeAsync()
    {
        _scope.Dispose();
        await Task.CompletedTask;
    }

    [Fact]
    public async Task ListAsync_ShouldFilterByTitle_WhenSearchParameterIsProvided()
    {
        // Arrange
        var baseTime = DateTime.UtcNow;
        var movie1 = new Movie("Matrix", "Ficção Científica", 136, 10m, baseTime);
        var movie2 = new Movie("Matrix Reloaded", "Continuação", 138, 12m, baseTime);
        var movie3 = new Movie("O Senhor dos Anéis", "Fantasia", 178, 15m, baseTime);

        await _context.Movies.AddRangeAsync(movie1, movie2, movie3);
        await _context.SaveChangesAsync();

        var query = new ListMoviesQuery(default, default, "matrix");

        // Act
        var (items, totalCount) = await _repository.ListAsync(
            query.Page,
            query.PageSize,
            query.Search,
            query.SortBy,
            query.SortDirection,
            CancellationToken.None
            );

        // Assert 
        Assert.NotNull(items);
        Assert.Equal(2, totalCount);
        Assert.DoesNotContain(items, m => m.Title == "O Senhor dos Anéis");
    }

    [Theory]
    [InlineData("price", SortDirection.Asc, "Filme Barato", "Filme Caro")]
    [InlineData("price", SortDirection.Desc, "Filme Caro", "Filme Barato")]
    [InlineData("durationInMinutes", SortDirection.Asc, "Filme Barato", "Filme Caro")]
    [InlineData("durationInMinutes", SortDirection.Desc, "Filme Caro", "Filme Barato")]
    [InlineData("title", SortDirection.Asc, "A Força", "Zorro")]
    [InlineData("title", SortDirection.Desc, "Zorro", "A Força")]
    [InlineData("createdat", SortDirection.Asc, "Filme Barato", "Filme Caro")]
    [InlineData("createdat", SortDirection.Desc, "Filme Caro", "Filme Barato")]
    public async Task ListAsync_ShouldSortCorrectly_WhenParamsAreValid(
        string sortBy,
        SortDirection sortDirection,
        string expectedTitleOfFirstItem,
        string expectedTitleOfLastItem)
    {
        // Arrange
        var baseTime = DateTime.UtcNow;
        var movie1 = new Movie("Filme Barato", "Desc", 100, 10m, baseTime);
        var movie2 = new Movie("Zorro", "Desc", 150, 50m, baseTime.AddMinutes(1));
        var movie3 = new Movie("A Força", "Desc", 130, 100m, baseTime.AddMinutes(2));
        var movie4 = new Movie("Filme Caro", "Desc", 240, 200m, baseTime.AddMinutes(3));

        await _context.Movies.AddRangeAsync(movie1, movie2, movie3, movie4);
        await _context.SaveChangesAsync();

        var query = new ListMoviesQuery(
            Page: 1,
            PageSize: 10,
            Search: default,
            SortBy: sortBy,
            SortDirection: sortDirection
        );

        // Act
        var (items, totalCount) = await _repository.ListAsync(
            query.Page,
            query.PageSize,
            query.Search,
            query.SortBy,
            query.SortDirection,
            CancellationToken.None
            );

        // Assert
        Assert.NotEmpty(items);
        Assert.Equal(4, totalCount);
        Assert.Equal(expectedTitleOfFirstItem, items[0].Title);
        Assert.Equal(expectedTitleOfLastItem, items[^1].Title);
    }

    [Fact]
    public async Task ListAsync_ShouldPaginateCorrectly_WhenPageAndPageSizeAreValid()
    {
        // Arrange
        var baseTime = DateTime.UtcNow;
        var movies = new List<Movie>();

        // Criamos 15 filmes rapidamente
        for (var i = 1; i <= 15; i++)
        {
            movies.Add(new Movie($"Movie {i}", "Desc", 100, 10m, baseTime.AddMinutes(i)));
        }

        await _context.Movies.AddRangeAsync(movies);
        await _context.SaveChangesAsync();

        var query = new ListMoviesQuery(Page: 2, PageSize: 5);

        // Act
        var (items, totalCount) = await _repository.ListAsync(
            query.Page,
            query.PageSize,
            query.Search,
            query.SortBy,
            query.SortDirection,
            CancellationToken.None
            );

        // Assert
        Assert.NotEmpty(items);
        Assert.Equal(5, items.Count);
        Assert.Equal(15, totalCount);
    }
}
