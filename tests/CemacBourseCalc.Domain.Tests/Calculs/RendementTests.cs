using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using static CemacBourseCalc.Domain.Tests.Calculs.Exemples;

namespace CemacBourseCalc.Domain.Tests.Calculs;

public class RendementObligationTests
{
    [Fact]
    public void Au_pair_un_an_avant_l_echeance_le_rendement_actuariel_vaut_le_taux_facial()
    {
        // Règlement le 30/06/2029, jour de coupon : coupon couru nul. Il reste un seul flux,
        // 10 000 + 650 = 10 650 FCFA le 30/06/2030, dans 365 jours (t = 1 an).
        // 10 000 = 10 650 / (1 + r)  →  r = 6,50 %.
        var r = RendementObligation.Calculer(InFineExact365(), D(2029, 6, 30), 100m);

        Assert.Equal(0m, r.CouponCouruParTitre);
        Assert.Equal(10_000m, r.PrixPleinCouponParTitre);
        var flux = Assert.Single(r.Flux);
        Assert.Equal(365, flux.Jours);
        Assert.Equal(10_650m, flux.Flux);
        Assert.Equal(6.5m, r.RendementActuariel);
    }

    [Fact]
    public void Sous_le_pair_le_rendement_actuariel_depasse_le_taux_facial()
    {
        // 9 800 = 10 650 / (1 + r)  →  r = 10 650 / 9 800 − 1 = 8,67347 %.
        var r = RendementObligation.Calculer(InFineExact365(), D(2029, 6, 30), 98m);
        Assert.Equal(8.6735m, r.RendementActuariel);
    }

    [Fact]
    public void Rendement_courant_taux_facial_sur_cours()
    {
        // 6,50 / 98,50 × 100 = 6,59898 %.
        var r = RendementObligation.Calculer(InFineExact365(), D(2026, 1, 15), 98.5m);
        Assert.Equal(6.5990m, r.RendementCourant);
        Assert.Equal(650m, r.CouponAnnuelParTitre);
    }

    [Fact]
    public void Les_flux_actualises_redonnent_le_prix_plein_coupon()
    {
        // Prix plein d'un titre au 15/01/2026 à 98,50 : 9 850 + 354,38 = 10 204,38 FCFA.
        var r = RendementObligation.Calculer(InFineExact365(), D(2026, 1, 15), 98.5m);

        Assert.Equal(10_204.38m, r.PrixPleinCouponParTitre);
        Assert.Equal(5, r.Flux.Count); // coupons 2026 à 2030, le dernier avec le capital
        Assert.InRange(r.Flux.Sum(f => f.ValeurActuelle), 10_204.38m - 0.05m, 10_204.38m + 0.05m);
        Assert.True(r.RendementActuariel > 6.5m);
    }

    [Fact]
    public void Au_dessus_du_pair_le_rendement_actuariel_est_inferieur_au_taux_facial()
    {
        var r = RendementObligation.Calculer(InFineExact365(), D(2026, 1, 15), 103m);
        Assert.True(r.RendementActuariel < 6.5m);
        Assert.True(r.RendementCourant < 6.5m);
    }

    [Fact]
    public void Amortissable_les_flux_comprennent_le_capital_restant()
    {
        // Au 30/12/2027 : nominal restant 7 500, puis 3 remboursements de 2 500 (2028 à 2030).
        var r = RendementObligation.Calculer(AmortissableExact360(), D(2027, 12, 30), 100m);

        Assert.Equal(7_500m, r.NominalRestant);
        Assert.Equal(3, r.Flux.Count);
        Assert.Equal(7_500m, r.Flux.Sum(f => f.Capital));
        Assert.All(r.Flux, f => Assert.True(f.Date > D(2027, 12, 30)));
    }

    [Fact]
    public void Un_cours_trop_bas_est_refuse()
    {
        // 100 = 10 650 / (1 + r)  →  r = 10 550 % : au-delà de la borne de 1 000 %.
        Assert.Throws<DomainException>(() => RendementObligation.Calculer(InFineExact365(), D(2029, 6, 30), 1m));
    }

    [Fact]
    public void Apres_l_echeance_il_n_y_a_plus_de_rendement()
    {
        Assert.Throws<DomainException>(() => RendementObligation.Calculer(InFineExact365(), D(2030, 6, 30), 100m));
    }
}

public class RendementDividendeTests
{
    [Fact]
    public void Dividende_sur_cours()
    {
        // 250 / 5 000 × 100 = 5 % brut ; 210 / 5 000 × 100 = 4,20 % net.
        var r = RendementDividende.Calculer(5_000m, 2025, 250m, 210m);
        Assert.Equal(5m, r.RendementBrut);
        Assert.Equal(4.2m, r.RendementNet);
    }

    [Fact]
    public void Sans_dividende_net_seul_le_brut_est_calcule()
    {
        var r = RendementDividende.Calculer(32_000m, 2025, 1_500m, null);
        Assert.Equal(4.6875m, r.RendementBrut);
        Assert.Null(r.RendementNet);
    }

    [Theory]
    [InlineData(0, 250, null)]
    [InlineData(-5, 250, null)]
    [InlineData(5000.123, 250, null)]
    [InlineData(5000, 0, null)]
    [InlineData(5000, 250, 300.0)]
    public void Saisies_invalides_refusees(decimal cours, decimal brut, double? net)
    {
        Assert.Throws<DomainException>(() => RendementDividende.Calculer(cours, 2025, brut, (decimal?)net));
    }
}
