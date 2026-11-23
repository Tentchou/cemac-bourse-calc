using FluentValidation;

namespace CemacBourseCalc.Application.Titres.Commands.ModifierAction;

/// <summary>Les mêmes contrôles de forme qu'à la saisie (sans l'ISIN, qui ne change pas), plus le statut.</summary>
public sealed class ModifierActionCommandValidator : AbstractValidator<ModifierActionCommand>
{
    public ModifierActionCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty().WithMessage("Le titre à modifier est inconnu.");
        RuleFor(c => c.Nom).NotEmpty().MaximumLength(200).WithMessage("Le nom est obligatoire (200 caractères au plus).");
        RuleFor(c => c.EmetteurId).NotEmpty().WithMessage("Choisissez l'émetteur du titre.");
        RuleFor(c => c.Nominal).GreaterThan(0m).WithMessage("La valeur nominale doit être supérieure à 0 FCFA.");
        RuleFor(c => c.Statut).IsInEnum().WithMessage("Le statut du titre n'est pas reconnu.");
        RuleFor(c => c.NombreTitres).GreaterThan(0).WithMessage("Le nombre d'actions de la société doit être supérieur à 0.");
    }
}
