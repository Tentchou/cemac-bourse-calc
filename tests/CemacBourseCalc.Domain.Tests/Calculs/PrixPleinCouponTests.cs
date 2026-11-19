using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using static CemacBourseCalc.Domain.Tests.Calculs.Exemples;

namespace CemacBourseCalc.Domain.Tests.Calculs;

public class PrixPleinCouponTests
{
    [Fact]
    public void Prix_plein_coupon_de_10_titres_a_98_50()
    {
        // Pied de coupon : 10 000 × 98,50 / 100 = 9 850 FCFA par titre, 98 500 FCFA pour 10.
        // Coupon couru au 15/01/2026 : 3 543,84 FCFA pour 10 (voir CouponCouruTests).
        // Plein coupon : 98 500 + 3 543,84 = 102 043,84 FCFA.
        var r = PrixPleinCoupon.Calculer(InFineExact365(), D(2026, 1, 15), 10, 98.50m);

        Assert.Equal(9_850m, r.PrixPiedCouponParTitre);
        Assert.Equal(98_500m, r.PrixPiedCouponTotal);
        Assert.Equal(3_543.84m, r.CouponCouru.Total);
        Assert.Equal(102_043.84m, r.PrixPleinCouponTotal);
    }

    [Fact]
    public void Le_cours_s_applique_au_nominal_restant()
    {
        // Amortissable au 30/12/2027 : nominal restant 7 500 FCFA, cours 101 % → 7 575 FCFA par titre.
        var r = PrixPleinCoupon.Calculer(AmortissableExact360(), D(2027, 12, 30), 1, 101m);
        Assert.Equal(7_575m, r.PrixPiedCouponParTitre);
        Assert.Equal(7_575m + 247.81m, r.PrixPleinCouponTotal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1000)]
    [InlineData(98.12345)]
    public void Un_cours_hors_limites_est_refuse(decimal cours)
    {
        Assert.Throws<DomainException>(() => PrixPleinCoupon.Calculer(InFineExact365(), D(2026, 1, 15), 1, cours));
    }
}
