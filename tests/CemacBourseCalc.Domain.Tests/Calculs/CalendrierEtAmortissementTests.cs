using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using static CemacBourseCalc.Domain.Tests.Calculs.Exemples;

namespace CemacBourseCalc.Domain.Tests.Calculs;

public class CalendrierEtAmortissementTests
{
    [Fact]
    public void Les_coupons_annuels_tombent_chaque_30_juin_jusqu_a_l_echeance()
    {
        Assert.Equal(
            [D(2026, 6, 30), D(2027, 6, 30), D(2028, 6, 30), D(2029, 6, 30), D(2030, 6, 30)],
            CalendrierCoupons.Dates(InFineExact365()));
    }

    [Fact]
    public void Un_premier_coupon_irregulier_sert_d_ancrage_aux_suivants()
    {
        Assert.Equal(
            [D(2026, 9, 30), D(2027, 3, 30), D(2027, 9, 30), D(2028, 3, 30), D(2028, 9, 30)],
            CalendrierCoupons.Dates(PremierCouponIrregulier30E360()));
    }

    [Fact]
    public void Un_ancrage_au_31_ne_glisse_pas_apres_fevrier()
    {
        var o = InFineExact365() with
        {
            Periodicite = Periodicite.Mensuelle, DateJouissance = D(2026, 1, 31), DateEcheance = D(2026, 5, 31)
        };
        Assert.Equal([D(2026, 2, 28), D(2026, 3, 31), D(2026, 4, 30), D(2026, 5, 31)], CalendrierCoupons.Dates(o));
    }

    [Fact]
    public void Une_obligation_in_fine_rembourse_tout_a_l_echeance()
    {
        var r = Assert.Single(Amortissement.Remboursements(InFineExact365()));
        Assert.Equal(new RemboursementCapital(D(2030, 6, 30), 10_000m, 0m), r);
    }

    [Fact]
    public void L_amortissement_constant_commence_apres_le_differe()
    {
        Assert.Equal(
            [
                new RemboursementCapital(D(2027, 6, 30), 2_500m, 7_500m),
                new RemboursementCapital(D(2028, 6, 30), 2_500m, 5_000m),
                new RemboursementCapital(D(2029, 6, 30), 2_500m, 2_500m),
                new RemboursementCapital(D(2030, 6, 30), 2_500m, 0m)
            ],
            Amortissement.Remboursements(AmortissableExact360()));
    }

    [Fact]
    public void La_derniere_echeance_prend_le_reste_de_l_arrondi()
    {
        // 10 000 / 3 = 3 333,333… : deux parts de 3 333,33, la dernière de 3 333,34.
        var o = AmortissableExact360() with { DateEcheance = D(2028, 6, 30), DiffereEcheances = 0 };
        Assert.Equal([3_333.33m, 3_333.33m, 3_333.34m], Amortissement.Remboursements(o).Select(r => r.Capital));
    }

    [Fact]
    public void Un_differe_qui_couvre_toutes_les_echeances_est_refuse()
    {
        var o = AmortissableExact360() with { DiffereEcheances = 5 };
        Assert.Throws<DomainException>(() => Amortissement.Remboursements(o));
    }
}
