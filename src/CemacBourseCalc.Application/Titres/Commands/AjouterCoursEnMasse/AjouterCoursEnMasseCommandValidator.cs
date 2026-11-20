using CemacBourseCalc.Application.Common.Masse;
using FluentValidation;

namespace CemacBourseCalc.Application.Titres.Commands.AjouterCoursEnMasse;

public sealed class AjouterCoursEnMasseCommandValidator : AbstractValidator<AjouterCoursEnMasseCommand>
{
    public AjouterCoursEnMasseCommandValidator() => RuleFor(c => c.Lignes).BornesMasse();
}
