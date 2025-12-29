using Moq;

using Movies.Application.Abstractions.Persistence;
using Movies.Application.Movies.Commands.DeleteMovie;
using Movies.Domain.Entities;

namespace Movies.UnitTests.Application.Movies.Commands.DeleteMovie;

public class DeleteMovieHandlerTests
{
    private readonly Mock<IMovieRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteMovieHandler _handler;

    public DeleteMovieHandlerTests()
    {
        _repositoryMock = new Mock<IMovieRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteMovieHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteMovie_WhenIdIsValid()
    {
        // Arrange
        var movie = new Movie(
                title: "Se7en",
                description: "Detetives caçam um assassino baseado nos sete pecados.",
                durationInMinutes: 127,
                price: 19.32m,
                createdAt: DateTime.UtcNow
                );

        var command = new DeleteMovieCommand(movie.Id);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
                ))
            .ReturnsAsync(movie);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);

        _repositoryMock
            .Verify(r => r.DeleteAsync(
                It.IsAny<Movie>(),
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
        var command = new DeleteMovieCommand(new Guid());

        _repositoryMock
            .Setup(r => r.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
                ))
            .Returns(Task.FromResult<Movie?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);

        _repositoryMock
            .Verify(r => r.DeleteAsync(
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
        var command = new DeleteMovieCommand(new Guid());

        _repositoryMock
            .Setup(r => r.GetByIdAsync(
                It.IsAny<Guid>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)
                ))
            .ThrowsAsync(new OperationCanceledException());

        var ct = new CancellationTokenSource();
        ct.Cancel();
        var canceledToken = ct.Token;

        await Assert.
                ThrowsAsync<OperationCanceledException>(() => _handler.Handle(command, canceledToken));

        _repositoryMock
            .Verify(r => r.DeleteAsync(
                It.IsAny<Movie>(),
                It.IsAny<CancellationToken>()
                ), Times.Never);

        _unitOfWorkMock
            .Verify(r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()
                ), Times.Never);
    }
}
