using System.Net;
using System.Net.Http.Json;

using Microsoft.Extensions.DependencyInjection;

using Movies.Application.DTOs;
using Movies.Domain.Entities;
using Movies.Infrastructure.Persistence;
using Movies.IntegrationTests.Abstractions;

namespace Movies.IntegrationTests.Application.Movies.Queries.ListMovies;

public class ListMoviesTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAll_ShouldReturnPagedResult_WhenParamsAreValid()
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

        // Act        
        var response = await Client.GetAsync("/api/movies?page=2&pageSize=5&sortBy=createdat&sortDirection=desc");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<MovieDto>>();

        Assert.NotNull(result);
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(5, result.Items.Count);
        Assert.Equal(15, result.TotalCount);

        Assert.Contains(result.Items, m => m.Title == "Movie 10");
        Assert.DoesNotContain(result.Items, m => m.Title == "Movie 1");
        Assert.DoesNotContain(result.Items, m => m.Title == "Movie 15");
    }

    [Theory]
    [InlineData(0, 10, "title", "Page cannot be zero")]
    [InlineData(-1, 10, "title", "Page cannot be negative")]
    [InlineData(1, 0, "title", "PageSize cannot be zero")]
    [InlineData(1, 51, "title", "PageSize cannot exceed 50")]
    [InlineData(1, 10, "diretor", "SortBy column does not exist")]
    public async Task GetAll_ShouldReturnBadRequest_WhenParamsAreInvalid(
        int page,
        int pageSize,
        string sortBy,
        string reason)
    {
        // Act        
        var response = await Client.GetAsync($"/api/movies?page={page}&pageSize={pageSize}&sortBy={sortBy}");

        // Assert        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
