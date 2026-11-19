using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using static CemacBourseCalc.Domain.Tests.Calculs.Exemples;

namespace CemacBourseCalc.Domain.Tests.Calculs;

/// <summary>Chaque résultat attendu est recompté à la main dans le commentaire du test.</summary>
public class CouponCouruTests
{
    [Fact]
    public void Exact365_in_fine()
    {
        // Du 30/06/2025 au 15/01/2026 : 31+31+30+31+30+31 + 15 = 199 jours.
        // 10 000 × 6,5 % × 199 / 365 = 129 350 / 365 = 354,383 561… → 354,38 FCFA.
        var r = CouponCouru.Calculer(InFineExact365(), D(2026, 1, 15), 1);

        Assert.Equal(D(2025, 6, 30), r.DebutPeriode);
        Assert.Equal(D(2026, 6, 30), r.FinPeriode);
        Assert.Equal(199, r.Jours);
        Assert.Equal(365, r.JoursAnnee);
        Assert.Equal(10_000m, r.NominalRestant);
        Assert.Equal(354.38m, r.ParTitre);
    }

    [Fact]
    public void Le_total_est_arrondi_une_seule_fois_a_la_fin()
    {
        // 10 titres : 3 543,835 616… → 3 543,84 FCFA (et non 10 × 354,38 = 3 543,80).
        var r = CouponCouru.Calculer(InFineExact365(), D(2026, 1, 15), 10);
        Assert.Equal(3_543.84m, r.Total);
    }

    [Fact]
    public void Le_jour_du_coupon_le_coupon_couru_repart_de_zero()
    {
        var r = CouponCouru.Calculer(InFineExact365(), D(2026, 6, 30), 1);

        Assert.Equal(D(2026, 6, 30), r.DebutPeriode);
        Assert.Equal(0, r.Jours);
        Assert.Equal(0m, r.Total);
    }

    [Fact]
    public void Amortissable_le_coupon_court_sur_le_nominal_restant()
    {
        // Après le remboursement du 30/06/2027, il reste 7 500 FCFA.
        // Du 30/06/2027 au 30/12/2027 : 31+31+30+31+30 + 30 = 183 jours.
        // 7 500 × 6,5 % × 183 / 360 = 89 212,5 / 360 = 247,8125 → 247,81 FCFA.
        var r = CouponCouru.Calculer(AmortissableExact360(), D(2027, 12, 30), 1);

        Assert.Equal(7_500m, r.NominalRestant);
        Assert.Equal(183, r.Jours);
        Assert.Equal(247.81m, r.ParTitre);
    }

    [Fact]
    public void Amortissable_pendant_le_differe_le_nominal_est_entier()
    {
        // Le coupon du 30/06/2026 est en différé : rien n'est remboursé, il reste 10 000 FCFA.
        var r = CouponCouru.Calculer(AmortissableExact360(), D(2027, 6, 29), 1);
        Assert.Equal(10_000m, r.NominalRestant);
    }

    [Fact]
    public void Premier_coupon_long_en_30E360()
    {
        // Du 15/01/2026 au 31/05/2026 en 30E/360 : 4 mois × 30 + (30 − 15) = 135 jours.
        // 10 000 × 5 % × 135 / 360 = 187,50 FCFA.
        var r = CouponCouru.Calculer(PremierCouponIrregulier30E360(), D(2026, 5, 31), 1);

        Assert.Equal(D(2026, 1, 15), r.DebutPeriode);
        Assert.Equal(D(2026, 9, 30), r.FinPeriode);
        Assert.Equal(135, r.Jours);
        Assert.Equal(187.50m, r.ParTitre);
    }

    [Fact]
    public void Periode_reguliere_apres_le_premier_coupon_en_30E360()
    {
        // Du 30/03/2027 au 31/05/2027 : 2 mois × 30 + (30 − 30) = 60 jours.
        // 10 000 × 5 % × 60 / 360 = 83,333… → 83,33 FCFA.
        var r = CouponCouru.Calculer(PremierCouponIrregulier30E360(), D(2027, 5, 31), 1);

        Assert.Equal(60, r.Jours);
        Assert.Equal(83.33m, r.ParTitre);
    }

    [Fact]
    public void Un_titre_echu_ne_porte_plus_d_interets()
    {
        var e = Assert.Throws<DomainException>(() => CouponCouru.Calculer(InFineExact365(), D(2030, 6, 30), 1));
        Assert.Contains("échéance", e.Message);
    }

    [Fact]
    public void Une_date_avant_la_jouissance_est_refusee()
    {
        Assert.Throws<DomainException>(() => CouponCouru.Calculer(InFineExact365(), D(2025, 6, 29), 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Une_quantite_nulle_ou_negative_est_refusee(int quantite)
    {
        Assert.Throws<DomainException>(() => CouponCouru.Calculer(InFineExact365(), D(2026, 1, 15), quantite));
    }
}
