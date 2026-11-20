namespace CemacBourseCalc.Application.Common.Masse;

/// <summary>Une ligne refusée : son numéro (1 = première ligne envoyée) et les raisons, en français.</summary>
public sealed record ErreurLigneDto(int Ligne, IReadOnlyList<string> Messages);

/// <summary>
/// Bilan d'une saisie en masse. Tout ou rien : s'il y a la moindre erreur, aucune ligne n'est
/// enregistrée (<see cref="Enregistrees"/> = 0) et toutes les erreurs sont listées d'un coup.
/// </summary>
public sealed record ResultatMasseDto(int Lignes, int Enregistrees, IReadOnlyList<ErreurLigneDto> Erreurs)
{
    public bool Complet => Erreurs.Count == 0;
}
