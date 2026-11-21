using CemacBourseCalc.Domain.Calculs;
using static CemacBourseCalc.Domain.Tests.Bvmac.DonneesBvmac;

namespace CemacBourseCalc.Domain.Tests.Bvmac;

/// <summary>
/// Nos échéanciers comparés à ceux que les émetteurs publient dans leurs fiches du bulletin.
///
/// La fiche donne, pour toute l'émission, le capital et les intérêts de chaque échéance future.
/// Nous calculons la même échéance pour le nombre de titres cotés. Les fiches étant arrondies
/// au franc, l'écart admis est strictement inférieur à 1 FCFA.
///
/// Les écarts déjà expliqués sont listés dans <see cref="EcartsConnus"/> : ils sont exclus de la
/// comparaison stricte, et un test vérifie qu'ils existent toujours (s'ils disparaissent, on les retire).
/// </summary>
public class EcheancierBvmacTests
{
    /// <summary>ISIN → raison de l'écart avec la fiche de l'émetteur.</summary>
    public static readonly IReadOnlyDictionary<string, string> EcartsConnus = new Dictionary<string, string>
    {
        ["CM0000020370"] = "Part d'amortissement 10 000 / 3 : la BVMAC ne l'arrondit pas, nous l'arrondissons au centime (3 333,33).",
        ["GA0000020495"] = "Part d'amortissement 10 000 / 3 : la BVMAC ne l'arrondit pas, nous l'arrondissons au centime (3 333,33).",
        ["CM0000020628"] = "Part d'amortissement 10 000 / 18 non arrondie par la BVMAC (nominal publié 8 333,333) ; intérêts de la fiche en Exact/360.",
        ["CM0000020610"] = "Intérêts de la fiche ALIOS en Exact/360 (jours réels / 360), alors que le coupon couru du bulletin est en Exact/365.",
    };

    public static TheoryData<string, string> EcheancesComparables()
    {
        var donnees = new TheoryData<string, string>();
        foreach (var f in Lire(Bulletin2643, "6_echeanciers_fiches.csv").Where(f => !EcartsConnus.ContainsKey(f["ISIN"])))
            donnees.Add(f["ISIN"], f["Date"]);
        return donnees;
    }

    [Theory]
    [MemberData(nameof(EcheancesComparables))]
    public void Bulletin_2643_echeance_identique_a_la_fiche_de_l_emetteur(string isin, string date)
    {
        var (nom, fiche, nous) = Comparer(isin, date);

        Assert.True(Math.Abs(nous.Capital - fiche.Capital) < 1m,
            $"{nom} ({isin}) au {date} : capital fiche {fiche.Capital:N0}, nous {nous.Capital:N2}.");
        Assert.True(Math.Abs(nous.Coupon - fiche.Interets) < 1m,
            $"{nom} ({isin}) au {date} : intérêts fiche {fiche.Interets:N0}, nous {nous.Coupon:N2}.");
    }

    [Fact]
    public void Bulletin_2643_les_ecarts_connus_existent_toujours()
    {
        foreach (var isin in EcartsConnus.Keys)
        {
            var ecart = Lire(Bulletin2643, "6_echeanciers_fiches.csv")
                .Where(f => f["ISIN"] == isin)
                .Select(f => Comparer(isin, f["Date"]))
                .Any(c => Math.Abs(c.Nous.Capital - c.Fiche.Capital) >= 1m || Math.Abs(c.Nous.Coupon - c.Fiche.Interets) >= 1m);

            Assert.True(ecart, $"{isin} ne s'écarte plus de la fiche : retirez-le des écarts connus.");
        }
    }

    private static (string Nom, (decimal Capital, decimal Interets) Fiche, LigneEcheancier Nous) Comparer(string isin, string date)
    {
        var obligation = Obligation(Lire(Bulletin2643, "2_obligations.csv").Single(l => l["ISIN"] == isin));
        var fiche = Lire(Bulletin2643, "6_echeanciers_fiches.csv").Single(f => f["ISIN"] == isin && f["Date"] == date);
        var quantite = (int)Nombre(fiche["Nombre de titres"]);

        var ligne = Echeancier.Calculer(obligation.Caracteristiques(), quantite, new HashSet<DateOnly>())
            .Lignes.SingleOrDefault(l => l.DateTheorique == Date(date));
        Assert.True(ligne is not null, $"{obligation.Nom} ({isin}) : pas d'échéance le {date} dans notre calendrier.");

        return (obligation.Nom, (Nombre(fiche["Capital"]), Nombre(fiche["Intérêts"])), ligne!);
    }
}
