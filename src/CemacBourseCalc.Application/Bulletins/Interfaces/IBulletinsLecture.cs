using CemacBourseCalc.Domain.Calculs;

namespace CemacBourseCalc.Application.Bulletins.Interfaces;

/// <summary>Un titre de la base, pour rapprocher les lignes du bulletin (par ISIN) et contrôler le coupon couru.</summary>
public sealed record TitreReference(Guid Id, string Isin, string Nom, string TypeTitre, string Statut, CaracteristiquesObligation? Obligation);

/// <summary>Ce que l'analyse d'un bulletin lit en base (Dapper, lecture seule).</summary>
public interface IBulletinsLecture
{
    /// <summary>Tous les titres (quelques dizaines sur la BVMAC), avec les caractéristiques des obligations.</summary>
    Task<IReadOnlyList<TitreReference>> TitresAsync(CancellationToken ct);

    /// <summary>Le nombre de cours déjà enregistrés pour une séance.</summary>
    Task<int> CoursDeLaSeanceAsync(DateOnly dateSeance, CancellationToken ct);
}
