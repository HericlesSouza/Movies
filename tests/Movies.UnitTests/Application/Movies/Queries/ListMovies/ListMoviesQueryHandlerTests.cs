using Moq;

using Movies.Application.Abstractions.Persistence;
using Movies.Application.Common.Enum;
using Movies.Application.Movies.Queries.ListMovies;
using Movies.Domain.Entities;

namespace Movies.UnitTests.Application.Movies.Queries.ListMovies;

public class ListMoviesQueryHandlerTests
{
    private readonly Mock<IMovieRepository> _movieRepositoryMock;
    private readonly ListMoviesQueryHandler _handler;

    public ListMoviesQueryHandlerTests()
    {
        _movieRepositoryMock = new Mock<IMovieRepository>();
        _handler = new ListMoviesQueryHandler(_movieRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedResult_WhenQueryIsValid()
    {
        // Arrange
        var query = new ListMoviesQuery(10, 2);
        var movies = new List<Movie>
        {
            new("Old Title", "Old Description", 100, 5.99m),
            new("Another Title", "Another Description", 120, 7.99m)
        };

        _movieRepositoryMock
            .Setup(m => m.ListAsync(
                It.Is<int>(p => p == query.Page),
                It.Is<int>(p => p == query.PageSize),
                It.Is<string>(p => p == query.Search),
                It.Is<string>(p => p == query.SortBy),
                It.Is<SortDirection>(p => p == query.SortDirection),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync((movies, movies.Count));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(movies.First().Id, result.Items.First().Id);
        Assert.Equal(movies.First().Title, result.Items.First().Title);
        Assert.Equal(movies.First().Description, result.Items.First().Description);
        Assert.Equal(movies.First().DurationInMinutes, result.Items.First().DurationInMinutes);
        Assert.Equal(movies.First().Price, result.Items.First().Price);
        Assert.Equal(movies.Count, result.TotalCount);
        Assert.Equal(movies.Count, result.Items.Count);
        Assert.Equal(query.Page, result.Page);
        Assert.Equal(query.PageSize, result.PageSize);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedResultWithEmptyList_WhenNoFoundDataInDatabase()
    {
        // Arrange
        var query = new ListMoviesQuery(10, 2);

        _movieRepositoryMock
            .Setup(m => m.ListAsync(
                It.Is<int>(p => p == query.Page),
                It.Is<int>(p => p == query.PageSize),
                It.Is<string>(p => p == query.Search),
                It.Is<string>(p => p == query.SortBy),
                It.Is<SortDirection>(p => p == query.SortDirection),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync((new List<Movie>(), 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(query.Page, result.Page);
        Assert.Equal(query.PageSize, result.PageSize);
    }

    [Fact]
    public async Task Handle_ShouldThrowOperationCanceledException_WhenTokenIsCanceled()
    {
        // Arrange
        var query = new ListMoviesQuery();

        _movieRepositoryMock
            .Setup(m => m.ListAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<SortDirection>(),
                It.IsAny<CancellationToken>()
            )).ThrowsAsync(new OperationCanceledException());

        var cts = new CancellationTokenSource();
        cts.Cancel();
        var canceledToken = cts.Token;

        // Act & Assert
        await Assert
                .ThrowsAsync<OperationCanceledException>(() => _handler.Handle(query, canceledToken));
    }
}
