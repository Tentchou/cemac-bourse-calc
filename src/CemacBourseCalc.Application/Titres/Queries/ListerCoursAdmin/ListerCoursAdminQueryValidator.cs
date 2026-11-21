using CemacBourseCalc.Application.Calculs.Common;
using CemacBourseCalc.Application.Common.Pagination;
using FluentValidation;

namespace CemacBourseCalc.Application.Titres.Queries.ListerCoursAdmin;

public sealed class ListerCoursAdminQueryValidator : ValidateurPagination<ListerCoursAdminQuery>
{
    public ListerCoursAdminQueryValidator()
    {
        RuleFor(q => q.DateSeance)
            .InclusiveBetween(BornesSaisie.DateMinimum, BornesSaisie.DateMaximum)
            .When(q => q.DateSeance.HasValue)
            .WithMessage("La date de séance doit être comprise entre 1990 et 2100.");
    }
}
