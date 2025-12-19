using FluentValidation;

namespace Movies.Application.Movies.Queries.ListMovies;

public class ListMoviesQueryValidator : AbstractValidator<ListMoviesQuery>
{
    public ListMoviesQueryValidator()
    {
        RuleFor(x => x.Page)
                    .GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("O tamanho da página deve ser pelo menos 1.")
            .LessThanOrEqualTo(50).WithMessage("O tamanho da página não pode exceder 50 registros.");

        var allowedSortColumns = new[] { "title", "price", "durationinminutes", "createdat" };

        RuleFor(x => x.SortBy)
            .Must(sortBy => string.IsNullOrEmpty(sortBy) || allowedSortColumns.Contains(sortBy.ToLower()))
            .WithMessage($"A ordenação é permitida apenas por: {string.Join(", ", allowedSortColumns)}");
    }
}
