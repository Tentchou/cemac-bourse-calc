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

    void Ajouter(Titre titre);
    void AjouterCours(Cours cours);
    void AjouterDividende(Dividende dividende);
}
