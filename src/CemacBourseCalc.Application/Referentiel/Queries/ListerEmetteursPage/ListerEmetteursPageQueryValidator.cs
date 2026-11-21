using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Domain.Communs;
using FluentValidation;

namespace CemacBourseCalc.Application.Referentiel.Queries.ListerEmetteursPage;

public sealed class ListerEmetteursPageQueryValidator : ValidateurPagination<ListerEmetteursPageQuery>
{
    public ListerEmetteursPageQueryValidator()
    {
        RuleFor(q => q.Pays)
            .Must(FiltresListes.EstValeur<PaysCemac>)
            .When(q => q.Pays is not null)
            .WithMessage("Pays inconnu.");
        RuleFor(q => q.Type)
            .Must(FiltresListes.EstValeur<TypeEmetteur>)
            .When(q => q.Type is not null)
            .WithMessage("Type d'émetteur inconnu.");
    }
}
