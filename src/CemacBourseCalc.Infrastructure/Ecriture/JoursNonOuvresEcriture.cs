using CemacBourseCalc.Application.Place.Interfaces;
using CemacBourseCalc.Domain.Place;
using CemacBourseCalc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CemacBourseCalc.Infrastructure.Ecriture;

public sealed class JoursNonOuvresEcriture(CalcDbContext db) : IJoursNonOuvresEcriture
{
    public Task<bool> ExisteAsync(DateOnly date, CancellationToken ct) => db.JoursNonOuvres.AnyAsync(j => j.Date == date, ct);

    public void Ajouter(JourNonOuvre jour) => db.JoursNonOuvres.Add(jour);
}
