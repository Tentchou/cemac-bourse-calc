using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using Dapper;
using Npgsql;

namespace CemacBourseCalc.Infrastructure.Lecture;

/// <summary>
/// Lecture des titres avec Dapper : requêtes paramétrées, les seules colonnes utiles.
/// Les dates passent en DateTime puis sont converties : Dapper ne connaît pas DateOnly.
/// </summary>
public sealed class TitresLecture(NpgsqlDataSource source) : ITitresLecture
{
    // Le dernier cours : la séance la plus récente (index unique ix_cours_titre_id_date_seance),
    // cours de clôture, à défaut cours de référence.
    private const string DernierCours = """
        LEFT JOIN LATERAL (
            SELECT COALESCE(c.cours_cloture, c.cours_reference) AS cours, c.date_seance
            FROM cours c
            WHERE c.titre_id = t.id
            ORDER BY c.date_seance DESC
            LIMIT 1
        ) dc ON TRUE
        """;

    private const string SqlListe = $"""
        SELECT t.id            AS Id,
               t.isin          AS Isin,
               t.nom           AS Nom,
               e.nom           AS Emetteur,
               e.pays_code     AS Pays,
               t.type_titre    AS TypeTitre,
               t.nominal       AS Nominal,
               t.statut        AS Statut,
               o.taux_facial   AS TauxFacial,
               o.date_echeance AS DateEcheance,
               dc.cours        AS DernierCours,
               dc.date_seance  AS DateDernierCours
        FROM titre t
        JOIN emetteur e ON e.id = t.emetteur_id
        LEFT JOIN obligation o ON o.titre_id = t.id
        {DernierCours}
        ORDER BY t.nom
        """;

    private const string SqlFiche = $"""
        SELECT t.id                  AS Id,
               t.isin                AS Isin,
               t.nom                 AS Nom,
               e.nom                 AS Emetteur,
               e.pays_code           AS Pays,
               t.type_titre          AS TypeTitre,
               t.nominal             AS Nominal,
               t.statut              AS Statut,
               dc.cours              AS DernierCours,
               dc.date_seance        AS DateDernierCours,
               o.taux_facial         AS TauxFacial,
               o.periodicite         AS Periodicite,
               o.mode_remboursement  AS ModeRemboursement,
               o.date_jouissance     AS DateJouissance,
               o.date_premier_coupon AS DatePremierCoupon,
               o.date_echeance       AS DateEcheance,
               o.code_convention     AS CodeConvention,
               o.regime_fiscal       AS RegimeFiscal,
               a.nombre_titres       AS NombreTitres
        FROM titre t
        JOIN emetteur e ON e.id = t.emetteur_id
        LEFT JOIN obligation o ON o.titre_id = t.id
        LEFT JOIN action a ON a.titre_id = t.id
        {DernierCours}
        WHERE t.id = @Id
        """;

    // Index ix_dividende_titre_id_exercice. Borné : une fiche n'affiche pas un historique sans fin.
    private const string SqlDividendes = """
        SELECT exercice         AS Exercice,
               montant_brut     AS Brut,
               montant_net      AS Net,
               date_detachement AS DateDetachement,
               date_paiement    AS DatePaiement
        FROM dividende
        WHERE titre_id = @Id
        ORDER BY exercice DESC, date_paiement DESC NULLS LAST
        LIMIT 50
        """;

    public async Task<IReadOnlyList<TitreResumeDto>> ListerAsync(CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        var lignes = await connexion.QueryAsync<LigneTitre>(new CommandDefinition(SqlListe, cancellationToken: ct));
        return lignes.Select(l => new TitreResumeDto(
                l.Id, l.Isin, l.Nom, l.Emetteur, l.Pays, l.TypeTitre, l.Nominal, l.Statut,
                l.TauxFacial, Date(l.DateEcheance), l.DernierCours, Date(l.DateDernierCours)))
            .ToList();
    }

    public async Task<FicheTitreDto?> FicheAsync(Guid id, CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        var l = await connexion.QuerySingleOrDefaultAsync<LigneFiche>(
            new CommandDefinition(SqlFiche, new { Id = id }, cancellationToken: ct));
        if (l is null) return null;

        FicheObligationDto? obligation = l.TauxFacial is { } taux
            ? new FicheObligationDto(taux, l.Periodicite!, l.ModeRemboursement!, Date(l.DateJouissance)!.Value,
                Date(l.DatePremierCoupon), Date(l.DateEcheance)!.Value, l.CodeConvention!, l.RegimeFiscal!)
            : null;

        FicheActionDto? action = null;
        if (l.NombreTitres is { } nombre)
        {
            var dividendes = await connexion.QueryAsync<LigneDividende>(
                new CommandDefinition(SqlDividendes, new { Id = id }, cancellationToken: ct));
            action = new FicheActionDto(nombre, dividendes
                .Select(d => new DividendeDto(d.Exercice, d.Brut, d.Net, Date(d.DateDetachement), Date(d.DatePaiement)))
                .ToList());
        }

        return new FicheTitreDto(l.Id, l.Isin, l.Nom, l.Emetteur, l.Pays, l.TypeTitre, l.Nominal, l.Statut,
            l.DernierCours, Date(l.DateDernierCours), obligation, action);
    }

    private static DateOnly? Date(DateTime? date) => date is { } d ? DateOnly.FromDateTime(d) : null;

    private class LigneTitre
    {
        public Guid Id { get; init; }
        public string Isin { get; init; } = null!;
        public string Nom { get; init; } = null!;
        public string Emetteur { get; init; } = null!;
        public string Pays { get; init; } = null!;
        public string TypeTitre { get; init; } = null!;
        public decimal Nominal { get; init; }
        public string Statut { get; init; } = null!;
        public decimal? TauxFacial { get; init; }
        public DateTime? DateEcheance { get; init; }
        public decimal? DernierCours { get; init; }
        public DateTime? DateDernierCours { get; init; }
    }

    private sealed class LigneFiche : LigneTitre
    {
        public string? Periodicite { get; init; }
        public string? ModeRemboursement { get; init; }
        public DateTime? DateJouissance { get; init; }
        public DateTime? DatePremierCoupon { get; init; }
        public string? CodeConvention { get; init; }
        public string? RegimeFiscal { get; init; }
        public long? NombreTitres { get; init; }
    }

    private sealed class LigneDividende
    {
        public int Exercice { get; init; }
        public decimal Brut { get; init; }
        public decimal? Net { get; init; }
        public DateTime? DateDetachement { get; init; }
        public DateTime? DatePaiement { get; init; }
    }
}
