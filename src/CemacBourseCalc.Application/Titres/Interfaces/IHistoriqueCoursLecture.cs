using CemacBourseCalc.Application.Titres.Dtos;

namespace CemacBourseCalc.Application.Titres.Interfaces;

/// <summary>Lecture de l'historique des cours d'un titre (Dapper, index unique titre + séance).</summary>
public interface IHistoriqueCoursLecture
{
    /// <summary>
    /// Les <paramref name="maximum"/> séances les plus récentes, de la plus ancienne à la plus récente ;
    /// null si le titre n'existe pas (une liste vide : titre connu, sans cours).
    /// </summary>
    Task<IReadOnlyList<PointCoursDto>?> HistoriqueAsync(Guid titreId, int maximum, CancellationToken ct);
}
