using CemacBourseCalc.Application.Calculs.Common;
using FluentValidation;

namespace CemacBourseCalc.Application.Place.Commands.AjouterJourNonOuvre;

public sealed class AjouterJourNonOuvreCommandValidator : AbstractValidator<AjouterJourNonOuvreCommand>
{
    public AjouterJourNonOuvreCommandValidator()
    {
        RuleFor(c => c.Date)
            .InclusiveBetween(BornesSaisie.DateMinimum, BornesSaisie.DateMaximum)
            .WithMessage("La date doit être comprise entre 1990 et 2100.");
        RuleFor(c => c.Libelle).NotEmpty().MaximumLength(100)
            .WithMessage("Indiquez le libellé du jour (100 caractères au plus), ex. « Noël ».");
    }
}
