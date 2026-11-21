using CemacBourseCalc.Application.Bulletins.Common;
using CemacBourseCalc.Application.Bulletins.Interfaces;
using CemacBourseCalc.Application.Bulletins.Queries.AnalyserBulletin;
using CemacBourseCalc.Application.Calculs.Dtos;
using CemacBourseCalc.Application.Calculs.Interfaces;
using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Place;
using CemacBourseCalc.Infrastructure.Pdf;
using Microsoft.Extensions.Logging.Abstractions;

namespace CemacBourseCalc.Application.Tests;

/// <summary>
/// Les règles de l'aperçu, sur le vrai bulletin n° 2643 : un titre inconnu ou de type différent bloque tout ;
/// une séance déjà saisie n'est pas réimportée ; un écart de coupon couru est signalé sans bloquer.
/// </summary>
public class AnalyseBulletinTests
{
    private const string Ecmr6 = "CM0000020305";

    private static readonly CaracteristiquesObligation CaracteristiquesEcmr6 = new(
        "ECMR 6,25% NET 2022-2029", 10_000m, 6.25m, Periodicite.Annuelle,
        new DateOnly(2022, 5, 27), null, new DateOnly(2029, 5, 27), ModeRemboursement.AmortissementConstant, 2, "EXACT_365");

    private static readonly Lazy<IReadOnlyList<PageBulletin>> Pages = new(() =>
    {
        var racine = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(racine.FullName, "CemacBourseCalc.slnx"))) racine = racine.Parent!;
        var pdf = File.ReadAllBytes(Path.Combine(racine.FullName, "donnees/bvmac/2026-11-20_BOC-2643/BOC-20261120.pdf"));
        return new LecteurPdfPig(NullLogger<LecteurPdfPig>.Instance).Lire(pdf, 100, AnalyseurBulletin.SeanceLue).Valeur!;
    });

    private sealed class PdfDejaLu : ILecteurPdf
    {
        public Common.Result<IReadOnlyList<PageBulletin>> Lire(byte[] contenu, int pagesMaximum, Func<IReadOnlyList<PageBulletin>, bool>? suffisant = null) =>
            Common.Result<IReadOnlyList<PageBulletin>>.Succes(Pages.Value);
    }

    private sealed class Base(IReadOnlyList<TitreReference> titres, int coursDejaSaisis) : IBulletinsLecture
    {
        public Task<IReadOnlyList<TitreReference>> TitresAsync(CancellationToken ct) => Task.FromResult(titres);
        public Task<int> CoursDeLaSeanceAsync(DateOnly dateSeance, CancellationToken ct) => Task.FromResult(coursDejaSaisis);
    }

    private sealed class Calculs : ILectureCalculs
    {
        public Task<CaracteristiquesObligation?> ObligationAsync(Guid titreId, CancellationToken ct) => throw new NotSupportedException();
        public Task<DernierDividendeLecture?> DernierDividendeAsync(Guid titreId, CancellationToken ct) => throw new NotSupportedException();
        public Task<string?> ParametreAsync(string cle, CancellationToken ct) => Task.FromResult<string?>(cle == ClesParametres.DelaiReglement ? "3" : null);
        public Task<IReadOnlySet<DateOnly>> JoursNonOuvresAsync(DateOnly du, DateOnly au, CancellationToken ct) => Task.FromResult<IReadOnlySet<DateOnly>>(new HashSet<DateOnly>());
    }

    /// <summary>Les 40 titres du bulletin, tels qu'en base ; ECMR6 avec ses caractéristiques (les autres sans contrôle).</summary>
    private static List<TitreReference> TitresDuBulletin(decimal? tauxEcmr6 = null)
    {
        var bulletin = AnalyseurBulletin.Analyser(Pages.Value).Valeur!;
        return bulletin.Cours.Select(c => new TitreReference(Guid.NewGuid(), c.Isin, c.Libelle, c.Section, "Actif",
                c.Isin == Ecmr6 ? CaracteristiquesEcmr6 with { TauxFacial = tauxEcmr6 ?? 6.25m } : null))
            .ToList();
    }

    private static async Task<Bulletins.Dtos.BulletinAnalyseDto> Analyser(IReadOnlyList<TitreReference> titres, int dejaSaisis = 0)
    {
        var resultat = await new AnalyserBulletinQueryHandler(new PdfDejaLu(), new Base(titres, dejaSaisis), new Calculs())
            .Handle(new AnalyserBulletinQuery("%PDF-"u8.ToArray()), CancellationToken.None);
        Assert.True(resultat.Reussi, resultat.Message);
        return resultat.Valeur!;
    }

    [Fact]
    public async Task Tous_les_titres_connus_le_bulletin_est_importable_et_le_coupon_couru_identique()
    {
        var analyse = await Analyser(TitresDuBulletin());

        Assert.True(analyse.Importable);
        Assert.Equal(2643, analyse.Numero);
        Assert.Equal(40, analyse.Lignes.Count);
        Assert.All(analyse.Lignes, l => Assert.Null(l.Probleme));
        var ecmr6 = Assert.Single(analyse.Lignes, l => l.Isin == Ecmr6);
        Assert.True(ecmr6.Controle!.Identique);
        Assert.Equal(new DateOnly(2026, 11, 25), ecmr6.Controle.DateReglement);
    }

    [Fact]
    public async Task Un_titre_inconnu_bloque_tout_l_import()
    {
        var titres = TitresDuBulletin().Where(t => t.Isin != "GA0000020669").ToList();

        var analyse = await Analyser(titres);

        Assert.False(analyse.Importable);
        var inconnu = Assert.Single(analyse.Lignes, l => l.Probleme is not null);
        Assert.Equal("GA0000020669", inconnu.Isin);
        Assert.Null(inconnu.TitreId);
    }

    [Fact]
    public async Task Un_titre_cote_comme_action_mais_obligation_en_base_bloque()
    {
        var titres = TitresDuBulletin().Select(t => t.Isin == "CM0000010009" ? t with { TypeTitre = "Obligation" } : t).ToList();

        var analyse = await Analyser(titres);

        Assert.False(analyse.Importable);
        Assert.Contains("action", Assert.Single(analyse.Lignes, l => l.Probleme is not null).Probleme);
    }

    [Fact]
    public async Task Une_seance_deja_saisie_n_est_pas_reimportee()
    {
        var analyse = await Analyser(TitresDuBulletin(), dejaSaisis: 40);

        Assert.False(analyse.Importable);
        Assert.Equal(40, analyse.CoursDejaEnregistres);
    }

    [Fact]
    public async Task Un_ecart_de_coupon_couru_est_signale_sans_bloquer()
    {
        var analyse = await Analyser(TitresDuBulletin(tauxEcmr6: 6.30m));

        Assert.True(analyse.Importable);
        var controle = Assert.Single(analyse.Lignes, l => l.Isin == Ecmr6).Controle!;
        Assert.False(controle.Identique);
        Assert.NotEqual(0m, controle.Ecart);
    }

    [Fact]
    public async Task Les_titres_actifs_absents_du_bulletin_sont_listes()
    {
        var titres = TitresDuBulletin();
        titres.Add(new TitreReference(Guid.NewGuid(), "CM0000099999", "Titre non coté ce jour", "Obligation", "Actif", null));
        titres.Add(new TitreReference(Guid.NewGuid(), "CM0000088888", "Titre échu", "Obligation", "Echu", null));

        var analyse = await Analyser(titres);

        Assert.True(analyse.Importable);
        Assert.Equal("CM0000099999", Assert.Single(analyse.Absents).Isin);
    }
}
