using CemacBourseCalc.Domain.Place;
using FluentValidation;

namespace CemacBourseCalc.Application.Place.Queries.PreparerAnnee;

public sealed class PreparerAnneeQueryValidator : AbstractValidator<PreparerAnneeQuery>
{
    public PreparerAnneeQueryValidator()
    {
        RuleFor(q => q.Annee)
            .InclusiveBetween(JoursFeriesCameroun.AnneeMinimum, JoursFeriesCameroun.AnneeMaximum)
            .WithMessage($"L'année doit être comprise entre {JoursFeriesCameroun.AnneeMinimum} et {JoursFeriesCameroun.AnneeMaximum}.");
    }
}
