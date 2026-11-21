using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Domain.Communs;
using FluentValidation;

namespace CemacBourseCalc.Application.Titres.Queries.ListerObligationsAdmin;

public sealed class ListerObligationsAdminQueryValidator : ValidateurPagination<ListerObligationsAdminQuery>
{
    public ListerObligationsAdminQueryValidator()
    {
        RuleFor(q => q.Pays)
            .Must(FiltresListes.EstValeur<PaysCemac>)
            .When(q => q.Pays is not null)
            .WithMessage("Pays inconnu.");
        RuleFor(q => q.Remboursement)
            .Must(FiltresListes.EstValeur<ModeRemboursement>)
            .When(q => q.Remboursement is not null)
            .WithMessage("Mode de remboursement inconnu.");
    }
}
