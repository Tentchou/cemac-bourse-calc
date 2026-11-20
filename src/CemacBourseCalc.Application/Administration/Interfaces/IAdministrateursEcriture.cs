using CemacBourseCalc.Domain.Administration;

namespace CemacBourseCalc.Application.Administration.Interfaces;

/// <summary>Comptes et sessions de l'administration (EF Core). Les ajouts sont enregistrés par l'unité de travail.</summary>
public interface IAdministrateursEcriture
{
    /// <summary>Le compte suivi par EF (ses changements seront enregistrés), ou null.</summary>
    Task<Administrateur?> ParEmailAsync(string emailMinuscules, CancellationToken ct);
    Task<bool> EmailExisteAsync(string emailMinuscules, CancellationToken ct);
    void Ajouter(Administrateur administrateur);

    Task<JetonSession?> SessionParEmpreinteAsync(string empreinte, CancellationToken ct);
    void AjouterSession(JetonSession session);
}
