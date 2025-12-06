using FluentValidation;

namespace Movies.Application.Commands.CreateMovie;

public class CreateMovieCommandValidator : AbstractValidator<CreateMovieCommand>
{
    public CreateMovieCommandValidator()
    {
        RuleFor(movie => movie.Title)
            .NotEmpty().WithMessage("Title cannot be empty.")
            .MaximumLength(50).WithMessage("Title cannot be longer than 50 characters.");

        RuleFor(movie => movie.DurationInMinutes)
            .GreaterThan(0).WithMessage("The duration must be greater than zero.");

        RuleFor(movie => movie.Price)
            .GreaterThan(0).WithMessage("The price must be greater than zero.");
    }
}
