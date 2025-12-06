using MediatR;

using Movies.Application.Abstractions.Persistence;
using Movies.Application.DTOs;
using Movies.Domain.Entities;

namespace Movies.Application.Commands.CreateMovie;

public class CreateMovieHandler(
    IMovieRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateMovieCommand, MovieDto>
{
    private readonly IMovieRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<MovieDto> Handle(CreateMovieCommand command, CancellationToken ct)
    {
        var movie = new Movie(
            command.Title,
            command.Description,
            command.DurationInMinutes,
            command.Price
            );

        await _repository.AddAsync(movie, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return new MovieDto(
            movie.Id,
            movie.Title,
            movie.Description,
            movie.DurationInMinutes,
            movie.Price,
            movie.CreatedAt,
            movie.UpdatedAt
            );
    }
}
