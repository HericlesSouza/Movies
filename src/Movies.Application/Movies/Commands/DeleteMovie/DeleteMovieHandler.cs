using MediatR;

using Movies.Application.Abstractions.Persistence;

namespace Movies.Application.Movies.Commands.DeleteMovie;

public class DeleteMovieHandler(
    IMovieRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteMovieCommand, bool>
{
    private readonly IMovieRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteMovieCommand command, CancellationToken ct)
    {
        var movie = await _repository.GetByIdAsync(command.Id, ct);

        if (movie is null)
            return false;

        await _repository.DeleteAsync(movie, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}
