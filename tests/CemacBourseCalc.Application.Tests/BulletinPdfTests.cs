using System.Globalization;
using CemacBourseCalc.Application.Bulletins.Common;
using CemacBourseCalc.Application.Bulletins.Queries.AnalyserBulletin;
using CemacBourseCalc.Infrastructure.Pdf;
using Microsoft.Extensions.Logging.Abstractions;

namespace CemacBourseCalc.Application.Tests;

/// <summary>
/// La lecture du vrai bulletin n° 2643 (PDF publié par la BVMAC) doit redonner, au centime, les cours
/// relevés et vérifiés à la main dans donnees/bvmac/2026-11-20_BOC-2643/4_cours.csv.
/// </summary>
public class BulletinPdfTests
{
    private const string Dossier = "donnees/bvmac/2026-11-20_BOC-2643";

    private static string Racine()
    {
        var dossier = new DirectoryInfo(AppContext.BaseDirectory);
        while (dossier is not null && !File.Exists(Path.Combine(dossier.FullName, "CemacBourseCalc.slnx"))) dossier = dossier.Parent;
        return dossier?.FullName ?? throw new InvalidOperationException("Racine du dépôt introuvable.");
    }

    private static byte[] Pdf() => File.ReadAllBytes(Path.Combine(Racine(), Dossier, "BOC-20261120.pdf"));

    private static List<Dictionary<string, string>> CoursVerifies()
    {
        var lignes = File.ReadAllLines(Path.Combine(Racine(), Dossier, "4_cours.csv"));
        var entetes = lignes[0].Split(';');
        return lignes.Skip(1).Where(l => l.Length > 0)
            .Select(l => entetes.Zip(l.Split(';')).ToDictionary(p => p.First, p => p.Second))
            .ToList();
    }

    private static decimal? Nombre(string texte) =>
        string.IsNullOrWhiteSpace(texte) ? null : decimal.Parse(texte.Replace(',', '.'), CultureInfo.InvariantCulture);

    [Fact]
    public void Le_bulletin_2643_redonne_les_40_cours_verifies()
    {
        var pages = new LecteurPdfPig(NullLogger<LecteurPdfPig>.Instance).Lire(Pdf(), AnalyserBulletinQuery.PagesMaximum, AnalyseurBulletin.SeanceLue);
        Assert.True(pages.Reussi, pages.Message);
        Assert.Equal(5, pages.Valeur!.Count); // pages 2 à 4 : la séance ; la page 5 arrête la lecture

        var lu = AnalyseurBulletin.Analyser(pages.Valeur!);
        Assert.True(lu.Reussi, lu.Message);
        var bulletin = lu.Valeur!;

        Assert.Equal(2643, bulletin.Numero);
        Assert.Equal(new DateOnly(2026, 11, 20), bulletin.DateSeance);
        Assert.Empty(bulletin.Anomalies);
        Assert.Equal(33, bulletin.Cours.Count(c => c.Section == AnalyseurBulletin.SectionObligation));
        Assert.Equal(7, bulletin.Cours.Count(c => c.Section == AnalyseurBulletin.SectionAction));

        var verifies = CoursVerifies();
        Assert.Equal(verifies.Count, bulletin.Cours.Count);
        foreach (var attendu in verifies)
        {
            var c = Assert.Single(bulletin.Cours, c => c.Isin == attendu["ISIN"]);
            Assert.True(Nombre(attendu["Cours de clôture"]) == c.CoursCloture, $"{c.Isin} clôture : attendu {attendu["Cours de clôture"]}, lu {c.CoursCloture}");
            Assert.True(Nombre(attendu["Volume"]) == c.Volume, $"{c.Isin} volume : attendu {attendu["Volume"]}, lu {c.Volume}");
            Assert.True(Nombre(attendu["Coupon couru publié"]) == c.CouponCouruPublie,
                $"{c.Isin} coupon couru : attendu {attendu["Coupon couru publié"]}, lu {c.CouponCouruPublie}");
        }
    }

    [Fact]
    public void Un_fichier_qui_n_est_pas_un_PDF_est_refuse_avant_lecture()
    {
        var validateur = new AnalyserBulletinQueryValidator();
        Assert.False(validateur.Validate(new AnalyserBulletinQuery([])).IsValid);
        Assert.False(validateur.Validate(new AnalyserBulletinQuery("ISIN;Nom\nCM0000020305;x"u8.ToArray())).IsValid);
        Assert.False(validateur.Validate(new AnalyserBulletinQuery(new byte[AnalyserBulletinQuery.TailleMaximum + 1])).IsValid);
        Assert.True(validateur.Validate(new AnalyserBulletinQuery(Pdf())).IsValid);
    }

    [Fact]
    public void Un_PDF_abime_donne_un_message_clair()
    {
        var abime = "%PDF-1.7\n1 0 obj << /Type /Catalog >> endobj\n%%EOF"u8.ToArray();
        var pages = new LecteurPdfPig(NullLogger<LecteurPdfPig>.Instance).Lire(abime, AnalyserBulletinQuery.PagesMaximum);
        Assert.False(pages.Reussi);
        Assert.Contains("illisible", pages.Message);
    }

    [Fact]
    public void Un_PDF_sans_en_tete_de_bulletin_est_refuse()
    {
        var resultat = AnalyseurBulletin.Analyser([new PageBulletin(1, [new MotPdf(10, 20, 30, 8, "Facture")])]);
        Assert.False(resultat.Reussi);
        Assert.Contains("bulletin officiel", resultat.Message);
    }
}
