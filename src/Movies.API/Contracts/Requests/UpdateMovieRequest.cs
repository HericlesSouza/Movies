namespace Movies.API.Contracts.Requests;

public record UpdateMovieRequest(
    string Title,
    string? Description,
    int DurationInMinutes,
    decimal Price
    );
