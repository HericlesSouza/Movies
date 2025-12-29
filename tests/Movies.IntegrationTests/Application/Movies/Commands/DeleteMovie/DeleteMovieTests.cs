using System.Net;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Movies.Domain.Entities;
using Movies.Infrastructure.Persistence;
using Movies.IntegrationTests.Abstractions;

namespace Movies.IntegrationTests.Application.Movies.Commands.DeleteMovie;

public class DeleteMovieTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Delete_ShouldRemoveFromDatabase_WhenIdExists()
    {
        // Arrange
        var movie = new Movie("Matrix", "Ficção Científica", 136, 10m, DateTime.UtcNow);

        using (var scope = ScopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Movies.AddAsync(movie);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await Client.DeleteAsync($"api/movies/{movie.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using (var scope = ScopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var movieDatabase = await context.Movies.FirstOrDefaultAsync(m => m.Id == movie.Id);

            Assert.Null(movieDatabase);
        }
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenMovieDoesNotExist()
    {
        // Act
        var response = await Client.DeleteAsync($"api/movies/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
