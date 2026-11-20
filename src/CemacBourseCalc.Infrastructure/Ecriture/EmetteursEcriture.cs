using CemacBourseCalc.Application.Referentiel.Interfaces;
using CemacBourseCalc.Domain.Referentiel;
using CemacBourseCalc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CemacBourseCalc.Infrastructure.Ecriture;

public sealed class EmetteursEcriture(CalcDbContext db) : IEmetteursEcriture
{
    public Task<bool> NomExisteAsync(string nom, CancellationToken ct)
    {
        var recherche = nom.Trim().ToLower();
        return db.Emetteurs.AnyAsync(e => e.Nom.ToLower() == recherche, ct);
    }

    public void Ajouter(Emetteur emetteur) => db.Emetteurs.Add(emetteur);
}
