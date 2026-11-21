namespace CemacBourseCalc.Application.Common.Pagination;

/// <summary>Une page d'une liste : ses lignes, le nombre total de lignes du filtre, la page et sa taille.</summary>
public sealed record PageDto<T>(IReadOnlyList<T> Lignes, int Total, int Page, int Taille);
