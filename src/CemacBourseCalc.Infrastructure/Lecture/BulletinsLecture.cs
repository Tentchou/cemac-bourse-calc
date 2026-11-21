using CemacBourseCalc.Application.Bulletins.Interfaces;
using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using Dapper;
using Npgsql;

namespace CemacBourseCalc.Infrastructure.Lecture;

/// <summary>Ce que l'analyse d'un bulletin lit en base : les titres (une requête) et les cours déjà saisis d'une séance.</summary>
public sealed class BulletinsLecture(NpgsqlDataSource source) : IBulletinsLecture
{
    private const string SqlTitres = """
        SELECT t.id AS Id, t.isin AS Isin, t.nom AS Nom, t.type_titre AS TypeTitre, t.statut AS Statut, t.nominal AS Nominal,
               o.taux_facial AS TauxFacial, o.periodicite AS Periodicite, o.date_jouissance AS DateJouissance,
               o.date_premier_coupon AS DatePremierCoupon, o.date_echeance AS DateEcheance,
               o.mode_remboursement AS ModeRemboursement, o.differe_echeances AS DiffereEcheances, o.code_convention AS CodeConvention
        FROM titre t
        LEFT JOIN obligation o ON o.titre_id = t.id
        """;

    // Index ix_cours_date_seance.
    private const string SqlCoursSeance = "SELECT COUNT(*)::int FROM cours WHERE date_seance = @Date::date";

    public async Task<IReadOnlyList<TitreReference>> TitresAsync(CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        var lignes = await connexion.QueryAsync<Ligne>(new CommandDefinition(SqlTitres, cancellationToken: ct));
        return lignes.Select(l => new TitreReference(l.Id, l.Isin, l.Nom, l.TypeTitre, l.Statut,
                l.TauxFacial is { } taux
                    ? new CaracteristiquesObligation(l.Nom, l.Nominal, taux, Enum.Parse<Periodicite>(l.Periodicite!),
                        DateOnly.FromDateTime(l.DateJouissance!.Value), LecturePaginee.Date(l.DatePremierCoupon),
                        DateOnly.FromDateTime(l.DateEcheance!.Value), Enum.Parse<ModeRemboursement>(l.ModeRemboursement!),
                        l.DiffereEcheances!.Value, l.CodeConvention!)
                    : null))
            .ToList();
    }

    public async Task<int> CoursDeLaSeanceAsync(DateOnly dateSeance, CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        return await connexion.ExecuteScalarAsync<int>(new CommandDefinition(SqlCoursSeance,
            new { Date = dateSeance.ToDateTime(TimeOnly.MinValue) }, cancellationToken: ct));
    }

    private sealed class Ligne
    {
        public Guid Id { get; init; }
        public string Isin { get; init; } = null!;
        public string Nom { get; init; } = null!;
        public string TypeTitre { get; init; } = null!;
        public string Statut { get; init; } = null!;
        public decimal Nominal { get; init; }
        public decimal? TauxFacial { get; init; }
        public string? Periodicite { get; init; }
        public DateTime? DateJouissance { get; init; }
        public DateTime? DatePremierCoupon { get; init; }
        public DateTime? DateEcheance { get; init; }
        public string? ModeRemboursement { get; init; }
        public int? DiffereEcheances { get; init; }
        public string? CodeConvention { get; init; }
    }
}
