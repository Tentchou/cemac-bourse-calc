using CemacBourseCalc.Application.Common.Masse;
using FluentValidation;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterObligationsEnMasse;

public sealed class AjouterObligationsEnMasseCommandValidator : AbstractValidator<AjouterObligationsEnMasseCommand>
{
    public AjouterObligationsEnMasseCommandValidator() => RuleFor(c => c.Lignes).BornesMasse();
}
