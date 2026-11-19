using CemacBourseCalc.Domain.Conventions;

namespace CemacBourseCalc.Domain.Tests.Conventions;

/// <summary>
/// Cas comptés à la main, jour par jour, sur un calendrier.
/// Du 15/01/2026 au 15/07/2026 : 31 + 28 + 31 + 30 + 31 + 30 = 181 jours réels.
/// </summary>
public class ConventionsDecompteTests
{
    private static DateOnly D(int a, int m, int j) => new(a, m, j);

    [Fact]
    public void Exact365_compte_les_jours_reels_et_divise_par_365()
    {
        var c = ConventionsDecompte.Exact365;
        Assert.Equal(181, c.Jours(D(2026, 1, 15), D(2026, 7, 15)));
        Assert.Equal(181m / 365m, c.FractionAnnee(D(2026, 1, 15), D(2026, 7, 15)));
    }

    [Fact]
    public void Exact365_divise_par_365_meme_en_annee_bissextile()
    {
        var c = ConventionsDecompte.Exact365;
        Assert.Equal(366, c.Jours(D(2028, 1, 1), D(2029, 1, 1)));
        Assert.Equal(366m / 365m, c.FractionAnnee(D(2028, 1, 1), D(2029, 1, 1)));
    }

    [Fact]
    public void Exact360_compte_les_jours_reels_et_divise_par_360()
    {
        var c = ConventionsDecompte.Exact360;
        Assert.Equal(181, c.Jours(D(2026, 1, 15), D(2026, 7, 15)));
        Assert.Equal(181m / 360m, c.FractionAnnee(D(2026, 1, 15), D(2026, 7, 15)));
    }

    [Theory]
    // Du 31/01 au 31/03 : les deux 31 deviennent des 30, soit 2 mois de 30 jours (59 jours réels).
    [InlineData(2026, 1, 31, 2026, 3, 31, 60)]
    // Du 28/02 au 31/03 : 1 mois + (30 - 28) = 32 (31 jours réels).
    [InlineData(2026, 2, 28, 2026, 3, 31, 32)]
    // Changement d'année : 360 - 6 x 30 = 180.
    [InlineData(2025, 12, 15, 2026, 6, 15, 180)]
    // Du 15/01 au 15/07 : 6 mois de 30 jours (181 jours réels).
    [InlineData(2026, 1, 15, 2026, 7, 15, 180)]
    public void Trente360Europeen_compte_des_mois_de_30_jours(int a1, int m1, int j1, int a2, int m2, int j2, int attendu)
    {
        Assert.Equal(attendu, ConventionsDecompte.Trente360Europeen.Jours(D(a1, m1, j1), D(a2, m2, j2)));
    }

    [Theory]
    [InlineData("EXACT_365", "Exact/365")]
    [InlineData("exact_360", "Exact/360")]
    [InlineData(" 30E_360 ", "30E/360")]
    public void Une_convention_se_retrouve_par_son_code(string code, string libelle)
    {
        Assert.Equal(libelle, ConventionsDecompte.Trouver(code)!.Libelle);
    }

    [Fact]
    public void Un_code_inconnu_ne_donne_aucune_convention()
    {
        Assert.Null(ConventionsDecompte.Trouver("ACT_ACT"));
        Assert.Null(ConventionsDecompte.Trouver(null));
    }
}
