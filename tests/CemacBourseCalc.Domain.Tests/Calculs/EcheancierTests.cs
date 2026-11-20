using CemacBourseCalc.Domain.Calculs;
using CemacBourseCalc.Domain.Communs;
using static CemacBourseCalc.Domain.Tests.Calculs.Exemples;

namespace CemacBourseCalc.Domain.Tests.Calculs;

/// <summary>Chaque montant attendu est recompté à la main dans le commentaire.</summary>
public class EcheancierTests
{
    private static readonly IReadOnlySet<DateOnly> Aucun = new HashSet<DateOnly>();

    [Fact]
    public void In_fine_cinq_coupons_de_650_et_le_capital_a_l_echeance()
    {
        // Coupon régulier annuel : 10 000 × 6,5 % / 1 = 650 FCFA. Capital de 10 000 FCFA en 2030.
        var r = Echeancier.Calculer(InFineExact365(), 1, Aucun);

        Assert.Equal(5, r.Lignes.Count);
        Assert.All(r.Lignes, l => Assert.Equal(650m, l.Coupon));
        Assert.All(r.Lignes, l => Assert.True(l.PeriodeReguliere));
        Assert.Equal([0m, 0m, 0m, 0m, 10_000m], r.Lignes.Select(l => l.Capital));
        Assert.Equal(3_250m, r.TotalCoupons);
        Assert.Equal(13_250m, r.TotalFlux);
    }

    [Fact]
    public void Un_paiement_un_week_end_est_reporte_au_lundi()
    {
        // 30/06/2029 est un samedi → lundi 02/07/2029. 30/06/2030 est un dimanche → lundi 01/07/2030.
        // 30/06/2028 est un vendredi : pas de report.
        var r = Echeancier.Calculer(InFineExact365(), 1, Aucun);

        Assert.Equal(D(2028, 6, 30), r.Lignes[2].DatePaiement);
        Assert.Equal(D(2029, 7, 2), r.Lignes[3].DatePaiement);
        Assert.Equal(D(2030, 7, 1), r.Lignes[4].DatePaiement);
        Assert.Equal(D(2030, 6, 30), r.Lignes[4].DateTheorique);
    }

    [Fact]
    public void Un_paiement_un_jour_non_ouvre_est_reporte_sans_changer_le_montant()
    {
        // Mercredi 30/06/2027 non ouvré → jeudi 01/07/2027, toujours 650 FCFA.
        var r = Echeancier.Calculer(InFineExact365(), 1, new HashSet<DateOnly> { D(2027, 6, 30) });

        Assert.Equal(D(2027, 7, 1), r.Lignes[1].DatePaiement);
        Assert.Equal(650m, r.Lignes[1].Coupon);
    }

    [Fact]
    public void Amortissable_le_coupon_suit_le_nominal_restant()
    {
        // 2026 : 10 000 × 6,5 % = 650 (différé) ; 2027 : 650 et 2 500 de capital ;
        // 2028 : 7 500 × 6,5 % = 487,50 ; 2029 : 5 000 → 325 ; 2030 : 2 500 → 162,50.
        // Total des coupons : 650 + 650 + 487,50 + 325 + 162,50 = 2 275 FCFA.
        var r = Echeancier.Calculer(AmortissableExact360(), 1, Aucun);

        Assert.Equal([650m, 650m, 487.50m, 325m, 162.50m], r.Lignes.Select(l => l.Coupon));
        Assert.Equal([0m, 2_500m, 2_500m, 2_500m, 2_500m], r.Lignes.Select(l => l.Capital));
        Assert.Equal([10_000m, 7_500m, 5_000m, 2_500m, 0m], r.Lignes.Select(l => l.NominalRestantApresParTitre));
        Assert.Equal(2_275m, r.TotalCoupons);
        Assert.Equal(10_000m, r.TotalCapital);
    }

    [Fact]
    public void Premier_coupon_long_au_prorata_puis_coupons_reguliers()
    {
        // Du 15/01/2026 au 30/09/2026 en 30E/360 : 8 mois × 30 + (30 − 15) = 255 jours.
        // 10 000 × 5 % × 255 / 360 = 354,166… → 354,17 FCFA. Ensuite : 10 000 × 5 % / 2 = 250 FCFA.
        var r = Echeancier.Calculer(PremierCouponIrregulier30E360(), 1, Aucun);

        Assert.False(r.Lignes[0].PeriodeReguliere);
        Assert.Equal(354.17m, r.Lignes[0].Coupon);
        Assert.All(r.Lignes.Skip(1), l => Assert.True(l.PeriodeReguliere));
        Assert.Equal([250m, 250m, 250m, 250m], r.Lignes.Skip(1).Select(l => l.Coupon));
    }

    [Fact]
    public void Le_coupon_irregulier_est_arrondi_sur_la_quantite()
    {
        // 3 titres : 354,166… × 3 = 1 062,50 FCFA (et non 3 × 354,17 = 1 062,51).
        var r = Echeancier.Calculer(PremierCouponIrregulier30E360(), 3, Aucun);

        Assert.Equal(1_062.50m, r.Lignes[0].Coupon);
        Assert.Equal(354.17m, r.Lignes[0].CouponParTitre);
    }

    [Fact]
    public void Une_derniere_periode_courte_est_calculee_au_prorata()
    {
        // Échéance avancée au 31/03/2030. Du 30/06/2029 au 31/03/2030 :
        // 31+31+30+31+30+31+31+28+31 = 274 jours. 650 × 274 / 365 = 487,945… → 487,95 FCFA.
        var o = InFineExact365() with { DateEcheance = D(2030, 3, 31) };
        var derniere = Echeancier.Calculer(o, 1, Aucun).Lignes[^1];

        Assert.False(derniere.PeriodeReguliere);
        Assert.Equal(D(2030, 3, 31), derniere.DateTheorique);
        Assert.Equal(487.95m, derniere.Coupon);
    }

    [Fact]
    public void Apres_un_achat_seules_les_echeances_suivantes_sont_rendues()
    {
        // Règlement le jour du coupon 2027 : ce coupon revient au vendeur, l'acheteur touche 2028, 2029, 2030.
        var r = Echeancier.Calculer(InFineExact365(), 1, Aucun, fluxApresLe: D(2027, 6, 30));

        Assert.Equal([D(2028, 6, 30), D(2029, 6, 30), D(2030, 6, 30)], r.Lignes.Select(l => l.DateTheorique));
        Assert.Equal(1_950m, r.TotalCoupons);
    }

    [Fact]
    public void Un_coupon_unique_in_fine_court_sur_toute_la_vie_du_titre()
    {
        // Du 30/06/2025 au 30/06/2027 en Exact/365 : 365 + 365 = 730 jours.
        // 10 000 × 6,5 % × 730 / 365 = 1 300 FCFA, payé avec le capital.
        var o = InFineExact365() with { Periodicite = Periodicite.InFine, DateEcheance = D(2027, 6, 30) };
        var ligne = Assert.Single(Echeancier.Calculer(o, 1, Aucun).Lignes);

        Assert.Equal(1_300m, ligne.Coupon);
        Assert.Equal(10_000m, ligne.Capital);
        Assert.Equal(11_300m, ligne.Total);
    }

    [Fact]
    public void Apres_l_echeance_il_ne_reste_rien_a_percevoir()
    {
        Assert.Throws<DomainException>(() => Echeancier.Calculer(InFineExact365(), 1, Aucun, fluxApresLe: D(2030, 6, 30)));
    }
}
