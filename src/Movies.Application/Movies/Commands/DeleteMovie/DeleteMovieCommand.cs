using MediatR;

namespace Movies.Application.Movies.Commands.DeleteMovie;

public record DeleteMovieCommand(
    Guid Id
    ) : IRequest<bool>;
