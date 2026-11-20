using CemacBourseCalc.Application.Calculs.Common;
using CemacBourseCalc.Domain.Calculs;
using FluentValidation;

namespace CemacBourseCalc.Application.Calculs.Queries.CalculerRendementObligation;

/// <summary>Bornes de saisie de l'API publique : elles protègent le serveur, les règles métier restent au Domain.</summary>
public sealed class CalculerRendementObligationQueryValidator : AbstractValidator<CalculerRendementObligationQuery>
{
    public CalculerRendementObligationQueryValidator()
    {
        RuleFor(q => q.TitreId).NotEmpty().WithMessage("Choisissez un titre.");
        RuleFor(q => q.DateNegociation)
            .InclusiveBetween(BornesSaisie.DateMinimum, BornesSaisie.DateMaximum)
            .WithMessage("La date de négociation doit être comprise entre 1990 et 2100.");
        RuleFor(q => q.CoursPiedCoupon)
            .GreaterThan(0m).LessThanOrEqualTo(PrixPleinCoupon.CoursMaximum)
            .WithMessage("Le cours s'exprime en pourcentage du nominal (ex. 98,50).");
    }
}
