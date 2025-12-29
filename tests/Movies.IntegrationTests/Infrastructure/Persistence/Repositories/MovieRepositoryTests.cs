using Microsoft.Extensions.DependencyInjection;

using Movies.Application.Abstractions.Persistence;
using Movies.Application.Common.Enum;
using Movies.Application.Movies.Queries.ListMovies;
using Movies.Domain.Entities;
using Movies.Infrastructure.Persistence;
using Movies.IntegrationTests.Abstractions;

namespace Movies.IntegrationTests.Infrastructure.Persistence.Repositories;

public class MovieRepositoryTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ListAsync_ShouldFilterByTitle_WhenSearchParameterIsProvided()
    {
        // Arrange
        var baseTime = DateTime.UtcNow;
        var movie1 = new Movie("Matrix", "Ficção Científica", 136, 10m, baseTime);
        var movie2 = new Movie("Matrix Reloaded", "Continuação", 138, 12m, baseTime);
        var movie3 = new Movie("O Senhor dos Anéis", "Fantasia", 178, 15m, baseTime);

        using (var scope = ScopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Movies.AddRangeAsync(movie1, movie2, movie3);
            await context.SaveChangesAsync();
        }

        var query = new ListMoviesQuery(default, default, "matrix");

        // Act
        using (var scope = ScopeFactory.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IMovieRepository>();

            var (items, totalCount) = await repository.ListAsync(
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

        using (var scope = ScopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Movies.AddRangeAsync(movie1, movie2, movie3, movie4);
            await context.SaveChangesAsync();
        }

        var query = new ListMoviesQuery(
            Page: 1,
            PageSize: 10,
            Search: default,
            SortBy: sortBy,
            SortDirection: sortDirection
        );

        // Act
        using (var scope = ScopeFactory.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IMovieRepository>();

            var (items, totalCount) = await repository.ListAsync(
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
    }

    [Fact]
    public async Task ListAsync_ShouldPaginateCorrectly_WhenPageAndPageSizeAreValid()
    {
        // Arrange
        var baseTime = DateTime.UtcNow;
        var movies = new List<Movie>();

        for (var i = 1; i <= 15; i++)
        {
            movies.Add(new Movie($"Movie {i}", "Desc", 100, 10m, baseTime.AddMinutes(i)));
        }

        using (var scope = ScopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Movies.AddRangeAsync(movies);
            await context.SaveChangesAsync();
        }

        var query = new ListMoviesQuery(Page: 2, PageSize: 5);

        // Act
        using (var scope = ScopeFactory.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IMovieRepository>();

            var (items, totalCount) = await repository.ListAsync(
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
}
