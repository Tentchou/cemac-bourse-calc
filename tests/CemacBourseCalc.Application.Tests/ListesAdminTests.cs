using CemacBourseCalc.Application.Calculs.Dtos;
using CemacBourseCalc.Application.Calculs.Interfaces;
using CemacBourseCalc.Application.Common.Pagination;
using CemacBourseCalc.Application.Place.Queries.ListerJoursNonOuvres;
using CemacBourseCalc.Application.Referentiel.Queries.ListerEmetteursPage;
using CemacBourseCalc.Application.Titres.Common;
using CemacBourseCalc.Application.Titres.Queries.ListerCoursAdmin;
using CemacBourseCalc.Application.Titres.Queries.ListerObligationsAdmin;
using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Place;

namespace CemacBourseCalc.Application.Tests;

/// <summary>Les listes paginées de l'administration : bornes, filtres, recherche et contrôle du coupon couru.</summary>
public class ListesAdminTests
{
    // ---------- Pagination ----------

    [Theory]
    [InlineData(1, 10, 0)]
    [InlineData(2, 10, 10)]
    [InlineData(3, 50, 100)]
    public void Le_decalage_saute_les_pages_precedentes(int page, int taille, int decalage) =>
        Assert.Equal(decalage, Pagination.Decalage(page, taille));

    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("   ", null)]
    [InlineData(" ecmr ", "%ecmr%")]
    [InlineData("6,25%", @"%6,25\%%")]
    [InlineData("a_b", @"%a\_b%")]
    [InlineData(@"c:\x", @"%c:\\x%")]
    [InlineData("'; DROP TABLE titre;--", "%'; DROP TABLE titre;--%")]
    public void La_recherche_devient_un_motif_dont_les_jokers_sont_neutralises(string? recherche, string? motif) =>
        Assert.Equal(motif, Pagination.Motif(recherche));

    [Theory]
    [InlineData(1, 10, true)]
    [InlineData(1, 20, true)]
    [InlineData(10_000, 50, true)]
    [InlineData(0, 10, false)]
    [InlineData(-1, 10, false)]
    [InlineData(10_001, 10, false)]
    [InlineData(1, 15, false)]
    [InlineData(1, 1000, false)]
    [InlineData(1, 0, false)]
    public void Page_et_taille_sont_bornees(int page, int taille, bool valide)
    {
        var resultat = new ListerObligationsAdminQueryValidator().Validate(new ListerObligationsAdminQuery(page, taille, null, null, null));
        Assert.Equal(valide, resultat.IsValid);
    }

    [Fact]
    public void Une_recherche_trop_longue_est_refusee()
    {
        var validateur = new ListerObligationsAdminQueryValidator();
        Assert.True(validateur.Validate(new ListerObligationsAdminQuery(1, 10, new string('a', 100), null, null)).IsValid);
        Assert.False(validateur.Validate(new ListerObligationsAdminQuery(1, 10, new string('a', 101), null, null)).IsValid);
    }

    [Theory]
    [InlineData("CM", true)]
    [InlineData("GQ", true)]
    [InlineData("cm", false)]
    [InlineData("0", false)]
    [InlineData("FR", false)]
    [InlineData("CM' OR 1=1", false)]
    public void Le_filtre_pays_n_accepte_que_les_codes_CEMAC(string pays, bool valide)
    {
        var resultat = new ListerObligationsAdminQueryValidator().Validate(new ListerObligationsAdminQuery(1, 10, null, pays, null));
        Assert.Equal(valide, resultat.IsValid);
    }

    [Theory]
    [InlineData("InFine", true)]
    [InlineData("AmortissementConstant", true)]
    [InlineData("infine", false)]
    [InlineData("1", false)]
    public void Le_filtre_remboursement_n_accepte_que_les_modes_connus(string mode, bool valide)
    {
        var resultat = new ListerObligationsAdminQueryValidator().Validate(new ListerObligationsAdminQuery(1, 10, null, null, mode));
        Assert.Equal(valide, resultat.IsValid);
    }

    [Theory]
    [InlineData("Etat", true)]
    [InlineData("Supranational", true)]
    [InlineData("État", false)]
    [InlineData("2", false)]
    public void Le_filtre_type_d_emetteur_n_accepte_que_les_types_connus(string type, bool valide)
    {
        var resultat = new ListerEmetteursPageQueryValidator().Validate(new ListerEmetteursPageQuery(1, 10, null, null, type));
        Assert.Equal(valide, resultat.IsValid);
    }

    [Theory]
    [InlineData(2026, true)]
    [InlineData(1989, false)]
    [InlineData(2101, false)]
    public void L_annee_du_calendrier_est_bornee(int annee, bool valide)
    {
        var resultat = new ListerJoursNonOuvresQueryValidator().Validate(new ListerJoursNonOuvresQuery(1, 10, null, annee));
        Assert.Equal(valide, resultat.IsValid);
    }

    [Fact]
    public void La_date_de_seance_est_bornee()
    {
        var validateur = new ListerCoursAdminQueryValidator();
        Assert.True(validateur.Validate(new ListerCoursAdminQuery(1, 10, null, new DateOnly(2026, 11, 20))).IsValid);
        Assert.False(validateur.Validate(new ListerCoursAdminQuery(1, 10, null, new DateOnly(1900, 1, 1))).IsValid);
    }

    // ---------- Contrôle du coupon couru (bulletin BVMAC n° 2643) ----------

    /// <summary>ECMR 6,25 % NET 2022-2029 (CM0000020305) : coupon couru publié 186,99 FCFA au règlement du 25/11/2026.</summary>
    private static readonly CaracteristiquesObligation Ecmr6 = new(
        "ECMR 6,25% NET 2022-2029", 10_000m, 6.25m, Periodicite.Annuelle,
        new DateOnly(2022, 5, 27), null, new DateOnly(2029, 5, 27), ModeRemboursement.AmortissementConstant, 2, "EXACT_365");

    private static readonly DateOnly Seance = new(2026, 11, 20);

    private sealed class LectureCalculsFixe(string? delai, params DateOnly[] nonOuvres) : ILectureCalculs
    {
        public int Lectures { get; private set; }

        public Task<CaracteristiquesObligation?> ObligationAsync(Guid titreId, CancellationToken ct) => throw new NotSupportedException();
        public Task<DernierDividendeLecture?> DernierDividendeAsync(Guid titreId, CancellationToken ct) => throw new NotSupportedException();

        public Task<string?> ParametreAsync(string cle, CancellationToken ct)
        {
            Lectures++;
            return Task.FromResult(cle == ClesParametres.DelaiReglement ? delai : null);
        }

        public Task<IReadOnlySet<DateOnly>> JoursNonOuvresAsync(DateOnly du, DateOnly au, CancellationToken ct)
        {
            Lectures++;
            return Task.FromResult<IReadOnlySet<DateOnly>>(nonOuvres.Where(d => d >= du && d <= au).ToHashSet());
        }
    }

    [Fact]
    public async Task Le_coupon_couru_publie_par_la_BVMAC_est_reconnu_identique()
    {
        var controles = await ControleCouponCouru.ControlerAsync(
            new LectureCalculsFixe("3"), [new ObligationAControler(Ecmr6, Seance, 186.99m)], CancellationToken.None);

        var c = Assert.Single(controles)!;
        Assert.True(c.Identique);
        Assert.Equal(new DateOnly(2026, 11, 25), c.DateReglement);
        Assert.Equal(186.99m, c.Calcule);
        Assert.Equal(0m, c.Ecart);
        Assert.Null(c.Erreur);
    }

    [Fact]
    public async Task Un_ecart_avec_la_BVMAC_est_signale_avec_son_montant()
    {
        var controles = await ControleCouponCouru.ControlerAsync(
            new LectureCalculsFixe("3"), [new ObligationAControler(Ecmr6, Seance, 186.00m)], CancellationToken.None);

        var c = controles[0]!;
        Assert.False(c.Identique);
        Assert.Equal(0.99m, c.Ecart);
    }

    [Fact]
    public async Task Un_jour_non_ouvre_decale_le_reglement_du_controle()
    {
        var ferie = new DateOnly(2026, 11, 24);
        var controles = await ControleCouponCouru.ControlerAsync(
            new LectureCalculsFixe("3", ferie), [new ObligationAControler(Ecmr6, Seance, 186.99m)], CancellationToken.None);

        Assert.Equal(new DateOnly(2026, 11, 26), controles[0]!.DateReglement);
        Assert.False(controles[0]!.Identique);
    }

    [Fact]
    public async Task Sans_delai_de_reglement_le_controle_dit_pourquoi_il_manque()
    {
        var controles = await ControleCouponCouru.ControlerAsync(
            new LectureCalculsFixe(null), [new ObligationAControler(Ecmr6, Seance, 186.99m)], CancellationToken.None);

        Assert.False(controles[0]!.Identique);
        Assert.Null(controles[0]!.Calcule);
        Assert.Equal(Calculs.Common.ReglesPlace.MessageDelaiAbsent, controles[0]!.Erreur);
    }

    [Fact]
    public async Task Les_lignes_sans_coupon_publie_restent_vides_et_la_page_ne_lit_la_base_que_deux_fois()
    {
        var lecture = new LectureCalculsFixe("3");
        var ligne = new ObligationAControler(Ecmr6, Seance, 186.99m);

        var controles = await ControleCouponCouru.ControlerAsync(lecture, [null, ligne, null, ligne, ligne], CancellationToken.None);

        Assert.Equal(5, controles.Count);
        Assert.Null(controles[0]);
        Assert.Null(controles[2]);
        Assert.All(new[] { controles[1], controles[3], controles[4] }, c => Assert.True(c!.Identique));
        Assert.Equal(2, lecture.Lectures);
    }

    [Fact]
    public async Task Une_page_sans_rien_a_controler_ne_lit_pas_la_base()
    {
        var lecture = new LectureCalculsFixe("3");
        var controles = await ControleCouponCouru.ControlerAsync(lecture, [null, null], CancellationToken.None);

        Assert.All(controles, Assert.Null);
        Assert.Equal(0, lecture.Lectures);
    }
}
