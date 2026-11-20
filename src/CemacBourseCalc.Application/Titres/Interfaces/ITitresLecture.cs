using CemacBourseCalc.Application.Titres.Dtos;

namespace CemacBourseCalc.Application.Titres.Interfaces;

/// <summary>Lectures des titres (Dapper, SQL écrit à la main).</summary>
public interface ITitresLecture
{
    Task<IReadOnlyList<TitreResumeDto>> ListerAsync(CancellationToken ct);

    /// <summary>
    /// La fiche d'un titre, ou null s'il n'existe pas. La convention est rendue par son code
    /// en base (« EXACT_365 ») : le handler la traduit en libellé.
    /// </summary>
    Task<FicheTitreDto?> FicheAsync(Guid id, CancellationToken ct);
}
