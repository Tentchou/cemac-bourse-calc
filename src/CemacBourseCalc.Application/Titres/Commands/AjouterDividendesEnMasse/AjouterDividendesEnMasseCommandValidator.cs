using CemacBourseCalc.Application.Common.Masse;
using FluentValidation;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterDividendesEnMasse;

public sealed class AjouterDividendesEnMasseCommandValidator : AbstractValidator<AjouterDividendesEnMasseCommand>
{
    public AjouterDividendesEnMasseCommandValidator() => RuleFor(c => c.Lignes).BornesMasse();
}
