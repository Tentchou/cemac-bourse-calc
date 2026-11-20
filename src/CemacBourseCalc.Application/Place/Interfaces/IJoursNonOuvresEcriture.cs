using CemacBourseCalc.Domain.Place;

namespace CemacBourseCalc.Application.Place.Interfaces;

/// <summary>Écriture du calendrier de la place (EF Core). Les ajouts sont enregistrés par l'unité de travail.</summary>
public interface IJoursNonOuvresEcriture
{
    Task<bool> ExisteAsync(DateOnly date, CancellationToken ct);
    void Ajouter(JourNonOuvre jour);
}
