using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Calculs;

public sealed record ResultatPrixPleinCoupon(
    decimal CoursPiedCoupon,
    decimal PrixPiedCouponParTitre,
    decimal PrixPiedCouponTotal,
    ResultatCouponCouru CouponCouru,
    decimal PrixPleinCouponTotal);

/// <summary>
/// Ce que l'acheteur paie pour le titre lui-même (hors frais de bourse) :
///
///   prix pied de coupon d'un titre = nominal restant × cours / 100
///   prix plein coupon              = prix pied de coupon + coupon couru
///
/// Le cours d'une obligation cotée s'exprime en pourcentage du nominal restant, pied de coupon.
/// </summary>
public static class PrixPleinCoupon
{
    public const decimal CoursMaximum = 999.9999m;

    public static ResultatPrixPleinCoupon Calculer(CaracteristiquesObligation o, DateOnly dateArrete, int quantite, decimal coursPiedCoupon)
    {
        if (coursPiedCoupon <= 0m || coursPiedCoupon > CoursMaximum)
            throw new DomainException("Le cours s'exprime en pourcentage du nominal (ex. 98,50) et doit être supérieur à 0.");
        if (decimal.Round(coursPiedCoupon, 4) != coursPiedCoupon)
            throw new DomainException("Le cours accepte au plus 4 décimales.");

        var couponCouru = CouponCouru.Calculer(o, dateArrete, quantite);
        var piedExact = couponCouru.NominalRestant * coursPiedCoupon / 100m;
        var piedTotal = Arrondis.Montant(piedExact * quantite);

        return new ResultatPrixPleinCoupon(
            coursPiedCoupon,
            Arrondis.Montant(piedExact),
            piedTotal,
            couponCouru,
            piedTotal + couponCouru.Total);
    }
}
