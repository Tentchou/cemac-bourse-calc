using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Domain.Administration;
using CemacBourseCalc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CemacBourseCalc.Infrastructure.Ecriture;

public sealed class AdministrateursEcriture(CalcDbContext db) : IAdministrateursEcriture
{
    public Task<Administrateur?> ParEmailAsync(string emailMinuscules, CancellationToken ct) =>
        db.Administrateurs.SingleOrDefaultAsync(a => a.Email == emailMinuscules, ct);

    public Task<bool> EmailExisteAsync(string emailMinuscules, CancellationToken ct) =>
        db.Administrateurs.AnyAsync(a => a.Email == emailMinuscules, ct);

    public void Ajouter(Administrateur administrateur) => db.Administrateurs.Add(administrateur);

    public Task<JetonSession?> SessionParEmpreinteAsync(string empreinte, CancellationToken ct) =>
        db.JetonsSession.SingleOrDefaultAsync(s => s.JetonHash == empreinte, ct);

    public void AjouterSession(JetonSession session) => db.JetonsSession.Add(session);
}
