using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Application.Titres.Queries.ListerCoursAdmin;
using CemacBourseCalc.Application.Titres.Queries.ListerDividendesAdmin;
using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using Dapper;
using Npgsql;
using static CemacBourseCalc.Infrastructure.Lecture.LecturePaginee;

namespace CemacBourseCalc.Infrastructure.Lecture;

/// <summary>Les listes paginées des cours et des dividendes pour l'administration. Requêtes paramétrées uniquement.</summary>
public sealed class MarcheAdminLecture(NpgsqlDataSource source) : IMarcheAdminLecture
{
    private const string FiltreRecherche =
        @"(@Motif::text IS NULL OR t.nom ILIKE @Motif ESCAPE '\' OR t.isin ILIKE @Motif ESCAPE '\')";

    // Sans séance précisée : la dernière séance saisie (index ix_cours_date_seance : lecture directe du maximum).
    private const string DepuisCours = $"""
        FROM cours c
        JOIN titre t ON t.id = c.titre_id
        LEFT JOIN obligation o ON o.titre_id = t.id
        WHERE c.date_seance = COALESCE(@Seance::date, (SELECT MAX(date_seance) FROM cours))
          AND {FiltreRecherche}
        """;

    private const string SqlCours = $"""
        SELECT c.id AS Id, t.id AS TitreId, t.isin AS Isin, t.nom AS Nom, t.type_titre AS TypeTitre,
               c.date_seance AS DateSeance, c.cours_reference AS CoursReference, c.cours_cloture AS CoursCloture,
               c.volume AS Volume, c.coupon_couru_publie AS CouponCouruPublie,
               t.nominal AS Nominal, o.taux_facial AS TauxFacial, o.periodicite AS Periodicite,
               o.date_jouissance AS DateJouissance, o.date_premier_coupon AS DatePremierCoupon,
               o.date_echeance AS DateEcheance, o.mode_remboursement AS ModeRemboursement,
               o.differe_echeances AS DiffereEcheances, o.code_convention AS CodeConvention
        {DepuisCours}
        ORDER BY t.nom, c.id
        LIMIT @Taille OFFSET @Decalage
        """;

    private const string SqlSeances = """
        SELECT DISTINCT date_seance FROM cours ORDER BY date_seance DESC LIMIT @Nombre
        """;

    private const string DepuisDividendes = $"""
        FROM dividende d
        JOIN titre t ON t.id = d.titre_id
        WHERE {FiltreRecherche}
        """;

    private const string SqlDividendes = $"""
        SELECT d.id AS Id, t.id AS TitreId, t.isin AS Isin, t.nom AS Nom, d.exercice AS Exercice,
               d.montant_brut AS MontantBrut, d.montant_net AS MontantNet,
               d.date_detachement AS DateDetachement, d.date_paiement AS DatePaiement
        {DepuisDividendes}
        ORDER BY d.exercice DESC, t.nom, d.date_paiement DESC NULLS LAST, d.id
        LIMIT @Taille OFFSET @Decalage
        """;

    public Task<PageDto<CoursAdminLigne>> CoursAsync(ListerCoursAdminQuery q, CancellationToken ct)
    {
        var parametres = new DynamicParameters(new
        {
            Motif = Pagination.Motif(q.Recherche),
            Seance = q.DateSeance?.ToDateTime(TimeOnly.MinValue),
        });
        return LireAsync<LigneCours, CoursAdminLigne>(source, $"SELECT COUNT(*) {DepuisCours}", SqlCours, q, parametres,
            l => new CoursAdminLigne(
                new CoursAdminDto(l.Id, l.TitreId, l.Isin, l.Nom, l.TypeTitre, DateOnly.FromDateTime(l.DateSeance),
                    l.CoursReference, l.CoursCloture, l.Volume, l.CouponCouruPublie),
                l.TauxFacial is { } taux
                    ? new CaracteristiquesObligation(l.Nom, l.Nominal, taux, Enum.Parse<Periodicite>(l.Periodicite!),
                        DateOnly.FromDateTime(l.DateJouissance!.Value), Date(l.DatePremierCoupon),
                        DateOnly.FromDateTime(l.DateEcheance!.Value), Enum.Parse<ModeRemboursement>(l.ModeRemboursement!),
                        l.DiffereEcheances!.Value, l.CodeConvention!)
                    : null),
            ct);
    }

    public async Task<IReadOnlyList<DateOnly>> SeancesAsync(int nombre, CancellationToken ct)
    {
        await using var connexion = await source.OpenConnectionAsync(ct);
        var dates = await connexion.QueryAsync<DateTime>(new CommandDefinition(SqlSeances, new { Nombre = nombre }, cancellationToken: ct));
        return dates.Select(DateOnly.FromDateTime).ToList();
    }

    public Task<PageDto<DividendeAdminDto>> DividendesAsync(ListerDividendesAdminQuery q, CancellationToken ct)
    {
        var parametres = new DynamicParameters(new { Motif = Pagination.Motif(q.Recherche) });
        return LireAsync<LigneDividende, DividendeAdminDto>(source, $"SELECT COUNT(*) {DepuisDividendes}", SqlDividendes,
            q, parametres, l => new DividendeAdminDto(l.Id, l.TitreId, l.Isin, l.Nom, l.Exercice, l.MontantBrut, l.MontantNet,
                Date(l.DateDetachement), Date(l.DatePaiement)),
            ct);
    }

    private sealed class LigneCours
    {
        public Guid Id { get; init; }
        public Guid TitreId { get; init; }
        public string Isin { get; init; } = null!;
        public string Nom { get; init; } = null!;
        public string TypeTitre { get; init; } = null!;
        public DateTime DateSeance { get; init; }
        public decimal? CoursReference { get; init; }
        public decimal? CoursCloture { get; init; }
        public long? Volume { get; init; }
        public decimal? CouponCouruPublie { get; init; }
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

    private sealed class LigneDividende
    {
        public Guid Id { get; init; }
        public Guid TitreId { get; init; }
        public string Isin { get; init; } = null!;
        public string Nom { get; init; } = null!;
        public int Exercice { get; init; }
        public decimal MontantBrut { get; init; }
        public decimal? MontantNet { get; init; }
        public DateTime? DateDetachement { get; init; }
        public DateTime? DatePaiement { get; init; }
    }
}
