using CemacBourseCalc.Application.Calculs.Common;
using CemacBourseCalc.Application.Common.Pagination;
using FluentValidation;

namespace CemacBourseCalc.Application.Place.Queries.ListerJoursNonOuvres;

public sealed class ListerJoursNonOuvresQueryValidator : ValidateurPagination<ListerJoursNonOuvresQuery>
{
    public ListerJoursNonOuvresQueryValidator()
    {
        RuleFor(q => q.Annee)
            .InclusiveBetween(BornesSaisie.DateMinimum.Year, BornesSaisie.DateMaximum.Year)
            .When(q => q.Annee.HasValue)
            .WithMessage("L'année doit être comprise entre 1990 et 2100.");
    }
}
