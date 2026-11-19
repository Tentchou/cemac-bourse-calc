using CemacBourseCalc.Application.Titres.Interfaces;
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

    public async Task AjouterAsync(Titre titre, CancellationToken ct)
    {
        db.Titres.Add(titre);
        await db.SaveChangesAsync(ct);
    }
}
