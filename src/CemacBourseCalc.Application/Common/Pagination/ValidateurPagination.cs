using FluentValidation;

namespace CemacBourseCalc.Application.Common.Pagination;

/// <summary>Les règles communes à toutes les listes paginées ; chaque requête y ajoute ses propres filtres.</summary>
public abstract class ValidateurPagination<T> : AbstractValidator<T> where T : IRequetePaginee
{
    protected ValidateurPagination()
    {
        RuleFor(q => q.Page)
            .InclusiveBetween(1, Pagination.PageMaximum)
            .WithMessage($"La page doit être comprise entre 1 et {Pagination.PageMaximum}.");
        RuleFor(q => q.Taille)
            .Must(Pagination.Tailles.Contains)
            .WithMessage("Le nombre de lignes par page doit être 10, 20 ou 50.");
        RuleFor(q => q.Recherche)
            .MaximumLength(Pagination.RechercheMaximum)
            .WithMessage($"La recherche est limitée à {Pagination.RechercheMaximum} caractères.");
    }
}
