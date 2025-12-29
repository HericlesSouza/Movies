using Moq;

using Movies.Application.Abstractions.Persistence;
using Movies.Application.Abstractions.Time;
using Movies.Application.Movies.Commands.CreateMovie;
using Movies.Domain.Entities;
namespace Movies.UnitTests.Application.Movies.Commands.CreateMovie;

public class CreateMovieHandlerTests
{
    private readonly Mock<IMovieRepository> _movieRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IClock> _clock;
    private readonly CreateMovieHandler _handler;

    public CreateMovieHandlerTests()
    {
        _movieRepositoryMock = new Mock<IMovieRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _clock = new Mock<IClock>();

        _handler = new CreateMovieHandler(_movieRepositoryMock.Object, _unitOfWorkMock.Object, _clock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMovieDto_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateMovieCommand(
            "Inception",
            "A mind-bending thriller",
            148,
            9.99m);

        var fixedDate = new DateTime(2025, 01, 01, 12, 0, 0, DateTimeKind.Utc);
        _clock.Setup(c => c.UtcNow).Returns(fixedDate);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _movieRepositoryMock.Verify(r => r.AddAsync(
            It.Is<Movie>(m =>
                m.Title == command.Title &&
                m.Description == command.Description &&
                m.DurationInMinutes == command.DurationInMinutes &&
                m.Price == command.Price),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(result);
        Assert.Equal(command.Title, result.Title);
        Assert.Equal(command.Description, result.Description);
        Assert.Equal(command.DurationInMinutes, result.DurationInMinutes);
        Assert.Equal(command.Price, result.Price);
        Assert.NotEqual(default, result.Id);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.NotEqual(default, result.UpdatedAt);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new CreateMovieCommand(
            "",
            "Descrição",
            -120,
            -10m);

        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenSaveChangesFailed()
    {
        // Arrange
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection error"));

        var command = new CreateMovieCommand("Titulo", "Desc", 120, 10);

        // Act & Assert
        await Assert
                .ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowOperationCanceledException_WhenTokenIsCanceled()
    {
        // Arrange
        var command = new CreateMovieCommand("Titulo", "Desc", 120, 10);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var cts = new CancellationTokenSource();
        cts.Cancel();
        var canceledToken = cts.Token;

        await Assert
                .ThrowsAsync<OperationCanceledException>(() => _handler.Handle(command, canceledToken));
    }
}
