using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Calculs;

/// <summary>
/// Les dates de coupon d'une obligation, de la première à l'échéance.
///
/// Chaque date est calculée depuis une date d'ancrage (+ k périodes), jamais de proche en proche :
/// un ancrage au 31 donne le 28 ou le 29 février puis revient au 31 mars, sans glisser.
/// L'ancrage est la date du premier coupon si le prospectus en fixe une (premier coupon
/// irrégulier), sinon la date de jouissance. La dernière date est toujours l'échéance.
///
/// Ce sont les dates THÉORIQUES, qui servent au décompte des intérêts. Le report d'un paiement
/// tombant un jour non ouvré ne change pas le décompte ; il relève de l'échéancier des paiements.
/// </summary>
public static class CalendrierCoupons
{
    public static IReadOnlyList<DateOnly> Dates(CaracteristiquesObligation o)
    {
        var mois = (int)o.Periodicite;
        if (mois == 0) return [o.DateEcheance]; // in fine : un seul coupon, à l'échéance

        var dates = new List<DateOnly>();
        var (ancrage, premierRang) = o.DatePremierCoupon is { } premier ? (premier, 0) : (o.DateJouissance, 1);

        for (var k = premierRang; ; k++)
        {
            var date = ancrage.AddMonths(k * mois);
            if (date >= o.DateEcheance) break;
            dates.Add(date);
        }

        dates.Add(o.DateEcheance);
        return dates;
    }

    /// <summary>
    /// La période de coupon qui contient <paramref name="date"/> : elle commence au dernier coupon
    /// détaché (ou à la jouissance) et finit au prochain. Un jour de coupon ouvre la période suivante.
    /// </summary>
    public static (DateOnly Debut, DateOnly Fin) PeriodeEnCours(CaracteristiquesObligation o, DateOnly date)
    {
        if (date < o.DateJouissance)
            throw new DomainException($"Le {date:dd/MM/yyyy} est avant la date de jouissance du titre {o.Nom} ({o.DateJouissance:dd/MM/yyyy}).");
        if (date >= o.DateEcheance)
            throw new DomainException($"Le titre {o.Nom} est arrivé à échéance le {o.DateEcheance:dd/MM/yyyy} : il ne porte plus d'intérêts.");

        var debut = o.DateJouissance;
        foreach (var coupon in Dates(o))
        {
            if (coupon > date) return (debut, coupon);
            debut = coupon;
        }

        // Inatteignable : la dernière date est l'échéance, qui est après la date.
        throw new InvalidOperationException("Période de coupon introuvable.");
    }
}
