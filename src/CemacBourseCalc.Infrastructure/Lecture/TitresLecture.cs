using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using Dapper;
using Npgsql;

namespace CemacBourseCalc.Infrastructure.Lecture;

/// <summary>Lecture des titres avec Dapper : une requête, les seules colonnes utiles.</summary>
public sealed class TitresLecture(NpgsqlDataSource source) : ITitresLecture
{
    private const string Sql = """
        SELECT id         AS Id,
               isin       AS Isin,
               nom        AS Nom,
               emetteur   AS Emetteur,
               left(isin, 2) AS Pays,
               type_titre AS TypeTitre,
               nominal    AS Nominal
        FROM titre
        ORDER BY nom
        """;

    public async Task<IReadOnlyList<TitreResumeDto>> ListerAsync(CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        var lignes = await connexion.QueryAsync<TitreResumeDto>(new CommandDefinition(Sql, cancellationToken: ct));
        return lignes.AsList();
    }
}
