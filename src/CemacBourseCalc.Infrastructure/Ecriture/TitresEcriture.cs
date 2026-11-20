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

    public void Ajouter(Titre titre) => db.Titres.Add(titre);
    public void AjouterCours(Cours cours) => db.Cours.Add(cours);
    public void AjouterDividende(Dividende dividende) => db.Dividendes.Add(dividende);
}
