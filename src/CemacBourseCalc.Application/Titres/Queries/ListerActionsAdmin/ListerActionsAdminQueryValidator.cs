using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Domain.Communs;
using FluentValidation;

namespace CemacBourseCalc.Application.Titres.Queries.ListerActionsAdmin;

public sealed class ListerActionsAdminQueryValidator : ValidateurPagination<ListerActionsAdminQuery>
{
    public ListerActionsAdminQueryValidator()
    {
        RuleFor(q => q.Pays)
            .Must(FiltresListes.EstValeur<PaysCemac>)
            .When(q => q.Pays is not null)
            .WithMessage("Pays inconnu.");
    }
}
