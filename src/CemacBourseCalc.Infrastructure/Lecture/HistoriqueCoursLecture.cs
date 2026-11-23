using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Domain.Communs;
using Dapper;
using Npgsql;

namespace CemacBourseCalc.Infrastructure.Lecture;

/// <summary>
/// Historique des cours d'un titre : un parcours de l'index unique ix_cours_titre_id_date_seance,
/// borné par LIMIT, trois colonnes. L'existence du titre n'est vérifiée que s'il n'a aucun cours.
/// Un titre radié est traité comme inconnu : il n'est plus public.
/// </summary>
public sealed class HistoriqueCoursLecture(NpgsqlDataSource source) : IHistoriqueCoursLecture
{
    private const string SqlHistorique = $"""
        SELECT h.date_seance AS DateSeance, h.cours AS Cours, h.volume AS Volume
        FROM (
            SELECT c.date_seance, COALESCE(c.cours_cloture, c.cours_reference) AS cours, c.volume
            FROM cours c
            WHERE c.titre_id = @Id
              AND EXISTS (SELECT 1 FROM titre t WHERE t.id = @Id AND t.statut <> '{nameof(StatutTitre.Radie)}')
            ORDER BY c.date_seance DESC
            LIMIT @Maximum
        ) h
        ORDER BY h.date_seance
        """;

    private const string SqlExiste = $"SELECT EXISTS (SELECT 1 FROM titre WHERE id = @Id AND statut <> '{nameof(StatutTitre.Radie)}')";

    public async Task<IReadOnlyList<PointCoursDto>?> HistoriqueAsync(Guid titreId, int maximum, CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        var lignes = (await connexion.QueryAsync<LignePoint>(
            new CommandDefinition(SqlHistorique, new { Id = titreId, Maximum = maximum }, cancellationToken: ct))).AsList();

        if (lignes.Count == 0)
        {
            var existe = await connexion.ExecuteScalarAsync<bool>(new CommandDefinition(SqlExiste, new { Id = titreId }, cancellationToken: ct));
            return existe ? [] : null;
        }

        return lignes.Select(l => new PointCoursDto(DateOnly.FromDateTime(l.DateSeance), l.Cours, l.Volume)).ToList();
    }

    private sealed class LignePoint
    {
        public DateTime DateSeance { get; init; }
        public decimal Cours { get; init; }
        public long? Volume { get; init; }
    }
}
