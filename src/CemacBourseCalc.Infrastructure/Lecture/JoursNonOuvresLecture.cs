using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Place.Dtos;
using CemacBourseCalc.Application.Place.Interfaces;
using CemacBourseCalc.Application.Place.Queries.ListerJoursNonOuvres;
using Dapper;
using Npgsql;

namespace CemacBourseCalc.Infrastructure.Lecture;

/// <summary>Le calendrier de la place pour l'administration. L'année devient un intervalle de dates (clé primaire utilisée).</summary>
public sealed class JoursNonOuvresLecture(NpgsqlDataSource source) : IJoursNonOuvresLecture
{
    private const string Depuis = """
        FROM jour_non_ouvre j
        WHERE (@Du::date IS NULL OR j.date BETWEEN @Du::date AND @Au::date)
          AND (@Motif::text IS NULL OR j.libelle ILIKE @Motif ESCAPE '\')
        """;

    private const string SqlPage = $"""
        SELECT j.date AS Date, j.libelle AS Libelle
        {Depuis}
        ORDER BY j.date DESC
        LIMIT @Taille OFFSET @Decalage
        """;

    // L'année entière, sur la clé primaire (date) : un parcours d'intervalle.
    private const string SqlAnnee = "SELECT j.date AS Date, j.libelle AS Libelle FROM jour_non_ouvre j WHERE j.date BETWEEN @Du::date AND @Au::date ORDER BY j.date";

    public async Task<IReadOnlyList<JourNonOuvreDto>> DeLAnneeAsync(int annee, CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        var lignes = await connexion.QueryAsync<Ligne>(new CommandDefinition(SqlAnnee,
            new { Du = new DateTime(annee, 1, 1), Au = new DateTime(annee, 12, 31) }, cancellationToken: ct));
        return lignes.Select(l => new JourNonOuvreDto(DateOnly.FromDateTime(l.Date), l.Libelle)).ToList();
    }

    public Task<PageDto<JourNonOuvreDto>> PageAsync(ListerJoursNonOuvresQuery q, CancellationToken ct)
    {
        var parametres = new DynamicParameters(new
        {
            Motif = Pagination.Motif(q.Recherche),
            Du = q.Annee is { } a ? new DateTime(a, 1, 1) : (DateTime?)null,
            Au = q.Annee is { } b ? new DateTime(b, 12, 31) : (DateTime?)null,
        });
        return LecturePaginee.LireAsync<Ligne, JourNonOuvreDto>(source, $"SELECT COUNT(*) {Depuis}", SqlPage, q, parametres,
            l => new JourNonOuvreDto(DateOnly.FromDateTime(l.Date), l.Libelle), ct);
    }

    private sealed class Ligne
    {
        public DateTime Date { get; init; }
        public string Libelle { get; init; } = null!;
    }
}
