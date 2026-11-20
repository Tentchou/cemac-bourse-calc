using FluentValidation;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterAction;

/// <summary>Contrôles de forme de la saisie. Les règles métier restent dans le Domain (constructeur d'Action).</summary>
public sealed class AjouterActionCommandValidator : AbstractValidator<AjouterActionCommand>
{
    public AjouterActionCommandValidator()
    {
        RuleFor(c => c.Isin).NotEmpty().WithMessage("Le code ISIN est obligatoire.");
        RuleFor(c => c.Nom).NotEmpty().MaximumLength(200).WithMessage("Le nom est obligatoire (200 caractères au plus).");
        RuleFor(c => c.EmetteurId).NotEmpty().WithMessage("Choisissez l'émetteur du titre.");
        RuleFor(c => c.Nominal).GreaterThan(0m).WithMessage("La valeur nominale doit être supérieure à 0 FCFA.");
        RuleFor(c => c.NombreTitres).GreaterThan(0).WithMessage("Le nombre d'actions de la société doit être supérieur à 0.");
    }
}
