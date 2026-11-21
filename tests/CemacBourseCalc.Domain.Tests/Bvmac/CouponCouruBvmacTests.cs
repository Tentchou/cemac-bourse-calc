using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Place;
using static CemacBourseCalc.Domain.Tests.Bvmac.DonneesBvmac;

namespace CemacBourseCalc.Domain.Tests.Bvmac;

/// <summary>
/// Notre coupon couru comparé à celui publié par la BVMAC, titre par titre.
///
/// Pour chaque obligation du bulletin : le titre est recréé à partir du fichier d'import
/// (jouissance, échéance, périodicité, différé…), le coupon couru est calculé à la date de
/// règlement de la séance (J+3 jours ouvrés, comme au bulletin) et doit tomber au centime
/// sur la valeur publiée. Un écart signale une erreur de saisie ou de calcul.
/// </summary>
public class CouponCouruBvmacTests
{
    private const int DelaiReglement = 3;

    public static TheoryData<string> Obligations2643()
    {
        var donnees = new TheoryData<string>();
        foreach (var l in Lire(Bulletin2643, "2_obligations.csv")) donnees.Add(l["ISIN"]);
        return donnees;
    }

    [Theory]
    [MemberData(nameof(Obligations2643))]
    public void Bulletin_2643_coupon_couru_identique_a_la_BVMAC(string isin)
    {
        var obligation = Obligation(Lire(Bulletin2643, "2_obligations.csv").Single(l => l["ISIN"] == isin));
        var cours = Lire(Bulletin2643, "4_cours.csv").Single(l => l["ISIN"] == isin);
        var publie = Nombre(cours["Coupon couru publié"]);

        var reglement = JoursOuvres.DateReglement(Date(cours["Date de séance"]), DelaiReglement, new HashSet<DateOnly>());
        var calcul = CouponCouru.Calculer(obligation.Caracteristiques(), reglement, 1);

        Assert.True(publie == calcul.ParTitre,
            $"{obligation.Nom} ({isin}) au {reglement:dd/MM/yyyy} : BVMAC {publie} FCFA, nous {calcul.ParTitre} FCFA " +
            $"(période du {calcul.DebutPeriode:dd/MM/yyyy}, {calcul.Jours} j, nominal restant {calcul.NominalRestant}).");
    }

    [Fact]
    public void Bulletin_2643_chaque_obligation_a_son_coupon_couru_publie()
    {
        var obligations = Lire(Bulletin2643, "2_obligations.csv").Select(l => l["ISIN"]).ToHashSet();
        var publies = Lire(Bulletin2643, "4_cours.csv")
            .Where(l => !string.IsNullOrWhiteSpace(l["Coupon couru publié"]))
            .Select(l => l["ISIN"])
            .ToHashSet();

        Assert.Equal(33, obligations.Count); // « 33 lignes obligataires », p. 6
        Assert.Equal(obligations, publies);
    }
}
