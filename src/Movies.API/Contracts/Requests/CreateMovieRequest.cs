namespace Movies.API.Contracts.Requests;

public record CreateMovieRequest(
    string Title,
    string? Description,
    int DurationInMinutes,
    decimal Price
    );
