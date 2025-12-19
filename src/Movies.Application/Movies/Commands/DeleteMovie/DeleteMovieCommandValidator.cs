using FluentValidation;

namespace Movies.Application.Movies.Commands.DeleteMovie;

public class DeleteMovieCommandValidator : AbstractValidator<DeleteMovieCommand>
{
    public DeleteMovieCommandValidator()
    {
        RuleFor(movie => movie.Id)
            .NotEmpty().WithMessage("Id cannot be empty.");
    }
}
