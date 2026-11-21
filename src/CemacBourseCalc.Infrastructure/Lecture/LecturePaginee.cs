using CemacBourseCalc.Application.Common.Pagination;
using Dapper;
using Npgsql;

namespace CemacBourseCalc.Infrastructure.Lecture;

/// <summary>
/// Lecture d'une page en un seul aller-retour : le nombre total de lignes du filtre, puis les lignes de la page
/// (deux requêtes envoyées ensemble). Le total est juste même quand la page demandée dépasse la fin de la liste.
/// Les requêtes reçoivent toujours @Taille et @Decalage, en plus de leurs propres paramètres.
/// </summary>
internal static class LecturePaginee
{
    public static async Task<PageDto<TDto>> LireAsync<TLigne, TDto>(
        NpgsqlDataSource source, string sqlTotal, string sqlLignes, IRequetePaginee q,
        DynamicParameters parametres, Func<TLigne, TDto> convertir, CancellationToken ct)
    {
        parametres.Add("Taille", q.Taille);
        parametres.Add("Decalage", Pagination.Decalage(q.Page, q.Taille));

        await using var connexion = await source.OpenConnectionAsync(ct);
        await using var lecture = await connexion.QueryMultipleAsync(
            new CommandDefinition($"{sqlTotal};\n{sqlLignes}", parametres, cancellationToken: ct));

        var total = await lecture.ReadSingleAsync<long>();
        var lignes = (await lecture.ReadAsync<TLigne>()).Select(convertir).ToList();
        return new PageDto<TDto>(lignes, (int)Math.Min(total, int.MaxValue), q.Page, q.Taille);
    }

    public static DateOnly? Date(DateTime? date) => date is { } d ? DateOnly.FromDateTime(d) : null;
}
