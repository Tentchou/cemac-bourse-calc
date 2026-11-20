using CemacBourseCalc.Application.Common.Masse;
using FluentValidation;

namespace CemacBourseCalc.Application.Place.Commands.AjouterJoursNonOuvresEnMasse;

public sealed class AjouterJoursNonOuvresEnMasseCommandValidator : AbstractValidator<AjouterJoursNonOuvresEnMasseCommand>
{
    public AjouterJoursNonOuvresEnMasseCommandValidator() => RuleFor(c => c.Lignes).BornesMasse();
}
