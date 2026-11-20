using CemacBourseCalc.Domain.Referentiel;

namespace CemacBourseCalc.Application.Referentiel.Interfaces;

/// <summary>Écriture des émetteurs (EF Core). Les ajouts sont enregistrés par l'unité de travail.</summary>
public interface IEmetteursEcriture
{
    /// <summary>Comparaison sans tenir compte des majuscules : « Etat du Gabon » et « ETAT DU GABON » sont le même émetteur.</summary>
    Task<bool> NomExisteAsync(string nom, CancellationToken ct);
    void Ajouter(Emetteur emetteur);
}
