using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using Dapper;
using Npgsql;

namespace CemacBourseCalc.Infrastructure.Lecture;

/// <summary>Lecture des titres avec Dapper : une requête, les seules colonnes utiles.</summary>
public sealed class TitresLecture(NpgsqlDataSource source) : ITitresLecture
{
    private const string Sql = """
        SELECT t.id          AS Id,
               t.isin        AS Isin,
               t.nom         AS Nom,
               e.nom         AS Emetteur,
               e.pays_code   AS Pays,
               t.type_titre  AS TypeTitre,
               t.nominal     AS Nominal,
               t.statut      AS Statut
        FROM titre t
        JOIN emetteur e ON e.id = t.emetteur_id
        ORDER BY t.nom
        """;

    public async Task<IReadOnlyList<TitreResumeDto>> ListerAsync(CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        var lignes = await connexion.QueryAsync<TitreResumeDto>(new CommandDefinition(Sql, cancellationToken: ct));
        return lignes.AsList();
    }
}
