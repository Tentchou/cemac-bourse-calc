using CemacBourseCalc.Domain.Titres;

namespace CemacBourseCalc.Application.Titres.Interfaces;

/// <summary>Écritures des titres (EF Core), réservées à l'administration.</summary>
public interface ITitresEcriture
{
    Task<bool> IsinExisteAsync(string isin, CancellationToken ct);
    Task<bool> EmetteurExisteAsync(Guid emetteurId, CancellationToken ct);
    Task AjouterAsync(Titre titre, CancellationToken ct);
}
