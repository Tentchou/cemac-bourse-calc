using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Domain.Titres;
using CemacBourseCalc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CemacBourseCalc.Infrastructure.Ecriture;

public sealed class TitresEcriture(CalcDbContext db) : ITitresEcriture
{
    public Task<bool> IsinExisteAsync(string isin, CancellationToken ct) =>
        db.Titres.AnyAsync(t => t.Isin == Isin.Creer(isin), ct);

    public async Task<int> AjouterAsync(Titre titre, CancellationToken ct)
    {
        db.Titres.Add(titre);
        await db.SaveChangesAsync(ct);
        return titre.Id;
    }
}
