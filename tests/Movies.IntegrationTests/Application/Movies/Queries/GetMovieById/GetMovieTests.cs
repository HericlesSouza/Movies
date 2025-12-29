using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Movies.Application.DTOs;
using Movies.Domain.Entities;
using Movies.Infrastructure.Persistence;
using Movies.IntegrationTests.Abstractions;

namespace Movies.IntegrationTests.Application.Movies.Queries.GetMovieById;

public class GetMovieTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Get_ShouldReturnMovie_WhenIdExists()
    {
        // Arrange
        using var scope = ScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var movie = new Movie("Matrix", "Ficção Científica", 136, 10m, DateTime.UtcNow);

        await context.Movies.AddAsync(movie);
        await context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/movies/{movie.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseDto = await response.Content.ReadFromJsonAsync<MovieDto>();

        Assert.NotNull(responseDto);
        Assert.Equal(movie.Title, responseDto.Title);
        Assert.Equal(movie.Description, responseDto.Description);
        Assert.Equal(movie.DurationInMinutes, responseDto.DurationInMinutes);
        Assert.Equal(movie.Price, responseDto.Price);

        Assert.Equal(movie.Id, responseDto.Id);
        Assert.InRange(responseDto.CreatedAt, movie.CreatedAt.AddSeconds(-1), movie.CreatedAt.AddSeconds(1));
        Assert.InRange(responseDto.UpdatedAt, movie.UpdatedAt.AddSeconds(-1), movie.UpdatedAt.AddSeconds(1));
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenIdDoesNotExist()
    {
        // Act
        var response = await Client.GetAsync($"/api/movies/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
