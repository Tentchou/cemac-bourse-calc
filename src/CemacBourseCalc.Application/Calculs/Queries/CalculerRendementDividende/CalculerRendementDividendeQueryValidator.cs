using CemacBourseCalc.Domain.Calculs;
using FluentValidation;

namespace CemacBourseCalc.Application.Calculs.Queries.CalculerRendementDividende;

/// <summary>Bornes de saisie de l'API publique : elles protègent le serveur, les règles métier restent au Domain.</summary>
public sealed class CalculerRendementDividendeQueryValidator : AbstractValidator<CalculerRendementDividendeQuery>
{
    public CalculerRendementDividendeQueryValidator()
    {
        RuleFor(q => q.TitreId).NotEmpty().WithMessage("Choisissez un titre.");
        RuleFor(q => q.Cours)
            .GreaterThan(0m).LessThanOrEqualTo(RendementDividende.CoursMaximum)
            .WithMessage("Le cours d'une action s'exprime en FCFA (ex. 32 000).");
    }
}
