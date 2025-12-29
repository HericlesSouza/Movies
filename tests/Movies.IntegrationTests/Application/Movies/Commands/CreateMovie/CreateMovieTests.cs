
using System.Net;
using System.Net.Http.Json;

using Microsoft.Extensions.DependencyInjection;

using Movies.Application.DTOs;
using Movies.Application.Movies.Commands.CreateMovie;
using Movies.Infrastructure.Persistence;
using Movies.IntegrationTests.Abstractions;

namespace Movies.IntegrationTests.Application.Movies.Commands.CreateMovie;

public class CreateMovieTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Create_ShouldPersistMovie_WhenRequestIsValid()
    {
        // Arrange
        var command = new CreateMovieCommand(
                Title: "Matrix",
                Description: "Ficção Científica",
                DurationInMinutes: 136,
                Price: 10m);

        // Act
        var createResponse = await Client.PostAsJsonAsync("/api/movies", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var responseDto = await createResponse.Content.ReadFromJsonAsync<MovieDto>();
        Assert.NotNull(responseDto);

        using var scope = ScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var movieDatabase = await context.Movies.FindAsync(responseDto.Id);

        Assert.NotNull(movieDatabase);
        Assert.Equal(command.Title, movieDatabase.Title);
        Assert.Equal(command.Description, movieDatabase.Description);
        Assert.Equal(command.DurationInMinutes, movieDatabase.DurationInMinutes);
        Assert.Equal(command.Price, movieDatabase.Price);

        Assert.InRange(movieDatabase.CreatedAt, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);
        Assert.InRange(movieDatabase.UpdatedAt, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);
    }

    [Theory]
    [MemberData(nameof(GetInvalidCommands))]
    public async Task Create_ShouldReturn400_WhenDataIsInvalid(CreateMovieCommand command)
    {
        // Act
        var response = await Client.PostAsJsonAsync("/api/movies", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public static IEnumerable<object[]> GetInvalidCommands()
    {
        yield return new object[]
        {
            new CreateMovieCommand("", "Description", 100, 10m)
        };

        yield return new object[]
        {
            new CreateMovieCommand(new string('a', 51), "Description", 100, 10m)
        };

        yield return new object[]
        {
            new CreateMovieCommand("Matrix", "Description", 0, 10m)
        };

        yield return new object[]
        {
            new CreateMovieCommand("Matrix", "Description", 100, 0m)
        };
    }
}
