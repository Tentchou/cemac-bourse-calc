using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Titres.Dtos;
using CemacBourseCalc.Application.Titres.Interfaces;
using CemacBourseCalc.Application.Titres.Queries.ListerActionsAdmin;
using CemacBourseCalc.Application.Titres.Queries.ListerObligationsAdmin;
using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using Dapper;
using Npgsql;
using static CemacBourseCalc.Infrastructure.Lecture.LecturePaginee;

namespace CemacBourseCalc.Infrastructure.Lecture;

/// <summary>
/// Les listes paginées des titres pour l'administration. Requêtes paramétrées uniquement : la recherche
/// est un motif ILIKE échappé, les filtres sont des valeurs déjà contrôlées par les validateurs.
/// </summary>
public sealed class TitresAdminLecture(NpgsqlDataSource source) : ITitresAdminLecture
{
    // Recherche sur le titre et son émetteur ; ESCAPE précise le caractère d'échappement du motif.
    private const string FiltreTitre = """
        (@Motif::text IS NULL
            OR t.nom ILIKE @Motif ESCAPE '\' OR t.isin ILIKE @Motif ESCAPE '\'
            OR e.nom ILIKE @Motif ESCAPE '\' OR e.sigle ILIKE @Motif ESCAPE '\')
        AND (@Pays::text IS NULL OR e.pays_code = @Pays)
        """;

    private const string TablesObligations = """
        FROM titre t
        JOIN obligation o ON o.titre_id = t.id
        JOIN emetteur e ON e.id = t.emetteur_id
        """;

    private const string FiltreObligations = $"""
        WHERE {FiltreTitre}
          AND (@Remboursement::text IS NULL OR o.mode_remboursement = @Remboursement)
        """;

    // Le dernier coupon couru publié : la séance la plus récente qui en porte un (index ix_cours_titre_id_date_seance).
    private const string SqlObligations = $"""
        SELECT t.id AS Id, t.isin AS Isin, t.nom AS Nom, COALESCE(e.sigle, e.nom) AS Emetteur, e.pays_code AS Pays,
               t.nominal AS Nominal, o.taux_facial AS TauxFacial, o.periodicite AS Periodicite,
               o.mode_remboursement AS ModeRemboursement, o.differe_echeances AS DiffereEcheances,
               o.date_jouissance AS DateJouissance, o.date_premier_coupon AS DatePremierCoupon,
               o.date_echeance AS DateEcheance, o.code_convention AS CodeConvention, o.regime_fiscal AS RegimeFiscal,
               cc.date_seance AS DateSeance, cc.coupon_couru_publie AS CouponPublie
        {TablesObligations}
        LEFT JOIN LATERAL (
            SELECT c.date_seance, c.coupon_couru_publie
            FROM cours c
            WHERE c.titre_id = t.id AND c.coupon_couru_publie IS NOT NULL
            ORDER BY c.date_seance DESC
            LIMIT 1
        ) cc ON TRUE
        {FiltreObligations}
        ORDER BY t.nom, t.id
        LIMIT @Taille OFFSET @Decalage
        """;

    private const string TablesActions = """
        FROM titre t
        JOIN action a ON a.titre_id = t.id
        JOIN emetteur e ON e.id = t.emetteur_id
        """;

    private const string SqlActions = $"""
        SELECT t.id AS Id, t.isin AS Isin, t.nom AS Nom, COALESCE(e.sigle, e.nom) AS Emetteur, e.pays_code AS Pays,
               t.nominal AS Nominal, a.nombre_titres AS NombreTitres, t.date_admission AS DateAdmission,
               dc.cours AS DernierCours, dc.date_seance AS DateDernierCours
        {TablesActions}
        LEFT JOIN LATERAL (
            SELECT COALESCE(c.cours_cloture, c.cours_reference) AS cours, c.date_seance
            FROM cours c
            WHERE c.titre_id = t.id
            ORDER BY c.date_seance DESC
            LIMIT 1
        ) dc ON TRUE
        WHERE {FiltreTitre}
        ORDER BY t.nom, t.id
        LIMIT @Taille OFFSET @Decalage
        """;

    public Task<PageDto<ObligationAdminLigne>> ObligationsAsync(ListerObligationsAdminQuery q, CancellationToken ct)
    {
        var parametres = new DynamicParameters(new { Motif = Pagination.Motif(q.Recherche), q.Pays, q.Remboursement });
        return LireAsync<LigneObligation, ObligationAdminLigne>(source, $"SELECT COUNT(*) {TablesObligations} {FiltreObligations}", SqlObligations,
            q, parametres, l => new ObligationAdminLigne(
                new ObligationAdminDto(l.Id, l.Isin, l.Nom, l.Emetteur, l.Pays, l.Nominal, l.TauxFacial, l.Periodicite,
                    l.ModeRemboursement, l.DiffereEcheances, DateOnly.FromDateTime(l.DateJouissance),
                    DateOnly.FromDateTime(l.DateEcheance), l.CodeConvention, l.RegimeFiscal),
                new CaracteristiquesObligation(l.Nom, l.Nominal, l.TauxFacial, Enum.Parse<Periodicite>(l.Periodicite),
                    DateOnly.FromDateTime(l.DateJouissance), Date(l.DatePremierCoupon), DateOnly.FromDateTime(l.DateEcheance),
                    Enum.Parse<ModeRemboursement>(l.ModeRemboursement), l.DiffereEcheances, l.CodeConvention),
                Date(l.DateSeance), l.CouponPublie),
            ct);
    }

    public Task<PageDto<ActionAdminDto>> ActionsAsync(ListerActionsAdminQuery q, CancellationToken ct)
    {
        var parametres = new DynamicParameters(new { Motif = Pagination.Motif(q.Recherche), q.Pays });
        return LireAsync<LigneAction, ActionAdminDto>(source, $"SELECT COUNT(*) {TablesActions} WHERE {FiltreTitre}", SqlActions,
            q, parametres, l => new ActionAdminDto(l.Id, l.Isin, l.Nom, l.Emetteur, l.Pays, l.Nominal, l.NombreTitres,
                Date(l.DateAdmission), l.DernierCours, Date(l.DateDernierCours)),
            ct);
    }

    private sealed class LigneObligation
    {
        public Guid Id { get; init; }
        public string Isin { get; init; } = null!;
        public string Nom { get; init; } = null!;
        public string Emetteur { get; init; } = null!;
        public string Pays { get; init; } = null!;
        public decimal Nominal { get; init; }
        public decimal TauxFacial { get; init; }
        public string Periodicite { get; init; } = null!;
        public string ModeRemboursement { get; init; } = null!;
        public int DiffereEcheances { get; init; }
        public DateTime DateJouissance { get; init; }
        public DateTime? DatePremierCoupon { get; init; }
        public DateTime DateEcheance { get; init; }
        public string CodeConvention { get; init; } = null!;
        public string RegimeFiscal { get; init; } = null!;
        public DateTime? DateSeance { get; init; }
        public decimal? CouponPublie { get; init; }
    }

    private sealed class LigneAction
    {
        public Guid Id { get; init; }
        public string Isin { get; init; } = null!;
        public string Nom { get; init; } = null!;
        public string Emetteur { get; init; } = null!;
        public string Pays { get; init; } = null!;
        public decimal Nominal { get; init; }
        public long NombreTitres { get; init; }
        public DateTime? DateAdmission { get; init; }
        public decimal? DernierCours { get; init; }
        public DateTime? DateDernierCours { get; init; }
    }
}
