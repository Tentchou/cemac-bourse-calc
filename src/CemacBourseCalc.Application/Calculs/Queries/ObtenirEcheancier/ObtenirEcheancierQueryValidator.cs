using CemacBourseCalc.Application.Calculs.Common;
using FluentValidation;

namespace CemacBourseCalc.Application.Calculs.Queries.ObtenirEcheancier;

public sealed class ObtenirEcheancierQueryValidator : AbstractValidator<ObtenirEcheancierQuery>
{
    public ObtenirEcheancierQueryValidator()
    {
        RuleFor(q => q.TitreId).NotEmpty().WithMessage("Choisissez un titre.");
        RuleFor(q => q.Quantite)
            .InclusiveBetween(1, BornesSaisie.QuantiteMaximum)
            .WithMessage($"La quantité doit être comprise entre 1 et {BornesSaisie.QuantiteMaximum:N0} titres.");
        RuleFor(q => q.DateNegociation!.Value)
            .InclusiveBetween(BornesSaisie.DateMinimum, BornesSaisie.DateMaximum)
            .When(q => q.DateNegociation.HasValue)
            .WithMessage("La date de négociation doit être comprise entre 1990 et 2100.");
    }
}
