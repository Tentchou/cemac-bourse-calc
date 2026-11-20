using CemacBourseCalc.Application.Administration.Common;
using FluentValidation;

namespace CemacBourseCalc.Application.Administration.Commands.CreerAdministrateur;

public sealed class CreerAdministrateurCommandValidator : AbstractValidator<CreerAdministrateurCommand>
{
    public CreerAdministrateurCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().MaximumLength(254).EmailAddress()
            .WithMessage("Saisissez une adresse e-mail valide.");
        RuleFor(c => c.Nom).NotEmpty().MaximumLength(200)
            .WithMessage("Le nom est obligatoire (200 caractères au plus).");
        RuleFor(c => c.MotDePasse)
            .Length(ReglesMotDePasse.LongueurMinimum, ReglesMotDePasse.LongueurMaximum)
            .WithMessage($"Le mot de passe doit faire entre {ReglesMotDePasse.LongueurMinimum} et {ReglesMotDePasse.LongueurMaximum} caractères.");
        RuleFor(c => c.Role).IsInEnum().WithMessage("Le rôle n'est pas reconnu.");
    }
}
