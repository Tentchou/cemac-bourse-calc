using CemacBourseCalc.Application.Calculs.Common;
using CemacBourseCalc.Domain.Calculs;
using FluentValidation;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterCours;

/// <summary>Contrôles de forme. Les bornes selon le type de titre (% ou FCFA) sont vérifiées par le handler.</summary>
public sealed class AjouterCoursCommandValidator : AbstractValidator<AjouterCoursCommand>
{
    public AjouterCoursCommandValidator()
    {
        RuleFor(c => c.TitreId).NotEmpty().WithMessage("Choisissez le titre.");
        RuleFor(c => c.DateSeance)
            .InclusiveBetween(BornesSaisie.DateMinimum, BornesSaisie.DateMaximum)
            .WithMessage("La date de séance doit être comprise entre 1990 et 2100.");
        RuleFor(c => c).Must(c => c.CoursReference is not null || c.CoursCloture is not null)
            .WithName("Cours").WithMessage("Saisissez au moins un cours (référence ou clôture).");
        RuleFor(c => c.CoursReference).GreaterThan(0m).LessThanOrEqualTo(RendementDividende.CoursMaximum)
            .When(c => c.CoursReference.HasValue).WithMessage("Le cours de référence doit être supérieur à 0.");
        RuleFor(c => c.CoursCloture).GreaterThan(0m).LessThanOrEqualTo(RendementDividende.CoursMaximum)
            .When(c => c.CoursCloture.HasValue).WithMessage("Le cours de clôture doit être supérieur à 0.");
        RuleFor(c => c.Volume).GreaterThanOrEqualTo(0).When(c => c.Volume.HasValue)
            .WithMessage("Le volume ne peut pas être négatif.");
        RuleFor(c => c.CouponCouruPublie).GreaterThanOrEqualTo(0m).When(c => c.CouponCouruPublie.HasValue)
            .WithMessage("Le coupon couru ne peut pas être négatif.");
    }
}
