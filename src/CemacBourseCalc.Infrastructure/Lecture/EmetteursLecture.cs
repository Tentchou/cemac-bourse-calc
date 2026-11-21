using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Referentiel.Dtos;
using CemacBourseCalc.Application.Referentiel.Interfaces;
using CemacBourseCalc.Application.Referentiel.Queries.ListerEmetteursPage;
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

    private const string Depuis = """
        FROM emetteur e
        WHERE (@Motif::text IS NULL OR e.nom ILIKE @Motif ESCAPE '\' OR e.sigle ILIKE @Motif ESCAPE '\')
          AND (@Pays::text IS NULL OR e.pays_code = @Pays)
          AND (@Type::text IS NULL OR e.type = @Type)
        """;

    // Le nombre de titres : index ix_titre_emetteur_id, compté pour les seules lignes de la page.
    private const string SqlPage = $"""
        SELECT e.id AS Id, e.nom AS Nom, e.sigle AS Sigle, e.type AS Type, e.pays_code AS Pays, e.site_web AS SiteWeb,
               (SELECT COUNT(*) FROM titre t WHERE t.emetteur_id = e.id)::int AS NombreTitres
        {Depuis}
        ORDER BY e.nom, e.id
        LIMIT @Taille OFFSET @Decalage
        """;

    public async Task<IReadOnlyList<EmetteurResumeDto>> ListerAsync(CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        var lignes = await connexion.QueryAsync<EmetteurResumeDto>(new CommandDefinition(Sql, cancellationToken: ct));
        return lignes.AsList();
    }

    public Task<PageDto<EmetteurAdminDto>> PageAsync(ListerEmetteursPageQuery q, CancellationToken ct)
    {
        var parametres = new DynamicParameters(new { Motif = Pagination.Motif(q.Recherche), q.Pays, q.Type });
        return LecturePaginee.LireAsync<EmetteurAdminDto, EmetteurAdminDto>(
            source, $"SELECT COUNT(*) {Depuis}", SqlPage, q, parametres, e => e, ct);
    }
}
