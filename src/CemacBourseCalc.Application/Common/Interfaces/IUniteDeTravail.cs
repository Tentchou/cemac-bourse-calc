namespace CemacBourseCalc.Application.Common.Interfaces;

/// <summary>
/// Enregistre en une fois tout ce que les dépôts ont ajouté (l'entité et sa ligne de journal ensemble).
/// La transaction sert à la saisie en masse : tout est enregistré, ou rien.
/// </summary>
public interface IUniteDeTravail
{
    Task EnregistrerAsync(CancellationToken ct);
    Task<ITransactionEcriture> CommencerTransactionAsync(CancellationToken ct);
}

/// <summary>Une transaction ouverte. Sans <see cref="ValiderAsync"/>, elle est annulée à sa libération.</summary>
public interface ITransactionEcriture : IAsyncDisposable
{
    Task ValiderAsync(CancellationToken ct);
    Task AnnulerAsync(CancellationToken ct);
}
