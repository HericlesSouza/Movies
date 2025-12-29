using System.Net;
using System.Net.Http.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Movies.API.Contracts.Requests;
using Movies.Domain.Entities;
using Movies.Infrastructure.Persistence;
using Movies.IntegrationTests.Abstractions;

namespace Movies.IntegrationTests.Application.Movies.Commands.UpdateMovie;

public class UpdateMovieTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Update_ShouldUpdateDatabase_WhenMovieExists()
    {
        // Arrange
        var movie = new Movie("Matrix", "Ficção Científica", 136, 10m, DateTime.UtcNow);

        using (var scope = ScopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Movies.AddAsync(movie);
            await context.SaveChangesAsync();
        }

        var payload = new UpdateMovieRequest("Matrix 2", "Ficção Científica", 120, 15m);

        // Act
        var response = await Client.PutAsJsonAsync($"api/movies/{movie.Id}", payload);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var scope = ScopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var movieDatabase = await context.Movies.FirstOrDefaultAsync(m => m.Id == movie.Id);

            Assert.NotNull(movieDatabase);
            Assert.Equal(payload.Title, movieDatabase.Title);
            Assert.Equal(payload.Description, movieDatabase.Description);
            Assert.Equal(payload.DurationInMinutes, movieDatabase.DurationInMinutes);
            Assert.Equal(payload.Price, movieDatabase.Price);

            Assert.InRange(movieDatabase.CreatedAt, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);
            Assert.InRange(movieDatabase.UpdatedAt, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);
        }
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenMovieDoesNotExist()
    {
        // Arrange
        var payload = new UpdateMovieRequest("Matrix 2", "Ficção Científica", 120, 15m);

        // Act
        var response = await Client.PutAsJsonAsync($"api/movies/{Guid.NewGuid()}", payload);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(GetInvalidUpdateRequests))]
    public async Task Update_ShouldReturnBadRequest_WhenDataIsInvalid(UpdateMovieRequest request)
    {
        // Act        
        var response = await Client.PutAsJsonAsync($"api/movies/{Guid.NewGuid()}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public static IEnumerable<object[]> GetInvalidUpdateRequests()
    {
        yield return new object[]
        {
            new UpdateMovieRequest("", "Description", 100, 10m)
        };

        yield return new object[]
        {
            new UpdateMovieRequest("Matrix", "Description", 0, 10m)
        };

        yield return new object[]
        {
            new UpdateMovieRequest("Matrix", "Description", 100, -10m)
        };
    }
}
