using FluentValidation;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterDividende;

/// <summary>Contrôles de forme. Les règles métier restent dans le Domain (constructeur de Dividende).</summary>
public sealed class AjouterDividendeCommandValidator : AbstractValidator<AjouterDividendeCommand>
{
    public const decimal MontantMaximum = 100_000_000m;

    public AjouterDividendeCommandValidator()
    {
        RuleFor(c => c.TitreId).NotEmpty().WithMessage("Choisissez l'action.");
        RuleFor(c => c.Exercice).InclusiveBetween(1990, 2100).WithMessage("L'exercice doit être compris entre 1990 et 2100.");
        RuleFor(c => c.MontantBrut).GreaterThan(0m).LessThanOrEqualTo(MontantMaximum)
            .WithMessage("Le dividende brut par action doit être supérieur à 0 FCFA.");
        RuleFor(c => c.MontantNet).GreaterThan(0m).LessThanOrEqualTo(MontantMaximum).When(c => c.MontantNet.HasValue)
            .WithMessage("Le dividende net par action doit être supérieur à 0 FCFA.");
    }
}
