namespace CemacBourseCalc.Application.Titres.Dtos;

/// <summary>Ce qu'est devenu un titre supprimé : effacé (aucun historique) ou radié (historique gardé).</summary>
public sealed record SuppressionTitreDto(Guid Id, bool Efface);
