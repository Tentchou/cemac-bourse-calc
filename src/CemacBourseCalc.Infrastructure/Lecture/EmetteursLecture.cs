using CemacBourseCalc.Application.Referentiel.Dtos;
using CemacBourseCalc.Application.Referentiel.Interfaces;
using Dapper;
using Npgsql;

namespace CemacBourseCalc.Infrastructure.Lecture;

public sealed class EmetteursLecture(NpgsqlDataSource source) : IEmetteursLecture
{
    private const string Sql = """
        SELECT id AS Id, nom AS Nom, sigle AS Sigle, type AS Type, pays_code AS Pays
        FROM emetteur
        ORDER BY nom
        """;

    public async Task<IReadOnlyList<EmetteurResumeDto>> ListerAsync(CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        var lignes = await connexion.QueryAsync<EmetteurResumeDto>(new CommandDefinition(Sql, cancellationToken: ct));
        return lignes.AsList();
    }
}
