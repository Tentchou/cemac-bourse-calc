using CemacBourseCalc.Application.Administration.Dtos;

namespace CemacBourseCalc.Application.Administration.Interfaces;

/// <summary>Lecture de la session à chaque requête de l'administration (Dapper, une requête indexée).</summary>
public interface ISessionsLecture
{
    /// <summary>L'administrateur d'une session ouverte, non expirée, non révoquée et d'un compte actif ; sinon null.</summary>
    Task<AdministrateurConnecteDto?> AdministrateurDeSessionAsync(string empreinte, DateTimeOffset maintenant, CancellationToken ct);
}
