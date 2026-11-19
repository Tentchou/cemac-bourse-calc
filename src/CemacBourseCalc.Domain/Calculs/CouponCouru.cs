using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Calculs;

/// <summary>
/// Le résultat du calcul avec chaque étape, pour que l'écran l'explique.
/// <para><see cref="ParTitreExact"/> n'est pas arrondi : c'est la valeur que le total reprend.</para>
/// </summary>
public sealed record ResultatCouponCouru(
    DateOnly DateArrete,
    DateOnly DebutPeriode,
    DateOnly FinPeriode,
    string Convention,
    int Jours,
    int JoursAnnee,
    decimal NominalRestant,
    decimal TauxFacial,
    int Quantite,
    decimal ParTitreExact,
    decimal ParTitre,
    decimal Total);

/// <summary>
/// Le coupon couru : la part du prochain coupon gagnée par le vendeur depuis le dernier coupon,
/// que l'acheteur lui rembourse.
///
///   coupon couru d'un titre = nominal restant × taux facial × jours / jours de l'année
///
/// Les jours et l'année de référence sont ceux de la convention du titre. Le nominal restant
/// est celui de la période en cours (après les remboursements déjà faits). Le total est arrondi
/// une seule fois, à la fin : nominal × taux × jours / base × quantité.
/// </summary>
public static class CouponCouru
{
    public static ResultatCouponCouru Calculer(CaracteristiquesObligation o, DateOnly dateArrete, int quantite)
    {
        if (quantite <= 0) throw new DomainException("La quantité doit être d'au moins un titre.");

        var (debut, fin) = CalendrierCoupons.PeriodeEnCours(o, dateArrete);
        var convention = o.Convention;
        var jours = convention.Jours(debut, dateArrete);
        var nominalRestant = Amortissement.NominalRestant(o, debut);

        var parTitreExact = nominalRestant * (o.TauxFacial / 100m) * jours / convention.JoursAnnee;

        return new ResultatCouponCouru(
            dateArrete, debut, fin, convention.Libelle, jours, convention.JoursAnnee,
            nominalRestant, o.TauxFacial, quantite,
            parTitreExact,
            Arrondis.Montant(parTitreExact),
            Arrondis.Montant(parTitreExact * quantite));
    }
}
