using CemacBourseCalc.Application.Common.Masse;
using FluentValidation;

namespace CemacBourseCalc.Application.Referentiel.Commands.AjouterEmetteursEnMasse;

public sealed class AjouterEmetteursEnMasseCommandValidator : AbstractValidator<AjouterEmetteursEnMasseCommand>
{
    public AjouterEmetteursEnMasseCommandValidator() => RuleFor(c => c.Lignes).BornesMasse();
}
