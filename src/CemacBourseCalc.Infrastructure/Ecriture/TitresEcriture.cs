using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;
using CemacBourseCalc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CemacBourseCalc.Infrastructure.Ecriture;

public sealed class TitresEcriture(CalcDbContext db) : ITitresEcriture
{
    public Task<bool> IsinExisteAsync(string isin, CancellationToken ct) =>
        db.Titres.AnyAsync(t => t.Isin == Isin.Creer(isin), ct);

    public Task<bool> EmetteurExisteAsync(Guid emetteurId, CancellationToken ct) =>
        db.Emetteurs.AnyAsync(e => e.Id == emetteurId, ct);

    public Task<TypeTitre?> TypeDuTitreAsync(Guid titreId, CancellationToken ct) =>
        db.Titres.Where(t => t.Id == titreId).Select(t => (TypeTitre?)t.TypeTitre).FirstOrDefaultAsync(ct);

    public Task<bool> CoursExisteAsync(Guid titreId, DateOnly dateSeance, CancellationToken ct) =>
        db.Cours.AnyAsync(c => c.TitreId == titreId && c.DateSeance == dateSeance, ct);

    public Task<Obligation?> ObligationAsync(Guid titreId, CancellationToken ct) =>
        db.Titres.OfType<Obligation>().FirstOrDefaultAsync(t => t.Id == titreId, ct);

    public Task<Domain.Titres.Action?> ActionAsync(Guid titreId, CancellationToken ct) =>
        db.Titres.OfType<Domain.Titres.Action>().FirstOrDefaultAsync(t => t.Id == titreId, ct);

    public Task<Titre?> TitreAsync(Guid titreId, CancellationToken ct) =>
        db.Titres.FirstOrDefaultAsync(t => t.Id == titreId, ct);

    // Trois EXISTS sur des index (titre_id en tête) : une seule requête, arrêtée à la première ligne trouvée.
    public Task<bool> AHistoriqueAsync(Guid titreId, CancellationToken ct) =>
        db.Titres.Where(t => t.Id == titreId)
            .Select(t => db.Cours.Any(c => c.TitreId == t.Id)
                || db.Dividendes.Any(d => d.TitreId == t.Id)
                || db.DocumentsSource.Any(d => d.TitreId == t.Id))
            .FirstOrDefaultAsync(ct);

    public void Ajouter(Titre titre) => db.Titres.Add(titre);
    public void Supprimer(Titre titre) => db.Titres.Remove(titre);
    public void AjouterCours(Cours cours) => db.Cours.Add(cours);
    public void AjouterDividende(Dividende dividende) => db.Dividendes.Add(dividende);
}
