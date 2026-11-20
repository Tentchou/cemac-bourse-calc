using CemacBourseCalc.Application.Administration.Dtos;
using CemacBourseCalc.Application.Administration.Interfaces;
using CemacBourseCalc.Domain.Communs;
using Dapper;
using Npgsql;

namespace CemacBourseCalc.Infrastructure.Lecture;

/// <summary>Appelée à chaque requête de l'administration : une seule requête, sur l'index unique ix_jeton_session_jeton_hash.</summary>
public sealed class SessionsLecture(NpgsqlDataSource source) : ISessionsLecture
{
    private const string Sql = """
        SELECT a.id    AS Id,
               a.nom   AS Nom,
               a.email AS Email,
               a.role  AS Role
        FROM jeton_session s
        JOIN administrateur a ON a.id = s.administrateur_id
        WHERE s.jeton_hash = @Empreinte
          AND s.revoque_le IS NULL
          AND s.expire_le > @Maintenant
          AND a.actif
        """;

    public async Task<AdministrateurConnecteDto?> AdministrateurDeSessionAsync(string empreinte, DateTimeOffset maintenant, CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        var ligne = await connexion.QuerySingleOrDefaultAsync<Ligne>(new CommandDefinition(Sql,
            new { Empreinte = empreinte, Maintenant = maintenant.ToUniversalTime() }, cancellationToken: ct));

        return ligne is null ? null : new AdministrateurConnecteDto(ligne.Id, ligne.Nom, ligne.Email, Enum.Parse<RoleAdministrateur>(ligne.Role));
    }

    private sealed class Ligne
    {
        public Guid Id { get; init; }
        public string Nom { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string Role { get; init; } = null!;
    }
}
