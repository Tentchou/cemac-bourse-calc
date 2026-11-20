using CemacBourseCalc.Application.Administration.Common;
using FluentValidation;

namespace CemacBourseCalc.Application.Administration.Commands.OuvrirSession;

/// <summary>Bornes seulement : un mot de passe trop long coûterait cher à vérifier.</summary>
public sealed class OuvrirSessionCommandValidator : AbstractValidator<OuvrirSessionCommand>
{
    public OuvrirSessionCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().MaximumLength(254).WithMessage("Saisissez votre adresse e-mail.");
        RuleFor(c => c.MotDePasse).NotEmpty().MaximumLength(ReglesMotDePasse.LongueurMaximum)
            .WithMessage("Saisissez votre mot de passe.");
    }
}
