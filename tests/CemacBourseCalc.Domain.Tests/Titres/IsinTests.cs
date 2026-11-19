using CemacBourseCalc.Domain.Communs;
using CemacBourseCalc.Domain.Titres;

namespace CemacBourseCalc.Domain.Tests.Titres;

public class IsinTests
{
    [Theory]
    // Deux ISIN publics hors CEMAC, dont le chiffre de contrôle est connu : ils vérifient l'algorithme seul.
    [InlineData("US037833100", 5)]
    [InlineData("FR000012027", 1)]
    public void Le_chiffre_de_controle_suit_l_algorithme_de_Luhn(string onze, int attendu)
    {
        Assert.Equal(attendu, Isin.ChiffreControle(onze));
    }

    [Theory]
    [InlineData("CM", PaysCemac.CM)]
    [InlineData("GA", PaysCemac.GA)]
    [InlineData("TD", PaysCemac.TD)]
    public void Un_isin_de_la_cemac_est_accepte_et_donne_son_pays(string prefixe, PaysCemac pays)
    {
        var onze = prefixe + "000000001";
        var isin = Isin.Creer($" {onze.ToLowerInvariant()}{Isin.ChiffreControle(onze)} ");

        Assert.Equal(pays, isin.Pays);
        Assert.Equal(onze + Isin.ChiffreControle(onze), isin.Valeur);
    }

    [Fact]
    public void Un_isin_hors_cemac_est_refuse()
    {
        var e = Assert.Throws<DomainException>(() => Isin.Creer("US0378331005"));
        Assert.Contains("CEMAC", e.Message);
    }

    [Fact]
    public void Un_chiffre_de_controle_faux_est_refuse()
    {
        var onze = "CM000000001";
        var faux = (Isin.ChiffreControle(onze) + 1) % 10;
        var e = Assert.Throws<DomainException>(() => Isin.Creer(onze + faux));
        Assert.Contains("contrôle", e.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("CM12345")]
    [InlineData("1M0000000011")]
    public void Une_saisie_qui_n_a_pas_la_forme_d_un_isin_est_refusee(string? saisie)
    {
        Assert.Throws<DomainException>(() => Isin.Creer(saisie));
    }
}
