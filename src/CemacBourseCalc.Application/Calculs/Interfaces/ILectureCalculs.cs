using CemacBourseCalc.Domain.Calculs;

namespace CemacBourseCalc.Application.Calculs.Interfaces;

/// <summary>Ce que les calculs lisent en base (Dapper). Aucune écriture.</summary>
public interface ILectureCalculs
{
    /// <summary>Les caractéristiques de l'obligation, ou null si l'identifiant n'est pas celui d'une obligation.</summary>
    Task<CaracteristiquesObligation?> ObligationAsync(Guid titreId, CancellationToken ct);

    /// <summary>La valeur brute d'un paramètre de la place, ou null s'il est absent.</summary>
    Task<string?> ParametreAsync(string cle, CancellationToken ct);

    /// <summary>Les jours non ouvrés de la place entre deux dates incluses.</summary>
    Task<IReadOnlySet<DateOnly>> JoursNonOuvresAsync(DateOnly du, DateOnly au, CancellationToken ct);
}
