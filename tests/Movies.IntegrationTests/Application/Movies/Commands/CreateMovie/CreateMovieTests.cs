
using System.Net;
using System.Net.Http.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Movies.Application.DTOs;
using Movies.Application.Movies.Commands.CreateMovie;
using Movies.Infrastructure.Persistence;

namespace Movies.IntegrationTests.Application.Movies.Commands.CreateMovie;

public class CreateMovieTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly IServiceScope _scope;
    private readonly AppDbContext _context;
    private readonly HttpClient _client;

    public CreateMovieTests(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _client = factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _scope.Dispose();
        await Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await _context.Movies.ExecuteDeleteAsync();
    }

    [Fact]
    public async Task Create_ShouldPersistAndRetrieveMovie_WhenRequestIsValid()
    {
        // Arrange
        var command = new CreateMovieCommand(
                Title: "Matrix",
                Description: "Ficção Científica",
                DurationInMinutes: 136,
                Price: 10m);

        var jsonString = JsonContent.Create(command);

        // Act (Create)
        var createResponse = await _client.PostAsync("/api/movies", jsonString);

        // Assert (Create)
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdMovie = await createResponse.Content.ReadFromJsonAsync<MovieDto>();

        Assert.NotNull(createdMovie);
        Assert.Equal(command.Title, createdMovie.Title);
        Assert.Equal(command.Description, createdMovie.Description);
        Assert.Equal(command.DurationInMinutes, createdMovie.DurationInMinutes);
        Assert.Equal(command.Price, createdMovie.Price);

        Assert.NotEqual(Guid.Empty, createdMovie.Id);
        Assert.True(createdMovie.CreatedAt > DateTime.UtcNow.AddSeconds(-30));
        Assert.True(createdMovie.UpdatedAt > DateTime.UtcNow.AddSeconds(-30));

        // Act (Retrieve)
        var getResponse = await _client.GetAsync($"/api/movies/{createdMovie.Id}");

        // Assert (Retrive)
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedMovie = await getResponse.Content.ReadFromJsonAsync<MovieDto>();
        Assert.NotNull(retrievedMovie);
        Assert.Equal(createdMovie.Id, retrievedMovie.Id);
        Assert.Equal(command.Title, retrievedMovie.Title);
        Assert.Equal(command.Description, retrievedMovie.Description);
        Assert.Equal(command.DurationInMinutes, retrievedMovie.DurationInMinutes);
        Assert.Equal(command.Price, retrievedMovie.Price);
    }

    [Theory]
    [MemberData(nameof(GetInvalidCommands))]
    public async Task Create_ShouldReturn400_WhenDataIsInvalid(CreateMovieCommand command)
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/movies", command);

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
