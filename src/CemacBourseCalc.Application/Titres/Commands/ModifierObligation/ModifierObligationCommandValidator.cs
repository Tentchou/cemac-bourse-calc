using CemacBourseCalc.Domain.Conventions;
using FluentValidation;

namespace CemacBourseCalc.Application.Titres.Commands.ModifierObligation;

/// <summary>Les mêmes contrôles de forme qu'à la saisie (sans l'ISIN, qui ne change pas), plus le statut.</summary>
public sealed class ModifierObligationCommandValidator : AbstractValidator<ModifierObligationCommand>
{
    public ModifierObligationCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty().WithMessage("Le titre à modifier est inconnu.");
        RuleFor(c => c.Nom).NotEmpty().MaximumLength(200).WithMessage("Le nom est obligatoire (200 caractères au plus).");
        RuleFor(c => c.EmetteurId).NotEmpty().WithMessage("Choisissez l'émetteur du titre.");
        RuleFor(c => c.Nominal).GreaterThan(0m).WithMessage("La valeur nominale doit être supérieure à 0 FCFA.");
        RuleFor(c => c.Statut).IsInEnum().WithMessage("Le statut du titre n'est pas reconnu.");
        RuleFor(c => c.TauxFacial).InclusiveBetween(0m, 100m).WithMessage("Le taux facial s'exprime en pourcentage, entre 0 et 100.");
        RuleFor(c => c.Periodicite).IsInEnum().WithMessage("La périodicité des coupons n'est pas reconnue.");
        RuleFor(c => c.ModeRemboursement).IsInEnum().WithMessage("Le mode de remboursement n'est pas reconnu.");
        RuleFor(c => c.RegimeFiscal).IsInEnum().WithMessage("Le régime fiscal du coupon n'est pas reconnu.");
        RuleFor(c => c.DiffereEcheances).GreaterThanOrEqualTo(0).WithMessage("Le différé ne peut pas être négatif.");
        RuleFor(c => c.CodeConvention)
            .Must(code => ConventionsDecompte.Trouver(code) is not null)
            .WithMessage("Choisissez une convention de décompte de la liste.");
    }
}
