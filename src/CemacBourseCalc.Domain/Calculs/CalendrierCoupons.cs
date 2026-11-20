using CemacBourseCalc.Domain.Communs;

namespace CemacBourseCalc.Domain.Calculs;

/// <summary>
/// Une période de coupon. Régulière : elle dure exactement une périodicité entre deux dates
/// d'ancrage, et le coupon vaut taux / nombre de coupons par an. Irrégulière (premier coupon
/// long ou court, dernière période courte, coupon unique in fine) : le coupon est calculé au
/// prorata, avec la convention du titre.
/// </summary>
public sealed record PeriodeCoupon(int Rang, DateOnly Debut, DateOnly Fin, bool Reguliere);

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
    public static IReadOnlyList<DateOnly> Dates(CaracteristiquesObligation o) =>
        Periodes(o).Select(p => p.Fin).ToList();

    public static IReadOnlyList<PeriodeCoupon> Periodes(CaracteristiquesObligation o)
    {
        var mois = (int)o.Periodicite;
        if (mois == 0) return [new PeriodeCoupon(1, o.DateJouissance, o.DateEcheance, false)]; // in fine : un coupon unique

        var (ancrage, premierRang) = o.DatePremierCoupon is { } premier ? (premier, 0) : (o.DateJouissance, 1);

        // Les dates d'ancrage strictement avant l'échéance, puis l'échéance elle-même.
        var dates = new List<DateOnly>();
        var echeanceAncree = false;
        for (var k = premierRang; ; k++)
        {
            var date = ancrage.AddMonths(k * mois);
            if (date >= o.DateEcheance) { echeanceAncree = date == o.DateEcheance; break; }
            dates.Add(date);
        }
        dates.Add(o.DateEcheance);

        var premiereReguliere = o.DatePremierCoupon is null || o.DatePremierCoupon == o.DateJouissance.AddMonths(mois);

        var periodes = new List<PeriodeCoupon>(dates.Count);
        var debut = o.DateJouissance;
        for (var i = 0; i < dates.Count; i++)
        {
            var derniere = i == dates.Count - 1;
            var reguliere = i == 0
                ? premiereReguliere && (!derniere || echeanceAncree)
                : !derniere || echeanceAncree;

            periodes.Add(new PeriodeCoupon(i + 1, debut, dates[i], reguliere));
            debut = dates[i];
        }

        return periodes;
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

        var periode = Periodes(o).First(p => p.Fin > date);
        return (periode.Debut, periode.Fin);
    }
}
