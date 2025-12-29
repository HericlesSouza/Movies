using Moq;

using Movies.Application.Abstractions.Persistence;
using Movies.Application.Movies.Queries.GetMovieById;
using Movies.Domain.Entities;

namespace Movies.UnitTests.Application.Movies.Queries.GetMovieById;

public class GetMovieByIdHandlerTests
{
    private readonly Mock<IMovieRepository> _movieRepositoryMock;
    private readonly GetMovieByIdHandler _handler;

    public GetMovieByIdHandlerTests()
    {
        _movieRepositoryMock = new Mock<IMovieRepository>();
        _handler = new GetMovieByIdHandler(_movieRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMovieDto_WhenIdIsValid()
    {
        // Arrange
        var query = new GetMovieByIdQuery(new Guid());
        var movie = new Movie(
            title: "Se7en",
            description: "Detetives caçam um assassino baseado nos sete pecados.",
            durationInMinutes: 127,
            price: 19.32m,
            createdAt: DateTime.UtcNow
            );


        _movieRepositoryMock
            .Setup(r => r.GetByIdAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(movie);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(movie.Id, result.Id);
        Assert.Equal(movie.Title, result.Title);
        Assert.Equal(movie.Description, result.Description);
        Assert.Equal(movie.DurationInMinutes, result.DurationInMinutes);
        Assert.Equal(movie.Price, result.Price);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenIdIsInvalid()
    {
        // Arrange
        var query = new GetMovieByIdQuery(new Guid());

        _movieRepositoryMock
            .Setup(r => r.GetByIdAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()
            ))
            .Returns(Task.FromResult<Movie?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task Handle_ShouldThrowOperationCanceledException_WhenTokenIsCanceled()
    {
        // Arrange
        var query = new GetMovieByIdQuery(new Guid());


        _movieRepositoryMock
            .Setup(r => r.GetByIdAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()
            ))
            .ThrowsAsync(new OperationCanceledException());

        var cts = new CancellationTokenSource();
        cts.Cancel();
        var canceledToken = cts.Token;

        // Act & Assert
        await Assert
                .ThrowsAsync<OperationCanceledException>(() => _handler.Handle(query, canceledToken));
    }
}
