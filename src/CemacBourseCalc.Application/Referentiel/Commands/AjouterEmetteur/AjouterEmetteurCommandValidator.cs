using FluentValidation;

namespace CemacBourseCalc.Application.Referentiel.Commands.AjouterEmetteur;

/// <summary>Contrôles de forme, alignés sur les longueurs des colonnes. Les règles métier restent dans le Domain.</summary>
public sealed class AjouterEmetteurCommandValidator : AbstractValidator<AjouterEmetteurCommand>
{
    public AjouterEmetteurCommandValidator()
    {
        RuleFor(c => c.Nom).NotEmpty().MaximumLength(200).WithMessage("Le nom est obligatoire (200 caractères au plus).");
        RuleFor(c => c.Sigle).MaximumLength(30).WithMessage("Le sigle fait 30 caractères au plus.");
        RuleFor(c => c.Type).IsInEnum().WithMessage("Le type d'émetteur n'est pas reconnu (Etat, Entreprise, Supranational).");
        RuleFor(c => c.PaysCode).IsInEnum().WithMessage("Le pays doit être l'un des six pays de la CEMAC.");
        RuleFor(c => c.SiteWeb).MaximumLength(300).WithMessage("Le site web fait 300 caractères au plus.");
    }
}
