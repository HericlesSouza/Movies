using Movies.Domain.Entities;

namespace Movies.UnitTests.Domain.Entities;

public class MovieTests
{
    public static IEnumerable<object?[]> InvalidTitles =>
        [
            [""],
            [null!],
            [new string('A', 51)]
        ];

    [Fact]
    public void Create_WhenParamsAreValid_ShouldReturnMovie()
    {
        // Arrange
        var title = "Inception";
        var description = "A mind-bending thriller";
        var durationInMinutes = 148;
        var price = 9.99m;

        // Act
        var movie = new Movie(
            title,
            description,
            durationInMinutes,
            price,
            DateTime.UtcNow);

        // Assert
        Assert.Equal(title, movie.Title);
        Assert.Equal(description, movie.Description);
        Assert.Equal(durationInMinutes, movie.DurationInMinutes);
        Assert.Equal(price, movie.Price);
    }

    [Theory]
    [MemberData(nameof(InvalidTitles))]
    public void Create_WhenTitleIsInvalid_ShouldThrowException(string title)
    {
        // Arrange
        var description = "Description";
        var duration = 120;
        var price = 10m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Movie(
            title,
            description,
            duration,
            price,
            DateTime.UtcNow));
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(-0)]
    public void Create_WhenDurationIsInvalid_ShouldThrowException(int duration)
    {
        // Arrange
        var title = "Movie Title";
        var description = "Description";
        var price = 10m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Movie(
            title,
            description,
            duration,
            price,
            DateTime.UtcNow));
    }

    [Theory]
    [InlineData(-10)]
    public void Create_WhenPriceIsInvalid_ShouldThrowException(decimal price)
    {
        // Arrange
        var title = "Movie Title";
        var description = "Description";
        var duration = 120;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Movie(
            title,
            description,
            duration,
            price,
            DateTime.UtcNow));
    }

    [Fact]
    public void Update_WhenParamsAreValid_ShouldUpdateMovie()
    {
        // Arrange
        var movie = new Movie(
            "Old Title",
            "Old Description",
            100,
            5.99m,
            DateTime.UtcNow);

        var newTitle = "New Title";
        var newDescription = "New Description";
        var newDuration = 150;
        var newPrice = 12.99m;
        var newUpdatedAt = DateTime.UtcNow.AddMinutes(1);

        // Act
        movie.Update(
            newTitle,
            newDescription,
            newDuration,
            newPrice,
            newUpdatedAt);

        // Assert
        Assert.Equal(newTitle, movie.Title);
        Assert.Equal(newDescription, movie.Description);
        Assert.Equal(newDuration, movie.DurationInMinutes);
        Assert.Equal(newPrice, movie.Price);
        Assert.Equal(newUpdatedAt, movie.UpdatedAt);
    }
}
