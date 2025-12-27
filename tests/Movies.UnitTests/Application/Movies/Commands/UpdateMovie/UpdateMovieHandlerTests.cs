using Moq;

using Movies.Application.Abstractions.Persistence;
using Movies.Application.Abstractions.Time;
using Movies.Application.Movies.Commands.UpdateMovie;
using Movies.Domain.Entities;

namespace Movies.UnitTests.Application.Movies.Commands.UpdateMovie;

public class UpdateMovieHandlerTests
{
    private readonly Mock<IMovieRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IClock> _clockMock;
    private readonly UpdateMovieHandler _handler;

    public UpdateMovieHandlerTests()
    {
        _repositoryMock = new Mock<IMovieRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _clockMock = new Mock<IClock>();
        _handler = new UpdateMovieHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _clockMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateMovie_WhenIdIsValid()
    {
        // Assert
        var movie = new Movie(
                title: "Se7en",
                description: "Detetives caçam um assassino baseado nos sete pecados.",
                durationInMinutes: 127,
                price: 19.32m
                );

        var command = new UpdateMovieCommand(
                Id: movie.Id,
                Title: "Title",
                Description: "Description",
                DurationInMinutes: 140,
                Price: 25.32m
            );

        var futureTime = DateTime.UtcNow.AddHours(2);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
                ))
            .ReturnsAsync(movie);

        _clockMock
            .Setup(c => c.UtcNow)
            .Returns(futureTime);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Title, result.Title);
        Assert.Equal(command.Description, result.Description);
        Assert.Equal(command.DurationInMinutes, result.DurationInMinutes);
        Assert.Equal(command.Price, result.Price);
        Assert.Equal(futureTime, result.UpdatedAt);

        _repositoryMock
            .Verify(r => r.UpdateAsync(
                It.Is<Movie>(m => m.Id == command.Id && m.Title == command.Title),
                It.IsAny<CancellationToken>()
                ), Times.Once);

        _unitOfWorkMock
            .Verify(r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()
                ), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenIdIsInvalid()
    {
        // Arrange
        var command = new UpdateMovieCommand(
                Id: new Guid(),
                Title: "Title",
                Description: "Description",
                DurationInMinutes: 140,
                Price: 25.32m
            );

        _repositoryMock
            .Setup(r => r.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
                ))
            .Returns(Task.FromResult<Movie?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);

        _repositoryMock
            .Verify(r => r.UpdateAsync(
                It.IsAny<Movie>(),
                It.IsAny<CancellationToken>()
                ), Times.Never);

        _unitOfWorkMock
            .Verify(r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()
                ), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowOperationCanceledException_WhenTokenIsCanceled()
    {
        // Arrange
        var command = new UpdateMovieCommand(
                Id: new Guid(),
                Title: "Title",
                Description: "Description",
                DurationInMinutes: 140,
                Price: 25.32m
            );

        _repositoryMock
            .Setup(r => r.GetByIdAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()
            ))
            .ThrowsAsync(new OperationCanceledException());

        var cts = new CancellationTokenSource();
        cts.Cancel();
        var canceledToken = cts.Token;

        // Act & Assert
        await Assert
                .ThrowsAsync<OperationCanceledException>(() => _handler.Handle(command, canceledToken));

        _repositoryMock
            .Verify(r => r.UpdateAsync(
                It.IsAny<Movie>(),
                It.IsAny<CancellationToken>()
                ), Times.Never);

        _unitOfWorkMock
            .Verify(r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()
                ), Times.Never);
    }
}
