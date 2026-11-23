using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;

namespace CemacBourseCalc.Application.Titres.Interfaces;

/// <summary>
/// Écritures des titres, cours et dividendes (EF Core), réservées à l'administration.
/// Les ajouts sont enregistrés par <c>IUniteDeTravail</c>, avec leur ligne de journal.
/// </summary>
public interface ITitresEcriture
{
    Task<bool> IsinExisteAsync(string isin, CancellationToken ct);
    Task<bool> EmetteurExisteAsync(Guid emetteurId, CancellationToken ct);

    /// <summary>Le type du titre, ou null s'il n'existe pas.</summary>
    Task<TypeTitre?> TypeDuTitreAsync(Guid titreId, CancellationToken ct);
    Task<bool> CoursExisteAsync(Guid titreId, DateOnly dateSeance, CancellationToken ct);

    /// <summary>L'obligation suivie pour modification, ou null si l'identifiant n'est pas une obligation.</summary>
    Task<Obligation?> ObligationAsync(Guid titreId, CancellationToken ct);

    /// <summary>L'action suivie pour modification, ou null si l'identifiant n'est pas une action.</summary>
    Task<Domain.Titres.Action?> ActionAsync(Guid titreId, CancellationToken ct);

    /// <summary>Le titre suivi pour suppression ou radiation, ou null.</summary>
    Task<Titre?> TitreAsync(Guid titreId, CancellationToken ct);

    /// <summary>Le titre a-t-il un historique (cours, dividendes, documents) ? Si oui, il est radié, jamais effacé.</summary>
    Task<bool> AHistoriqueAsync(Guid titreId, CancellationToken ct);

    void Ajouter(Titre titre);
    void Supprimer(Titre titre);
    void AjouterCours(Cours cours);
    void AjouterDividende(Dividende dividende);
}
