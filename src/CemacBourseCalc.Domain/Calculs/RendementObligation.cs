using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Calculs;

/// <summary>Un flux futur d'un titre, actualisé au taux actuariel trouvé (pour l'explication).</summary>
public sealed record FluxActualise(
    DateOnly Date,
    int Jours,
    decimal FractionAnnee,
    decimal Coupon,
    decimal Capital,
    decimal Flux,
    decimal FacteurActualisation,
    decimal ValeurActuelle);

public sealed record ResultatRendementObligation(
    DateOnly DateReglement,
    decimal CoursPiedCoupon,
    decimal NominalRestant,
    decimal TauxFacial,
    decimal PrixPiedCouponParTitre,
    decimal CouponCouruParTitre,
    decimal PrixPleinCouponParTitre,
    decimal CouponAnnuelParTitre,
    decimal RendementCourant,
    decimal RendementActuariel,
    IReadOnlyList<FluxActualise> Flux);

/// <summary>
/// Deux rendements d'une obligation achetée à un cours donné, pour un titre :
///
///   rendement courant   = coupon annuel / prix pied de coupon
///                       = taux facial / cours × 100
///   rendement actuariel = le taux r tel que  prix plein coupon = Σ flux / (1 + r)^(jours / 365)
///
/// Les flux sont les coupons et remboursements de l'échéancier postérieurs au règlement, à leur
/// date théorique. Le rendement actuariel suppose le titre gardé jusqu'à l'échéance, les flux
/// payés comme prévu ; il est annuel, avant impôt, et ignore les frais de bourse.
/// </summary>
public static class RendementObligation
{
    public const int JoursAnneeActuarielle = 365;

    private const double TauxMinimum = -0.99;
    private const double TauxMaximum = 10.0; // 1 000 % : au-delà, le cours saisi n'a pas de sens
    private const int IterationsMaximum = 200;
    private const double Precision = 1e-10;

    public static ResultatRendementObligation Calculer(CaracteristiquesObligation o, DateOnly dateReglement, decimal coursPiedCoupon)
    {
        var prix = PrixPleinCoupon.Calculer(o, dateReglement, 1, coursPiedCoupon);
        var couponCouru = prix.CouponCouru;
        var prixPiedExact = couponCouru.NominalRestant * coursPiedCoupon / 100m;
        var prixPleinExact = prixPiedExact + couponCouru.ParTitreExact;

        // Les dates théoriques suffisent : le calendrier des jours non ouvrés n'est pas nécessaire ici.
        var lignes = Echeancier.Calculer(o, 1, new HashSet<DateOnly>(), dateReglement).Lignes;
        var echeances = lignes
            .Select(l => (l.DateTheorique, Jours: l.DateTheorique.DayNumber - dateReglement.DayNumber, l.Coupon, l.Capital, Flux: l.Total))
            .ToList();

        var taux = TauxActuariel(echeances.Select(e => ((double)e.Flux, e.Jours / (double)JoursAnneeActuarielle)).ToList(), (double)prixPleinExact);

        var flux = echeances.Select(e =>
        {
            var fraction = e.Jours / (decimal)JoursAnneeActuarielle;
            var facteur = (decimal)Math.Pow(1 + taux, -e.Jours / (double)JoursAnneeActuarielle);
            return new FluxActualise(e.DateTheorique, e.Jours, decimal.Round(fraction, 6),
                e.Coupon, e.Capital, e.Flux, decimal.Round(facteur, 8), Arrondis.Montant(e.Flux * facteur));
        }).ToList();

        var couponAnnuelExact = couponCouru.NominalRestant * o.TauxFacial / 100m;

        return new ResultatRendementObligation(
            dateReglement, coursPiedCoupon, couponCouru.NominalRestant, o.TauxFacial,
            Arrondis.Montant(prixPiedExact), couponCouru.ParTitre, Arrondis.Montant(prixPleinExact),
            Arrondis.Montant(couponAnnuelExact),
            decimal.Round(o.TauxFacial / coursPiedCoupon * 100m, 4, MidpointRounding.AwayFromZero),
            decimal.Round((decimal)taux * 100m, 4, MidpointRounding.AwayFromZero),
            flux);
    }

    /// <summary>
    /// Résout Σ flux × (1 + r)^-t = prix par dichotomie. La valeur actuelle des flux décroît
    /// quand le taux monte : la racine est unique et la dichotomie converge toujours.
    /// </summary>
    private static double TauxActuariel(IReadOnlyList<(double Flux, double Annees)> flux, double prix)
    {
        double Ecart(double r) => flux.Sum(f => f.Flux * Math.Pow(1 + r, -f.Annees)) - prix;

        if (Ecart(TauxMaximum) > 0)
            throw new DomainException("Le cours saisi est trop bas : le rendement dépasserait 1 000 % par an.");
        if (Ecart(TauxMinimum) < 0)
            throw new DomainException("Le cours saisi est trop élevé par rapport aux flux restants du titre.");

        double bas = TauxMinimum, haut = TauxMaximum;
        for (var i = 0; i < IterationsMaximum && haut - bas > Precision; i++)
        {
            var milieu = (bas + haut) / 2;
            if (Ecart(milieu) > 0) bas = milieu; else haut = milieu;
        }
        return (bas + haut) / 2;
    }
}
