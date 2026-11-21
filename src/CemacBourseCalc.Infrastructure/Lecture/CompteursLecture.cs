using CemacBourseCalc.Application.Administration.Dtos;
using CemacBourseCalc.Application.Administration.Interfaces;
using Dapper;
using Npgsql;

namespace CemacBourseCalc.Infrastructure.Lecture;

/// <summary>Les compteurs du menu : six comptages dans une seule requête.</summary>
public sealed class CompteursLecture(NpgsqlDataSource source) : ICompteursLecture
{
    private const string Sql = """
        SELECT (SELECT COUNT(*) FROM obligation)::int     AS Obligations,
               (SELECT COUNT(*) FROM action)::int         AS Actions,
               (SELECT COUNT(*) FROM cours)::int          AS Cours,
               (SELECT COUNT(*) FROM dividende)::int      AS Dividendes,
               (SELECT COUNT(*) FROM emetteur)::int       AS Emetteurs,
               (SELECT COUNT(*) FROM jour_non_ouvre)::int AS JoursNonOuvres
        """;

    public async Task<CompteursAdminDto> CompteursAsync(CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        return await connexion.QuerySingleAsync<CompteursAdminDto>(new CommandDefinition(Sql, cancellationToken: ct));
    }
}
