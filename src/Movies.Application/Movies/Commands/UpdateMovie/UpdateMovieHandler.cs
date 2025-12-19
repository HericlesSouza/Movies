using MediatR;

using Movies.Application.Abstractions.Persistence;
using Movies.Application.DTOs;

namespace Movies.Application.Movies.Commands.UpdateMovie;

public class UpdateMovieHandler(
    IMovieRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateMovieCommand, MovieDto?>
{
    private readonly IMovieRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<MovieDto?> Handle(UpdateMovieCommand command, CancellationToken ct)
    {
        var movie = await _repository.GetByIdAsync(command.Id, ct);

        if (movie is null)
        {
            return null;
        }

        movie.Update(command.Title, command.Description, command.DurationInMinutes, command.Price);

        await _repository.UpdateAsync(movie, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return new MovieDto(
            movie.Id,
            movie.Title,
            movie.Description,
            movie.DurationInMinutes,
            movie.Price,
            movie.CreatedAt,
            movie.UpdatedAt);
    }
}
