using FluentValidation;

namespace Movies.Application.DTOs.Validators;

public class MovieUpdateDtoValidator : AbstractValidator<MovieUpdateDto>
{
    public MovieUpdateDtoValidator()
    {
        RuleFor(movie => movie.Title)
            .NotEmpty().WithMessage("Title cannot be empty.")
            .MaximumLength(50).WithMessage("Title cannot be longer than 50 characters.");

        RuleFor(movie => movie.DurationInMinues)
            .GreaterThan(0).WithMessage("The duration must be greater than zero.");

        RuleFor(movie => movie.Price)
            .GreaterThan(0).WithMessage("The price must be greater than zero.");
    }
}
