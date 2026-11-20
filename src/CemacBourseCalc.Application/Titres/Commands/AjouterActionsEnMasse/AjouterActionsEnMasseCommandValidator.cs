using CemacBourseCalc.Application.Common.Masse;
using FluentValidation;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterActionsEnMasse;

public sealed class AjouterActionsEnMasseCommandValidator : AbstractValidator<AjouterActionsEnMasseCommand>
{
    public AjouterActionsEnMasseCommandValidator() => RuleFor(c => c.Lignes).BornesMasse();
}
