namespace CemacBourseCalc.Application.Common.Pagination;

/// <summary>Une requête de liste paginée : la page (à partir de 1), sa taille et un texte de recherche facultatif.</summary>
public interface IRequetePaginee
{
    int Page { get; }
    int Taille { get; }
    string? Recherche { get; }
}
